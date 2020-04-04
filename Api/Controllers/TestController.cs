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
		[Route("GetPosts")]
		public async Task<Post> GetPosts()
		{
			//await HttpContext.Session.LoadAsync().ConfigureAwait(false);
			Post p = this._db.User.Where(u => u.Id == 2).FirstOrDefault().Post.FirstOrDefault();
			p.OwnerNavigation = null;
			return this._db.User.Where(u => u.Id == 2).FirstOrDefault().Post.FirstOrDefault();
		}

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
