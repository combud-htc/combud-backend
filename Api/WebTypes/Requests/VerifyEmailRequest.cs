using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.WebTypes.Requests
{
	public class VerifyEmailRequest
	{
		public string Token { get; set; }
	}
}
