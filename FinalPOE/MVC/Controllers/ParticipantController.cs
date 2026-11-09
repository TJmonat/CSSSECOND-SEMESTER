using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;

using RaceDayMVC.Models;

using System.Net.Http.Json;

namespace RaceDayMVC.Controllers

{

    [Authorize(Roles = "Participant")]

    public class ParticipantController : Controller

    {

        private readonly IHttpClientFactory _httpClientFactory;

        public ParticipantController(

            IHttpClientFactory httpClientFactory)

        {

            _httpClientFactory = httpClientFactory;

        }

        private HttpClient CreateApiClient()

        {

            var client =

                _httpClientFactory.CreateClient("RaceDayAPI");

            var apiCookie =

                HttpContext.Session.GetString("ApiCookie");

            if (!string.IsNullOrEmpty(apiCookie))

            {

                client.DefaultRequestHeaders.TryAddWithoutValidation(

                    "Cookie", apiCookie);

            }

            return client;

        }

      

        public async Task<IActionResult> Index(

            string? location,

            DateTime? eventDate,

            string? category)

        {

            var client = CreateApiClient();

            try

            {

                var events =

                    await client.GetFromJsonAsync<List<EventViewModel>>(

                        "api/events");

                events ??= new List<EventViewModel>();

                var upcomingEvents = events

                    .Where(e => e.Event_Date.Date >= DateTime.Today);

                if (!string.IsNullOrWhiteSpace(location))

                {

                    upcomingEvents = upcomingEvents.Where(e =>

                        e.Event_Location != null &&

                        e.Event_Location.Contains(

                            location,

                            StringComparison.OrdinalIgnoreCase));

                }

                if (eventDate.HasValue)

                {

                    upcomingEvents = upcomingEvents.Where(e =>

                        e.Event_Date.Date == eventDate.Value.Date);

                }

                if (!string.IsNullOrWhiteSpace(category))

                {

                    upcomingEvents = upcomingEvents.Where(e =>

                        e.CategoryID != null &&

                        e.CategoryID.Equals(

                            category,

                            StringComparison.OrdinalIgnoreCase));

                }

                ViewBag.Location = location;

                ViewBag.EventDate =

                    eventDate?.ToString("yyyy-MM-dd");

                ViewBag.Category = category;

                return View(

                    upcomingEvents.OrderBy(e => e.Event_Date).ToList());

            }

            catch (HttpRequestException)

            {

                ViewBag.Error =

                    "Could not retrieve events from the API.";

                return View(new List<EventViewModel>());

            }

        }

       

        public async Task<IActionResult> Details(string id)

        {

            var client = CreateApiClient();

            try

            {

                var selectedEvent =

                    await client.GetFromJsonAsync<EventViewModel>(

                        $"api/events/{Uri.EscapeDataString(id)}");

                if (selectedEvent == null)

                    return NotFound("Event not found.");

                var categories =

                    await client.GetFromJsonAsync<List<CategoryViewModel>>(

                        "api/categories");

                categories ??= new List<CategoryViewModel>();

                ViewBag.Categories = categories;

                return View(selectedEvent);

            }

            catch (HttpRequestException)

            {

                return View("Error");

            }

        }

       

        public async Task<IActionResult> MyEnrolments()

        {

            var client = CreateApiClient();

            try

            {

                var enrolments =

                    await client.GetFromJsonAsync<List<MyEnrolmentViewModel>>(

                        "api/enrolments/mine");

                return View(

                    enrolments ?? new List<MyEnrolmentViewModel>());

            }

            catch (HttpRequestException)

            {

                ViewBag.Error =

                    "Could not retrieve your enrolments.";

                return View(new List<MyEnrolmentViewModel>());

            }

        }

       

        public async Task<IActionResult> MyResults()

        {

            var client = CreateApiClient();

            try

            {

                var results =

                    await client.GetFromJsonAsync<List<MyResultViewModel>>(

                        "api/raceresults/mine");

                return View(results ?? new List<MyResultViewModel>());

            }

            catch (HttpRequestException)

            {

                ViewBag.Error =

                    "Could not retrieve your race results.";

                return View(new List<MyResultViewModel>());

            }

        }

        

        [HttpGet]

        public async Task<IActionResult> Enrol(string id)

