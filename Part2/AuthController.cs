using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaceDayAP1.Data;
using RaceDayAP1.Models;
using System.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Cryptography;

namespace RaceDayAP1.Controllers

{
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{private readonly RaceDayContext _db;
        private readonly IPasswordHasher<UserAccount> _hasher;
        public AuthController(
            RaceDayContext db,
            IPasswordHasher<UserAccount> hasher)

        {

            _db = db;

            _hasher = hasher;

        }

        [HttpPost("register")]

        public async Task<IActionResult> Register(

            [FromBody] RegisterRequest request)

        {

            var role = request.UserRole.Trim();

            if (role != "Participant" && role != "Organiser")

            {

                return BadRequest(

                    "Role must be Participant or Organiser.");

            }

            var email = request.Email.Trim().ToLowerInvariant();

            if (await _db.UserAccounts.AnyAsync(

                u => u.Email.ToLower() == email))

            {

                return Conflict("This email is already registered.");

            }

           

            string profileId = Guid.NewGuid().ToString("N")[..13];

            await using var transaction =

                await _db.Database.BeginTransactionAsync(

                    IsolationLevel.Serializable);

            try

            {

                var account = new UserAccount

                {

                    Email = email,

                    UserRole = role

                };

                if (role == "Participant")

                {

                    _db.Participants.Add(new Participant

                    {

                        Participant_ID = profileId,

                        Participant_Name = request.Name.Trim(),

                        Participant_Surname = request.Surname.Trim(),

                        Participant_Email = email

                    });

                    account.Participant_ID = profileId;

                }

                else

                {

                    _db.EventOrganizers.Add(new EventOrganizer

                    {

                        Event_Organizer_ID = profileId,

                        Event_Organizer_Name = request.Name.Trim(),

                        Event_Organizer_Surname = request.Surname.Trim(),

                        Event_Organizers_Email = email

                    });

                    account.Event_Organizer_ID = profileId;

                }

                account.PasswordHash = _hasher.HashPassword(

                    account, request.Password);

                _db.UserAccounts.Add(account);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return StatusCode(201, new

                {

                    message = "Registration successful.",

                    email = account.Email,

                    role = account.UserRole

                });

            }

            catch (DbUpdateException)

            {

                await transaction.RollbackAsync();

                return Conflict(

                    "Registration could not be completed. " +

                    "Check that the email and generated ID are unique.");

            }

        }
        [HttpPost("login")]

        public async Task<IActionResult> Login(

    [FromBody] LoginRequest request)

        {

            var email = request.Email.Trim().ToLowerInvariant();

            var account = await _db.UserAccounts

                .FirstOrDefaultAsync(u => u.Email == email);

            if (account == null)

            {

                return Unauthorized(

                    new { message = "Invalid email or password." });

            }

            var passwordResult = _hasher.VerifyHashedPassword(

                account,

                account.PasswordHash,

                request.Password);

            if (passwordResult == PasswordVerificationResult.Failed)

            {

                return Unauthorized(

                    new { message = "Invalid email or password." });

            }

            

            var claims = new List<Claim>

    {

        new Claim(ClaimTypes.NameIdentifier,

            account.UserAccount_ID.ToString()),

        new Claim(ClaimTypes.Name, account.Email),

        new Claim(ClaimTypes.Email, account.Email),

        new Claim(ClaimTypes.Role, account.UserRole)

    };

            var identity = new ClaimsIdentity(

                claims, "RaceDay.Auth");

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(

                "RaceDay.Auth",

                principal);

            return Ok(new

            {

                message = "Login successful.",

                email = account.Email,

                role = account.UserRole

            });

        }

        [Authorize]

        [HttpPost("logout")]

        public async Task<IActionResult> Logout()

        {

            await HttpContext.SignOutAsync("RaceDay.Auth");

            return Ok(new

            {

                message = "Logout successful."

            });

        }

    }

}