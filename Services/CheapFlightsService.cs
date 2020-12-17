using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CheapFlightsApp.Models;
using Newtonsoft.Json;
using static CheapFlightsApp.Models.JsonModel;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace CheapFlightsApp.Services
{
    public class FlightClient
    {
        HttpClient httpClient = new HttpClient();
        private readonly string TokenUrl;
        private readonly string client_id;      // Api Key
        private readonly string client_secret;  // Api Secret

        public FlightClient(IConfiguration configuration) 
        {
            TokenUrl = configuration.GetSection("AmadeusApi")["TokenUrl"];
            client_id = configuration.GetSection("AmadeusApi")["client_id"];
            client_secret = configuration.GetSection("AmadeusApi")["client_secret"];
        }

        // GET request - store response data in a List<Flight> model
        public async Task<List<Flight>> GetFlightsAsync(SearchParameters searchData)
        {
            var ApiParams = QueryString(searchData); // construct query parameters

            // GET request - (if unauthorized make a POST request to retrieve access token)
            // -----------
            var responseContent = await httpClient.GetAsync($"https://test.api.amadeus.com/v2/shopping/flight-offers?{ApiParams}"); 

            if (responseContent.StatusCode == HttpStatusCode.Unauthorized) {
                if (!await ClientAuth())
                {
                    return null;
                }
                responseContent = await httpClient.GetAsync($"https://test.api.amadeus.com/v2/shopping/flight-offers?{ApiParams}");
            }
            else if (responseContent.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }

            var flightsResponse = await responseContent.Content.ReadAsStringAsync();

            Root FlightsData = JsonConvert.DeserializeObject<Root>(flightsResponse); // Deserialize JSON to objects

            var flightsData = SortFlights(FlightsData);     // Populate List<Flight> model

            return flightsData;
        }


        // Setup httpClient
        private async Task<bool> ClientAuth()
        {
            var TokenData = await GetToken();
            if (TokenData == null)
            {
                httpClient.DefaultRequestHeaders.Authorization = null;
                return false;
            }
            httpClient.DefaultRequestHeaders.Authorization = null;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.access_token);
            return true;
        }


        // POST request - retrieve access Token
        private async Task<TokenData> GetToken() 
        {
            var keyValues = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"client_id", client_id},
                {"client_secret", client_secret},
            };

            // Generate access token
            // ------------
            HttpContent content = new FormUrlEncodedContent(keyValues);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var responseResult = (await httpClient.PostAsync(TokenUrl, content));
            if (responseResult.IsSuccessStatusCode)
            {
                var jsonResults = responseResult.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<TokenData>(jsonResults);   //Deserialize JSON response to object

                return data;
            }
            else
            {
                //responseResult.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }

        }


        // Construct query parameters for GET request
        private string QueryString(SearchParameters data)
        {
            var sb = new StringBuilder();
            sb.Append($"originLocationCode={data.OriginAirport}&destinationLocationCode={data.DestinationAirport}&departureDate={data.DepartureDate.Value:yyyy-MM-dd}&adults={data.NumberOfPassengers}&max=200");

            if (data.ReturnDate != null)
            {
                sb.Append($"&returnDate={data.ReturnDate.Value:yyyy-MM-dd}");
            }
            if (data.Currency != null)
            {
                sb.Append($"&currencyCode={data.Currency}");
            }
            return sb.ToString();
        }


        // Populate Flight object with deserialized json response data
        private List<Flight> SortFlights(Root root)
        {
            var flights_data = root.Data.Select(flightData => {
                var OriginFlight = flightData.Itineraries.First();
                var ReturnFlight = flightData.Itineraries.Last();

                return new Flight
                {
                    DepartureAirport = OriginFlight.Segments.First().Departure.IataCode,
                    DepartureDate = OriginFlight.Segments.First().Departure.At.ToString("yyyy-MM-dd HH:mm:ss"),
                    ArrivalAirport = ReturnFlight.Segments.First().Departure.IataCode,
                    ReturnDate = ReturnFlight.Segments.First().Departure.At.ToString("yyyy-MM-dd HH:mm:ss"),
                    DepartureConnections = OriginFlight.Segments.Count(),
                    ReturnConnections = ReturnFlight.Segments.Count(),
                    NumberOfPassengers = flightData.TravelerPricings.Count(),
                    Currency = flightData.Price.Currency,
                    Price = flightData.Price.GrandTotal
                };
            }).ToList();

            flights_data.Count();
            return flights_data;
        }
    }
}
