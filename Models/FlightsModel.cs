using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CheapFlightsApp.Models
{
    public class SearchParameters
    {
        public string OriginAirport { get; set; }

        public string DestinationAirport { get; set; }

        public DateTime? DepartureDate { get; set; }

        public int NumberOfPassengers { get; set; }

        public DateTime?  ReturnDate { get; set; }

        public string Currency { get; set; }
    }

    public class Flight
    {
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public string DepartureDate { get; set; }
        public string ReturnDate { get; set; }
        public int DepartureConnections { get; set; }
        public int ReturnConnections { get; set; }
        public int NumberOfPassengers { get; set; }
        public string Currency { get; set; }
        public string Price { get; set; }
    }
}
