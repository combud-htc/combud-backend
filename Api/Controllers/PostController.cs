using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.WebTypes;
using Api.WebTypes.Requests;
using Api.WebTypes.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PostController : ControllerBase
	{
		private readonly Db _db;
		private readonly ILogger<PostController> _logger;

		public PostController(Db db, ILogger<PostController> logger)
		{
			this._db = db;
			this._logger = logger;
		}

		[HttpPost]
		[Route("NewPost")]
		public async Task<ActionResult<NewPostResponse>> NewPost([FromBody] NewPostRequest req)
		{

			try
			{
				await HttpContext.Session.LoadAsync().ConfigureAwait(false);
				if (!HttpContext.Session.Keys.Contains("Id")) return Unauthorized();

				User user = this._db.User.Where(u => u.Id == HttpContext.Session.GetInt32("Id")).FirstOrDefault();
				user.Post.Add(new Post() {
					Address = req.Address,
					Description = req.Description,
					Latitude = req.Latitude,
					Longitude = req.Longitude,
					TimePosted = DateTime.UtcNow,
					Title = req.Title,
					TimeDue = req.DueDate
				});
				
				await this._db.SaveChangesAsync().ConfigureAwait(false);
				return new NewPostResponse() { statusCode = WebTypes.StatusCode.OK, errorMessage = "Posted succesfully!" };
			} catch(Exception e)
			{
				this._logger.LogError(e, "Fucky wuckie happened!", null);
				return new NewPostResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Server error!" };
			}
		}

		[HttpGet]
		[Route("JoinQueue")]
		public async Task<ActionResult> JoinQueue() {

			return Ok();
		}

		[HttpPost]
		[Route("GetNearbyPosts")]
		public async Task<ActionResult<NearbyResponse>> GetNearbyWantedHelp([FromBody] NearbyRequest req)
		{
			try
			{
				await HttpContext.Session.LoadAsync().ConfigureAwait(false);
				if (!HttpContext.Session.Keys.Contains("Id")) return Unauthorized();

				User user = this._db.User
					.Where(u => u.Id == HttpContext.Session.GetInt32("Id"))
					.FirstOrDefault();

				float lat = req.Latitiude;
				float lng = req.Longitude;

				IEnumerable<Post> posts = this._db.Post.AsEnumerable().Where(p => Helper.IsClose(p, lat, lng, user.Radius));

				NearbyResponse res = new NearbyResponse();
				res.posts = new List<ResponsePost>();

				foreach (Post p in posts)
				{
					res.posts.Add(new ResponsePost()
					{
						Address = p.Address,
						Description = p.Description,
						FirstName = p.OwnerNavigation.FirstName,
						LastName = p.OwnerNavigation.LastName,
						Latitude = p.Latitude,
						Longitude = p.Longitude,
						TimePosted = p.TimePosted,
						Title = p.Title,
						TimeDue = p.TimeDue,
						Username = p.OwnerNavigation.Username
					});
				}

				res.statusCode = WebTypes.StatusCode.OK;
				return res;
			} catch(Exception e)
			{
				this._logger.LogError(e, "Fucky wuckie happened!", null);
				return new NearbyResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Server side error!" };
			}
		}
	}
}