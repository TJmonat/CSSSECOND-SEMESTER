using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using RaceDayMVC.Models;

using System.Net.Http.Json;

namespace RaceDayMVC.Controllers

{

    [Authorize(Roles = "Organiser")]

    public class OrganiserController : Controller

    {

        private readonly IHttpClientFactory _httpClientFactory;

        public OrganiserController(IHttpClientFactory httpClientFactory)

        {

            _httpClientFactory = httpClientFactory;

        }

        // Organiser Dashboard

        public async Task<IActionResult> Index()

        {

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            try

            {

                var events = await client.GetFromJsonAsync<List<EventViewModel>>(

                    "api/events");

                var enrolments = await client.GetFromJsonAsync<List<EnrolmentViewModel>>(

                    "api/enrolments");

                events ??= new List<EventViewModel>();

                enrolments ??= new List<EnrolmentViewModel>();

                foreach (var item in events)

                {

                    item.TotalEnrolments = enrolments.Count(e =>

                        e.EventID == item.Event_ID);

                }

                ViewBag.UpcomingEvents = events

                    .Where(e => e.Event_Date.Date >= DateTime.Today)

                    .OrderBy(e => e.Event_Date)

                    .ToList();

                return View(events);

            }

            catch (HttpRequestException)

            {

                ViewBag.Error = "Could not connect to the RaceDay API.";

                return View(new List<EventViewModel>());

            }

        }

        // View enrolments for a selected event

        public async Task<IActionResult> Enrolments(string eventId)

        {

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var apiCookie = HttpContext.Session.GetString("ApiCookie");

            if (!string.IsNullOrEmpty(apiCookie))

            {

                client.DefaultRequestHeaders.Add("Cookie", apiCookie);

            }

            try

            {

                var enrolments = await client.GetFromJsonAsync<List<EnrolmentViewModel>>(

                    "api/enrolments");

                var participants = await client.GetFromJsonAsync<List<ParticipantViewModel>>(

                    "api/participants");

                var events = await client.GetFromJsonAsync<List<EventViewModel>>(

                    "api/events");

                enrolments ??= new List<EnrolmentViewModel>();

                participants ??= new List<ParticipantViewModel>();

                events ??= new List<EventViewModel>();

                var selectedEvent = events.FirstOrDefault(e =>

                    e.Event_ID == eventId);

                if (selectedEvent == null)

                    return NotFound("Event not found.");

                var eventEnrolments = enrolments

                    .Where(e => e.EventID == eventId)

                    .ToList();

                ViewBag.TotalEnrolments = enrolments.Count;

                ViewBag.MatchingEnrolments = eventEnrolments.Count;

                foreach (var enrolment in eventEnrolments)

                {

                    var participant = participants.FirstOrDefault(p =>

                        p.Participant_ID == enrolment.ParticipantID);

                    if (participant != null)

                    {

                        enrolment.Participant_Name = participant.Participant_Name;

                        enrolment.Participant_Surname = participant.Participant_Surname;

                    }

                    enrolment.CategoryID = selectedEvent.CategoryID;

                }

                ViewBag.EventID = selectedEvent.Event_ID;

                ViewBag.EventLocation = selectedEvent.Event_Location;

                return View(eventEnrolments);

            }

            catch (HttpRequestException)

            {

                ViewBag.Error = "Could not retrieve enrolments from the API.";

                return View(new List<EnrolmentViewModel>());

            }

        }

        // View all categories

        public async Task<IActionResult> Categories()

        {

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            try

            {

                var categories = await client.GetFromJsonAsync<List<CategoryViewModel>>(

                    "api/categories");

                categories ??= new List<CategoryViewModel>();

                return View(categories);

            }

            catch (HttpRequestException)

            {

                ViewBag.Error = "Could not retrieve event categories.";

                return View(new List<CategoryViewModel>());

            }

        }

        // Display Create Category Form

        [HttpGet]

        public IActionResult CreateCategory()

        {

            return View();

        }

        // Save New Category

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreateCategory(CategoryViewModel model)

        {

            if (!ModelState.IsValid)

                return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            try

            {

                var response = await client.PostAsJsonAsync(

                    "api/categories", model);

                if (response.IsSuccessStatusCode)

                {

                    TempData["Success"] = "Category created successfully!";

                    return RedirectToAction("Categories");

                }

                ModelState.AddModelError("",

                    "Could not create category. Check the Category ID and try again.");

                return View(model);

            }

            catch (HttpRequestException)

            {

                ModelState.AddModelError("",

                    "Could not connect to the RaceDay API.");

                return View(model);

            }

        }

        // Display Edit Category Form

        [HttpGet]

        public async Task<IActionResult> EditCategory(string id)

        {

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            try

            {

                var category = await client.GetFromJsonAsync<CategoryViewModel>(

                    $"api/categories/{Uri.EscapeDataString(id)}");

                if (category == null)

                    return NotFound();

                return View(category);

            }

            catch (HttpRequestException)

            {

                return View("Error");

            }

        }

        // Save Edited Category

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditCategory(

            string id, CategoryViewModel model)

