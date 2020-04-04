using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.WebTypes.Requests
{
	public class NearbyRequest
	{
		public string Country { get; set; }
		public float Latitiude { get; set; }
		public float Longitude { get; set; }
	}
}
