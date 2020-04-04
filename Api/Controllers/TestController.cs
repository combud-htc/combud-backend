using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Api.Models;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TestController : ControllerBase
	{
		private Db _db;

		public TestController(Db db) {
			this._db = db;
		}

		[HttpGet]
		[Route("Test")]
		public string Test() => "Test";
	}
}
