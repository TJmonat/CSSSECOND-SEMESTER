using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using RaceDayMVC.Models;

using System.Globalization;

using System.Net;

using System.Net.Http.Json;

namespace RaceDayMVC.Controllers

{

    [Authorize(Roles = "Organiser")]

    public class EventsController : Controller

    {

        private readonly IHttpClientFactory _httpClientFactory;

        public EventsController(IHttpClientFactory httpClientFactory)

        {

            _httpClientFactory = httpClientFactory;

        }

       

        private HttpClient GetApiClient()

        {

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var apiCookie = HttpContext.Session.GetString("ApiCookie");

            if (!string.IsNullOrEmpty(apiCookie))

            {

                client.DefaultRequestHeaders.TryAddWithoutValidation(

                    "Cookie", apiCookie);

            }

            return client;

        }

        
        [HttpGet]

        public IActionResult Create()

        {

            return View();

        }

      

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(EventViewModel model)

        {

            if (!ModelState.IsValid)

                return View(model);

            var client = GetApiClient();

            var raceEvent = new

            {

                Event_ID = model.Event_ID,

                Event_Location = model.Event_Location,

                Event_Date = model.Event_Date,

                CategoryID = model.CategoryID,

                EventOrganizerID = model.EventOrganizerID,

                VenueID = model.VenueID

            };

            var eventResponse = await client.PostAsJsonAsync(

                "api/events", raceEvent);

            if (!eventResponse.IsSuccessStatusCode)

            {

                ModelState.AddModelError("",

                    "Could not create the event. Check your Category ID, Organiser ID and Venue ID.");

                return View(model);

            }

            var eventDetails = new EventDetailsViewModel

            {

                EventDetails_ID = Guid.NewGuid().ToString("N")[..13],

                Event_ID = model.Event_ID,

                Event_Name = model.Event_Name ?? "",

                Event_Description = model.Event_Description,

                Event_Distance = model.Event_Distance?.ToString(

                    CultureInfo.InvariantCulture),

                Event_Type = model.Event_Type

            };

            var detailsResponse = await client.PostAsJsonAsync(

                "api/eventdetails", eventDetails);

            if (!detailsResponse.IsSuccessStatusCode)

            {

                await client.DeleteAsync(

                    $"api/events/{Uri.EscapeDataString(model.Event_ID)}");

                ModelState.AddModelError("",

                    "The event was created, but its additional details could not be saved. Please check the event details and try again.");

                return View(model);

            }

           

            if (model.BannerImage != null &&

                model.BannerImage.Length > 0)

            {

                using var formData = new MultipartFormDataContent();

                using var imageStream =

                    model.BannerImage.OpenReadStream();

                using var imageContent =

                    new StreamContent(imageStream);

                imageContent.Headers.ContentType =

                    new System.Net.Http.Headers.MediaTypeHeaderValue(

                        model.BannerImage.ContentType);

                formData.Add(

                    imageContent,

                    "image",

                    model.BannerImage.FileName);

                var imageResponse = await client.PostAsync(

                    $"api/Images/event-banner/{Uri.EscapeDataString(model.Event_ID)}",

                    formData);

                if (!imageResponse.IsSuccessStatusCode)

                {

                    await client.DeleteAsync(

                        $"api/eventdetails/{Uri.EscapeDataString(model.Event_ID)}");

                    await client.DeleteAsync(

                        $"api/events/{Uri.EscapeDataString(model.Event_ID)}");

                    ModelState.AddModelError("",

                        "The event banner could not be uploaded. The event was not saved. Please try again.");

                    return View(model);

                }

            }

            TempData["Success"] =

                "Event and banner created successfully!";

            return RedirectToAction("Index", "Organiser");

        }

       

        [HttpGet]

        public async Task<IActionResult> Edit(string id)

        {

            var client = GetApiClient();

            var raceEvent = await client.GetFromJsonAsync<EventViewModel>(

                $"api/events/{Uri.EscapeDataString(id)}");

            if (raceEvent == null)

                return NotFound();

            var details = await client.GetFromJsonAsync<EventDetailsViewModel>(

                $"api/eventdetails/{Uri.EscapeDataString(id)}");

            if (details != null)

            {

                raceEvent.Event_Name = details.Event_Name;

                raceEvent.Event_Description = details.Event_Description;

                raceEvent.Event_Type = details.Event_Type;

                if (decimal.TryParse(

                    details.Event_Distance,

                    NumberStyles.Number,

                    CultureInfo.InvariantCulture,

                    out decimal distance))

                {

                    raceEvent.Event_Distance = distance;

                }

            }

            return View(raceEvent);

        }

        

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(EventViewModel model)

        {

            if (!ModelState.IsValid)

                return View(model);

            var client = GetApiClient();

            var updatedEvent = new

            {

                Event_ID = model.Event_ID,

                Event_Location = model.Event_Location,

                Event_Date = model.Event_Date,

                CategoryID = model.CategoryID,

                EventOrganizerID = model.EventOrganizerID,

                VenueID = model.VenueID

            };

            var eventResponse = await client.PutAsJsonAsync(

                $"api/events/{Uri.EscapeDataString(model.Event_ID)}",

                updatedEvent);

            if (!eventResponse.IsSuccessStatusCode)

            {

                ModelState.AddModelError("",

                    "Could not update the event. Check your Category ID, Organiser ID and Venue ID.");

                return View(model);

            }

            var eventDetails = new EventDetailsViewModel

            {

                EventDetails_ID = Guid.NewGuid().ToString("N")[..13],

                Event_ID = model.Event_ID,

                Event_Name = model.Event_Name ?? "",

                Event_Description = model.Event_Description,

                Event_Distance = model.Event_Distance?.ToString(

                    CultureInfo.InvariantCulture),

                Event_Type = model.Event_Type

            };

            var detailsResponse = await client.PutAsJsonAsync(

                $"api/eventdetails/{Uri.EscapeDataString(model.Event_ID)}",

                eventDetails);

            if (detailsResponse.StatusCode == HttpStatusCode.NotFound)

            {

                detailsResponse = await client.PostAsJsonAsync(

                    "api/eventdetails", eventDetails);

            }

            if (!detailsResponse.IsSuccessStatusCode)

            {

                ModelState.AddModelError("",

                    "The event was updated, but its additional details could not be saved.");

                return View(model);

            }

            TempData["Success"] = "Event updated successfully!";

            return RedirectToAction("Index", "Organiser");

        }

        

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete(string id)

        {

            var client = GetApiClient();

            var detailsResponse = await client.DeleteAsync(

                $"api/eventdetails/{Uri.EscapeDataString(id)}");

            if (!detailsResponse.IsSuccessStatusCode &&

                detailsResponse.StatusCode != HttpStatusCode.NotFound)

            {

                TempData["Error"] =

                    "Could not delete the event details.";

                return RedirectToAction("Index", "Organiser");

            }

            var response = await client.DeleteAsync(

                $"api/events/{Uri.EscapeDataString(id)}");

            if (response.IsSuccessStatusCode)

            {

                TempData["Success"] = "Event deleted successfully!";

            }

            else if (response.StatusCode == HttpStatusCode.Conflict)

            {

                TempData["Error"] =

                    "This event cannot be deleted because it has registrations.";

            }

            else

            {

                TempData["Error"] = "Could not delete the event.";

            }

            return RedirectToAction("Index", "Organiser");

        }

    }

}