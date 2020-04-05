using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.WebTypes
{
	public class ResponsePost
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime TimePosted { get; set; }
		public DateTime TimeDue { get; set; }
		public string Address { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public string Username { get; set; }
	}
}
