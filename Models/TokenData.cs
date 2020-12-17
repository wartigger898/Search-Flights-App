using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheapFlightsApp.Models
{
    public class TokenData
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}
