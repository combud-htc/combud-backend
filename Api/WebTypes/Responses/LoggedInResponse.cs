using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.WebTypes.Responses
{
	public class LoggedInResponse : GenericResponse
	{
		public bool IsLoggedIn { get; set; }
	}
}
