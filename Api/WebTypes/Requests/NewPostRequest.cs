using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.WebTypes.Requests
{
	public class NewPostRequest
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Address { get; set; }
		public DateTime DueDate { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}
}