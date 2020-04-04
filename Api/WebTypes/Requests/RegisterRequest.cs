using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.WebTypes.Requests
{
	public class RegisterRequest
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public string Username { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
}
