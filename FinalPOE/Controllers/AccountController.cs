using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Authentication.Cookies;

using System.Security.Claims;

using System.Net.Http.Json;

using System.Text.Json;

using RaceDayMVC.Models;

namespace RaceDayMVC.Controllers

{

    public class AccountController : Controller

    {

        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)

        {

            _httpClientFactory = httpClientFactory;

        }


        [HttpGet]

        public IActionResult Register()

        {

            return View();

        }

        

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Register(RegisterViewModel model)

        {

            if (!ModelState.IsValid)

                return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            try

            {

                var response = await client.PostAsJsonAsync(

                    "api/auth/register", model);

                if (response.IsSuccessStatusCode)

                {

                    TempData["Success"] =

                        "Registration successful. Please log in.";

                    return RedirectToAction("Login");

                }

                ModelState.AddModelError(

                    string.Empty,

                    "Registration failed. Please check your details or email address.");

                return View(model);

            }

            catch (HttpRequestException)

            {

                ModelState.AddModelError(

                    string.Empty,

                    "We cannot connect to RaceDay right now. Please try again later.");

                return View(model);

            }

            catch (TaskCanceledException)

            {

                ModelState.AddModelError(

                    string.Empty,

                    "The request took too long. Please try again.");

                return View(model);

            }

        }

        

        [HttpGet]

        public IActionResult Login()

        {

            return View();

        }

       

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(LoginViewModel model)

        {

            if (!ModelState.IsValid)

                return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            try

            {

                var response = await client.PostAsJsonAsync(

                    "api/auth/login", model);

                if (!response.IsSuccessStatusCode)

                {

                    ModelState.AddModelError(

                        string.Empty,

                        "Invalid email or password.");

                    return View(model);

                }

                

                if (response.Headers.TryGetValues(

                    "Set-Cookie", out var cookieHeaders))

                {

                    var cookie = cookieHeaders.FirstOrDefault();

                    if (!string.IsNullOrEmpty(cookie))

                    {

                        var cookieValue = cookie.Split(';')[0];

                        HttpContext.Session.SetString(

                            "ApiCookie", cookieValue);

                    }

                }

                var result =

                    await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (result == null ||

                    string.IsNullOrEmpty(result.Role))

                {

                    HttpContext.Session.Remove("ApiCookie");

                    ModelState.AddModelError(

                        string.Empty,

                        "Login could not be completed. Please try again.");

                    return View(model);

                }

                if (result.Role != "Participant" &&

                    result.Role != "Organiser")

                {

                    HttpContext.Session.Remove("ApiCookie");

                    ModelState.AddModelError(

                        string.Empty,

                        "Your account has an invalid role. Please contact support.");

                    return View(model);

                }

               

                var claims = new List<Claim>

                {

                    new Claim(

                        ClaimTypes.Name,

                        result.Email ?? model.Email),

                    new Claim(

                        ClaimTypes.Email,

                        result.Email ?? model.Email),

                    new Claim(

                        ClaimTypes.Role,

                        result.Role)

                };

                var identity = new ClaimsIdentity(

                    claims,

                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(

                    CookieAuthenticationDefaults.AuthenticationScheme,

                    principal);

                TempData["Success"] = "Login successful.";

                

                if (result.Role == "Participant")

                {

                    return RedirectToAction("Index", "Participant");

                }

                return RedirectToAction("Index", "Organiser");

            }

            catch (HttpRequestException)

            {

                ModelState.AddModelError(

                    string.Empty,

                    "We cannot connect to RaceDay right now. Please try again later.");

                return View(model);

            }

            catch (TaskCanceledException)

            {

                ModelState.AddModelError(

                    string.Empty,

                    "The login request took too long. Please try again.");

                return View(model);

            }

            catch (JsonException)

            {

                ModelState.AddModelError(

                    string.Empty,

                    "We received an invalid response. Please try again later.");

                return View(model);

            }

        }

        

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Logout()

        {

            var apiCookie =

                HttpContext.Session.GetString("ApiCookie");

            if (!string.IsNullOrEmpty(apiCookie))

            {

                try

                {

                    var client =

                        _httpClientFactory.CreateClient("RaceDayAPI");

                    client.DefaultRequestHeaders.Add(

                        "Cookie", apiCookie);

                    await client.PostAsync(

                        "api/auth/logout", null);

                }

                catch (HttpRequestException)

                {

                  

                }

                catch (TaskCanceledException)

                {

                   

                }

            }

            await HttpContext.SignOutAsync(

                CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");

        }

    }

    public class LoginResponse

    {

        public string? Message { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }

    }

}