        {

            if (!ModelState.IsValid)

                return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            try

            {

                var response = await client.PutAsJsonAsync(

                    $"api/categories/{Uri.EscapeDataString(id)}", model);

                if (response.IsSuccessStatusCode)

                {

                    TempData["Success"] = "Category updated successfully!";

                    return RedirectToAction("Categories");

                }

                ModelState.AddModelError("",

                    "Could not update category.");

                return View(model);

            }

            catch (HttpRequestException)

            {

                ModelState.AddModelError("",

                    "Could not connect to the RaceDay API.");

                return View(model);

            }

        }

        // Delete Category

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteCategory(string id)

        {

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            try

            {

                var response = await client.DeleteAsync(

                    $"api/categories/{Uri.EscapeDataString(id)}");

                if (response.IsSuccessStatusCode)

                {

                    TempData["Success"] = "Category deleted successfully!";

                }

                else

                {

                    TempData["Error"] =

                        "Category could not be deleted. It may be used by an existing event.";

                }

            }

            catch (HttpRequestException)

            {

                TempData["Error"] = "Could not connect to the RaceDay API.";

            }

            return RedirectToAction("Categories");

        }
        public async Task<IActionResult> Results()

        {

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var apiCookie = HttpContext.Session.GetString("ApiCookie");

            if (!string.IsNullOrEmpty(apiCookie))

            {

                client.DefaultRequestHeaders.Add("Cookie", apiCookie);

            }

            try

            {

                var results = await client.GetFromJsonAsync<List<RaceResultViewModel>>(

                    "api/raceresults");

                return View(results ?? new List<RaceResultViewModel>());

            }

            catch (HttpRequestException)

            {

                ViewBag.Error = "Could not retrieve race results.";

                return View(new List<RaceResultViewModel>());

            }

        }

        // Display Add Result form

        [HttpGet]

        public async Task<IActionResult> CreateResult()

        {

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var apiCookie = HttpContext.Session.GetString("ApiCookie");

            if (!string.IsNullOrEmpty(apiCookie))

            {

                client.DefaultRequestHeaders.Add("Cookie", apiCookie);

            }

            try

            {

                ViewBag.Enrolments =

                    await client.GetFromJsonAsync<List<EnrolmentViewModel>>(

                        "api/enrolments")

                    ?? new List<EnrolmentViewModel>();

            }

            catch (HttpRequestException)

            {

                ViewBag.Enrolments = new List<EnrolmentViewModel>();

                ViewBag.Error = "Could not retrieve enrolments.";

            }

            return View();

        }

        // Save new result

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreateResult(RaceResultViewModel model)

        {

            if (!ModelState.IsValid)

                return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var apiCookie = HttpContext.Session.GetString("ApiCookie");

            if (!string.IsNullOrEmpty(apiCookie))

            {

                client.DefaultRequestHeaders.Add("Cookie", apiCookie);

            }

            model.Finish_Time = model.Finish_Time;

            try

            {

                var response = await client.PostAsJsonAsync(

                    "api/raceresults", model);

                if (response.IsSuccessStatusCode)

                {

                    TempData["Success"] = "Race result added successfully!";

                    return RedirectToAction("Results");

                }

                ModelState.AddModelError("",

                    "Could not save the result. Check the registration and result details.");

            }

            catch (HttpRequestException)

            {

                ModelState.AddModelError("",

                    "Could not connect to the RaceDay API.");

            }

            return View(model);

        }

        // Display Edit Result form

        [HttpGet]

        public async Task<IActionResult> EditResult(string id)

        {

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var apiCookie = HttpContext.Session.GetString("ApiCookie");

            if (!string.IsNullOrEmpty(apiCookie))

            {

                client.DefaultRequestHeaders.Add("Cookie", apiCookie);

            }

            var result = await client.GetFromJsonAsync<RaceResultViewModel>(

                $"api/raceresults/{Uri.EscapeDataString(id)}");

            if (result == null)

                return NotFound();

            return View(result);

        }

        // Save edited result

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditResult(

            string id, RaceResultViewModel model)

        {

            if (!ModelState.IsValid)

                return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var apiCookie = HttpContext.Session.GetString("ApiCookie");

            if (!string.IsNullOrEmpty(apiCookie))

            {

                client.DefaultRequestHeaders.Add("Cookie", apiCookie);

            }

            var response = await client.PutAsJsonAsync(

                $"api/raceresults/{Uri.EscapeDataString(id)}", model);

            if (response.IsSuccessStatusCode)

            {

                TempData["Success"] = "Race result updated successfully!";

                return RedirectToAction("Results");

            }

            ModelState.AddModelError("", "Could not update race result.");

            return View(model);

        }

        // Delete result

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteResult(string id)

        {

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var apiCookie = HttpContext.Session.GetString("ApiCookie");

            if (!string.IsNullOrEmpty(apiCookie))

            {

                client.DefaultRequestHeaders.Add("Cookie", apiCookie);

            }

            var response = await client.DeleteAsync(

                $"api/raceresults/{Uri.EscapeDataString(id)}");

            if (response.IsSuccessStatusCode)

            {

                TempData["Success"] = "Race result deleted successfully!";

            }

            else

            {

                TempData["Error"] = "Could not delete race result.";

            }

            return RedirectToAction("Results");

        }

    }

}