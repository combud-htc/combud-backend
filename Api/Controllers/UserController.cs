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

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Db _db;

        public UserController(Db db)
        {
            this._db = db;
        }

        [HttpPost]
        [Route("ChangeSettings")]
        public async Task<ActionResult<ChangeSettingsResponse>> ChangeSettings([FromBody] ChangeSettingsRequest req)
        {
            try
            {

                string country = req.Country;
                string town = req.Town;
                int radius = req.Radius;

                if (country == null || string.IsNullOrWhiteSpace(country) || country.Length != 2) return new ChangeSettingsResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Not a valid country!" };
                if (town == null || string.IsNullOrWhiteSpace(town) || town.Length > 20) return new ChangeSettingsResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Not a valid town!" };
                if (radius > 100 || radius < 0) return new ChangeSettingsResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Not a valid radius!" };

                await HttpContext.Session.LoadAsync().ConfigureAwait(false);
                if (!HttpContext.Session.Keys.Contains("Id")) return Unauthorized();

                int UserId = HttpContext.Session.GetInt32("Id").GetValueOrDefault(-1);
                if (UserId == -1) return Unauthorized();

                User user = this._db.User
                    .Where(u => u.Id == UserId)
                    .FirstOrDefault();

                user.Country = req.Country;
                user.Town = req.Town;
                user.Radius = req.Radius;

                await this._db.SaveChangesAsync().ConfigureAwait(false);

                return new ChangeSettingsResponse() { statusCode = WebTypes.StatusCode.OK };
            }
            catch
            {
                return new ChangeSettingsResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Server error!" };
            }
        }
        

        [HttpGet]
        [Route("GetUserData")]
        public async Task<ActionResult<UserDataResponse>> GetUserData()
        {
            try
            {
                await HttpContext.Session.LoadAsync().ConfigureAwait(false);
                if (!HttpContext.Session.Keys.Contains("Id")) return Unauthorized();

                int UserId = HttpContext.Session.GetInt32("Id").GetValueOrDefault(-1);
                if (UserId == -1) return Unauthorized();

                User user = this._db.User
                    .Where(u => u.Id == UserId)
                    .FirstOrDefault();

                List<ResponsePost> Posts = new List<ResponsePost>();

                foreach(Post p in user.Post)
                {
                    Posts.Add(new ResponsePost()
                    {
                        Address = p.Address,
                        Description = p.Description,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude,
                        TimeDue = p.TimeDue,
                        TimePosted = p.TimePosted,
                        Title = p.Title
                    });
                }

                return new UserDataResponse()
                {
                    statusCode = WebTypes.StatusCode.OK,
                    Country = user.Country,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Radius = user.Radius,
                    Town = user.Town,
                    Username = user.Username,
                    Posts = Posts
                };

            } catch
            {
                return new UserDataResponse()
                {
                    statusCode = WebTypes.StatusCode.ERROR,
                    errorMessage = "Server error!"
                };
            }
        }
    }
}