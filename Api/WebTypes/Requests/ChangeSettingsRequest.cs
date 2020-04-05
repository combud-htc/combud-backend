using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.WebTypes.Requests
{
	public class ChangeSettingsRequest
	{
		public int Radius { get; set; }
		public string Country { get; set; }
		public string Town { get; set; }
	}
}
