using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.WebTypes.Requests;
using Api.WebTypes.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LocationController : ControllerBase
	{
		private readonly Db _db;
		public LocationController(Db db)
		{
			this._db = db;
		}

		[HttpPost]
		[Route("GetNearbyPosts")]
		public async Task<ActionResult<NearbyResponse>> GetNearbyWantedHelp([FromBody] NearbyRequest req)
		{
			await HttpContext.Session.LoadAsync().ConfigureAwait(false);
			if (!HttpContext.Session.Keys.Contains("Id")) return Unauthorized();
			int UserId = HttpContext.Session.GetInt32("Id").GetValueOrDefault();
			
			string country = req.Country;
			float lat = req.Latitiude;
			float lng = req.Longitude;

			Coordinate location = new Coordinate(lat, lng);

			return new NearbyResponse();
		}
	}
}