        {

            var client = CreateApiClient();

            try

            {

                var selectedEvent =

                    await client.GetFromJsonAsync<EventViewModel>(

                        $"api/events/{Uri.EscapeDataString(id)}");

                if (selectedEvent == null)

                    return NotFound("Event not found.");

                if (selectedEvent.Event_Date.Date < DateTime.Today)

                {

                    return BadRequest(

                        "You cannot enrol for a completed event.");

                }

                var categories =

                    await client.GetFromJsonAsync<List<EventCategoryOptionViewModel>>(

                        $"api/eventcategoryoptions/{Uri.EscapeDataString(id)}");

                categories ??= new List<EventCategoryOptionViewModel>();

                ViewBag.AvailableCategories = categories;

                var allCategories =

                    await client.GetFromJsonAsync<List<CategoryViewModel>>(

                        "api/categories");

                allCategories ??= new List<CategoryViewModel>();

                ViewBag.AllCategories = allCategories;

                return View(selectedEvent);

            }

            catch (HttpRequestException)

            {

                return View("Error");

            }

        }

        

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Enrol(

            string id,

            bool confirm,

            string categoryId)

        {

            if (!confirm)

                return RedirectToAction("Details", new { id });

            if (string.IsNullOrWhiteSpace(categoryId))

            {

                ViewBag.Error = "Please select a category.";

                return await Enrol(id);

            }

            var client = CreateApiClient();

            var enrolment = new

            {

                Registration_ID = Guid.NewGuid().ToString("N")[..13],

                EventID = id,

                Category_ID = categoryId

            };

            try

            {

                var response = await client.PostAsJsonAsync(

                    "api/enrolments", enrolment);

                if (response.IsSuccessStatusCode)

                {

                    TempData["Success"] =

                        "You have successfully enrolled for this event!";

                    return RedirectToAction("Details", new { id });

                }

                ViewBag.Error =

                    await response.Content.ReadAsStringAsync();

                return await Enrol(id);

            }

            catch (HttpRequestException)

            {

                ViewBag.Error =

                    "Could not connect to the RaceDay API.";

                return await Enrol(id);

            }

        }

        

        [HttpGet]

        public async Task<IActionResult> Profile()

        {

            var client = CreateApiClient();

            try

            {

                var profile =

                    await client.GetFromJsonAsync<ProfileViewModel>(

                        "api/participants/mine");

                if (profile == null)

                    return NotFound("Profile not found.");

                profile.ProfilePictureUrl =

                    Url.Action(

                        "ProfilePictureImage",

                        "Participant");

                return View(profile);

            }

            catch (HttpRequestException)

            {

                ViewBag.Error =

                    "Could not retrieve your profile.";

                return View(new ProfileViewModel());

            }

        }

       

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Profile(

            ProfileViewModel profile)

        {

            var client = CreateApiClient();

            try

            {


                profile.ProfilePicture = null;

                profile.ProfilePictureUrl = null;

                var response = await client.PutAsJsonAsync(

                    "api/participants/mine", profile);

                if (response.IsSuccessStatusCode)

                {

                    TempData["Success"] =

                        "Your profile was updated successfully!";

                    return RedirectToAction("Profile");

                }

                ViewBag.Error =

                    await response.Content.ReadAsStringAsync();

                return View(profile);

            }

            catch (HttpRequestException)

            {

                ViewBag.Error =

                    "Could not connect to the RaceDay API.";

                return View(profile);

            }

        }

        
        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> UploadProfilePicture(

            IFormFile profilePicture)

        {

            if (profilePicture == null ||

                profilePicture.Length == 0)

            {

                TempData["Error"] =

                    "Please select a picture.";

                return RedirectToAction("Profile");

            }

            var client = CreateApiClient();

            try

            {

                using var content =

                    new MultipartFormDataContent();

                using var stream =

                    profilePicture.OpenReadStream();

                using var fileContent =

                    new StreamContent(stream);

                content.Add(

                    fileContent,

                    "image",

                    profilePicture.FileName);

                var response = await client.PostAsync(

                    "api/Images/profile-picture",

                    content);

                if (response.IsSuccessStatusCode)

                {

                    TempData["Success"] =

                        "Profile picture uploaded successfully!";

                }

                else

                {

                    TempData["Error"] =

                        await response.Content.ReadAsStringAsync();

                }

            }

            catch (HttpRequestException)

            {

                TempData["Error"] =

                    "Could not connect to the RaceDay API.";

            }

            return RedirectToAction("Profile");

        }

       

        [HttpGet]

        public async Task<IActionResult> ProfilePictureImage()

        {

            var client = CreateApiClient();

            try

            {

                var response = await client.GetAsync(

                    "api/Images/profile-picture");

                if (!response.IsSuccessStatusCode)

                    return NotFound();

                var image =

                    await response.Content.ReadAsByteArrayAsync();

                var contentType =

                    response.Content.Headers.ContentType?.MediaType

                    ?? "application/octet-stream";

                return File(image, contentType);

            }

            catch (HttpRequestException)

            {

                return NotFound();

            }

        }

    }

}