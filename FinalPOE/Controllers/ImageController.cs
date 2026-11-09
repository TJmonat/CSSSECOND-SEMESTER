using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;

using RaceDayAP1.Services;

namespace RaceDayAP1.Controllers

{

    [ApiController]

    [Route("api/[controller]")]

    [Authorize]

    public class ImagesController : ControllerBase

    {

        private readonly AzureBlobStorageService _storageService;

        public ImagesController(

            AzureBlobStorageService storageService)

        {

            _storageService = storageService;

        }

        

        [HttpPost("event-banner/{eventId}")]

        [Authorize(Roles = "Organiser")]

        [RequestSizeLimit(6 * 1024 * 1024)]

        public async Task<IActionResult> UploadEventBanner(

            string eventId,

            IFormFile image)

        {

            try

            {

                if (string.IsNullOrWhiteSpace(eventId))

                    return BadRequest("Event ID is required.");

                string imageUrl =

                    await _storageService.UploadImageAsync(

                        image,

                        "event-banners",

                        eventId);

                return Ok(new

                {

                    Message = "Event banner uploaded successfully.",

                    Event_ID = eventId,

                    ImageUrl = imageUrl

                });

            }

            catch (ArgumentException ex)

            {

                return BadRequest(new { Message = ex.Message });

            }

            catch (Exception)

            {

                return StatusCode(500, new

                {

                    Message = "Unable to upload the event banner."

                });

            }

        }

        
        [HttpPost("profile-picture")]

        [Authorize(Roles = "Participant")]

        [RequestSizeLimit(6 * 1024 * 1024)]

        public async Task<IActionResult> UploadProfilePicture(

            IFormFile image)

        {

            try

            {

                string? participantId =

                    User.FindFirst("Participant_ID")?.Value;

                if (string.IsNullOrWhiteSpace(participantId))

                    return Forbid();

                string imageUrl =

                    await _storageService.UploadImageAsync(

                        image,

                        "profile-pictures",

                        participantId);

                return Ok(new

                {

                    Message = "Profile picture uploaded successfully.",

                    ImageUrl = imageUrl

                });

            }

            catch (ArgumentException ex)

            {

                return BadRequest(new { Message = ex.Message });

            }

            catch (Exception)

            {

                return StatusCode(500, new

                {

                    Message = "Unable to upload the profile picture."

                });

            }

        }

        

        [HttpGet("profile-picture")]

        [Authorize(Roles = "Participant")]

        public async Task<IActionResult> GetMyProfilePicture()

        {

            string? participantId =

                User.FindFirst("Participant_ID")?.Value;

            if (string.IsNullOrWhiteSpace(participantId))

                return Forbid();

            var result =

                await _storageService.GetProfilePictureAsync(

                    participantId);

            if (result.Content == null)

                return NotFound("No profile picture was found.");

            return File(

                result.Content,

                result.ContentType ?? "application/octet-stream");

        }

        

        [HttpGet("event-banner/{eventId}")]

        [AllowAnonymous]

        public async Task<IActionResult> GetEventBanner(

            string eventId)

        {

            if (string.IsNullOrWhiteSpace(eventId))

                return BadRequest();

            var result =

                await _storageService.GetEventBannerAsync(eventId);

            if (result.Content == null)

                return NotFound(

                    "No banner image was found for this event.");

            return File(

                result.Content,

                result.ContentType ?? "application/octet-stream");

        }

    }

}