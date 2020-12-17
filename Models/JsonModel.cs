using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CheapFlightsApp.Models
{
    public class JsonModel
    {
        public class Links
        {
            [JsonPropertyName("self")]
            public string Self { get; set; }
        }

        public class Meta
        {
            public Links links { get; set; }
        }

        public class Departure
        {
            [JsonPropertyName("iataCode")]
            public string IataCode { get; set; }

            [JsonPropertyName("DepartureTime")]
            public DateTime At { get; set; }
        }

        public class Arrival
        {
            [JsonPropertyName("iataCode")]
            public string IataCode { get; set; }

            [JsonPropertyName("ArrivalTime")]
            public DateTime At { get; set; }
        }

        public class FlightStopover
        {
            [JsonPropertyName("Departure")]
            public Departure Departure { get; set; }

            [JsonPropertyName("Arrival")]
            public Arrival Arrival { get; set; }
        }

        public class FlightRoute
        {
            [JsonPropertyName("FlightStopovers")]
            public List<FlightStopover> Segments { get; set; }
        }

        public class Price
        {
            [JsonPropertyName("Currency")]
            public string Currency { get; set; }

            [JsonPropertyName("PriceTotal")]
            public string GrandTotal { get; set; }
        }

        public class Traveler
        {
            public int TravelerId { get; set; }
        }

        public class Data
        {
            [JsonPropertyName("FlightRoutes")]
            public List<FlightRoute> Itineraries { get; set; }

            [JsonPropertyName("NumberOfPassengers")]
            public int NumberOfPassengers { get; set; }

            [JsonPropertyName("Price")]
            public Price Price { get; set; }

            [JsonPropertyName("Travelers")]
            public List<Traveler> TravelerPricings { get; set; }
        }


        public class Root
        {
            [JsonPropertyName("Meta")]
            public Meta Meta { get; set; }

            [JsonPropertyName("Data")]
            public List<Data> Data { get; set; }
        }
    }
}
