using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TestController : ControllerBase
	{
		[HttpGet]
		[Route("Set")]
		public async Task<string> Set()
		{
			HttpContext.Session.SetString("uwu", "owo");
			await HttpContext.Session.CommitAsync().ConfigureAwait(false);
			return "OWO";
		}

		[HttpGet]
		[Route("Get")]
		public string Get()
		{
			return HttpContext.Session.GetString("uwu");
		}

		[HttpGet]
		[Route("Test")]
		public string Test() => "Test";
	}
}
