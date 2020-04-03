using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.WebTypes.Requests;
using Api.WebTypes.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using bcrypt = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Http;

namespace Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AuthenticationController : ControllerBase
	{
		private readonly ILogger<AuthenticationController> _logger;
		private readonly Db _db;

		public AuthenticationController(ILogger<AuthenticationController> logger, Db db)
		{
			this._logger = logger;
			this._db = db;
		}

		//bcrypt.HashPassword("", bcrypt.GenerateSalt(12), true, BCrypt.Net.HashType.SHA512);
		#region Login
		[HttpPost]
		[Route("Login")]
		public async Task<LoginResponse> Login([FromBody] LoginRequest req)
		{
			string email = req.Email;
			string password = req.Password;

			User user = this._db.users.Where(u => u.Email == email).FirstOrDefault();

			if (user == null || user == default)
				return new LoginResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "The email entered does not have an account registered to it!" };

			if (!user.ValidatedEmail)
				return new LoginResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "The account email has not been verified!" };

			string hashedPassword = user.HashedPassword;
			bool correctPassword = bcrypt.Verify(password, hashedPassword, true, BCrypt.Net.HashType.SHA512);

			if (correctPassword)
			{
				HttpContext.Session.SetInt32("Id", user.Id);
				await HttpContext.Session.CommitAsync().ConfigureAwait(false);
				return new LoginResponse() { statusCode = WebTypes.StatusCode.OK };
			}
			else return new LoginResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Invalid Password!" };
		}
		#endregion

		#region Register
		[HttpPost]
		[Route("Regiser")]
		public RegisterResponse Register([FromBody] RegisterRequest req)
		{
			string email = req.Email;
			string username = req.Username;

			if (this._db.users.Where(u => u.Email == email).Any()) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "This email is already in use!" };
			if (this._db.users.Where(u => u.Username == username).Any()) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "This username is already in use!" };


			return new RegisterResponse();
		}
		#endregion
	}
}
