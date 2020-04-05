using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.WebTypes.Responses
{
	public class UserDataResponse : GenericResponse
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Username { get; set; }
		public string Country { get; set; }
		public string Town { get; set; }
		public int Radius { get; set; }
		public List<ResponsePost> Posts { get; set; }
	}
}
