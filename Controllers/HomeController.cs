using CheapFlightsApp.Models;
using CheapFlightsApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CheapFlightsApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly FlightClient _flightService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, FlightClient flightsService, IConfiguration configuration)
        {
            _flightService = flightsService;
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GetFlights(SearchParameters searchData)
        {
            List<Flight> flightsModel = new List<Flight>();
            try
            {
                flightsModel = await _flightService.GetFlightsAsync(searchData);

                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                // Skip number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int totalrows = flightsModel.Count();

                var data = flightsModel.ToList();

                return Json(new { draw = draw, data = data, recordsTotal = totalrows, recordsFiltered = totalrows});
            }
            catch (Exception)
            {
                return null;
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
