using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.WebTypes.Responses
{
	public class GenericResponse
	{
		public StatusCode statusCode { get; set; }
		public string errorMessage { get; set; }
	}
}
