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
using Microsoft.SqlServer.Types;
using NetTopologySuite.Geometries;
using System.Data.Entity.Spatial;
using System.Net.Mail;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthenticationController : ControllerBase
	{
		private readonly ILogger<AuthenticationController> _logger;
		private readonly Db _db;

		public AuthenticationController(ILogger<AuthenticationController> logger, Db db)
		{
			this._logger = logger;
			this._db = db;
		}
		
		#region Routes
		#region Login
		[HttpPost]
		[Route("Login")]
		public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
		{
			string email = req.Email;
			string password = req.Password;

			User user = this._db.User.Where(u => u.Email == email).FirstOrDefault();

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
		[Route("Register")]
		public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest req)
		{
			string email = req.Email;
			string username = req.Username;
			string password = req.Password;
			string fname = req.FirstName;
			string lname = req.LastName;

			#region validation
			// Verify user submitted data
			if (email == null || string.IsNullOrWhiteSpace(email) ||  !IsValidEmail(email)) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Not a valid email!" };
			if (username == null || string.IsNullOrWhiteSpace(username) || username.Length < 16) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Not a valid username!" };
			if (this._db.User.Where(u => u.Email == email).Any()) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "This email is already in use!" };
			if (this._db.User.Where(u => u.Username == username).Any()) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "This username is already in use!" };
			if (fname == null || string.IsNullOrWhiteSpace(fname) || fname.Length < 2 || fname.Length > 30) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "First name is not valid!" };
			if (lname == null || string.IsNullOrWhiteSpace(lname) || lname.Length < 2 || lname.Length > 30) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Last name is not valid!" };

			#endregion

			User user = new User()
			{
				Email = email,
				HashedPassword =
					bcrypt.HashPassword(
						password,
						bcrypt.GenerateSalt(12), true,
						BCrypt.Net.HashType.SHA512
				),
				FirstName = fname,
				LastName = lname,
				Username = username,
				ValidatedEmail = false,
				EmailValidationToken = RandomString(50),
			};

			this._db.User.Add(user);
			await this._db.SaveChangesAsync().ConfigureAwait(false);


			// SEND VERIFICATION EMAIL USING AWS SES API!
			return new RegisterResponse() { statusCode = WebTypes.StatusCode.OK };
		}
		#endregion
		#region IsLoggedIn
		[HttpGet]
		[Route("IsLoggedIn")]
		public async Task<ActionResult<LoggedInResponse>> IsLoggedIn()
		{
			try
			{
				await HttpContext.Session.LoadAsync().ConfigureAwait(false);

				if (HttpContext.Session.Keys.Contains("Id")) {
					User u = this._db.User.Where(user => user.Id == HttpContext.Session.GetInt32("Id")).FirstOrDefault();
					return new LoggedInResponse() { statusCode = WebTypes.StatusCode.OK, IsLoggedIn = true, Username =  u.Username};
				}

				return new LoggedInResponse() { statusCode = WebTypes.StatusCode.OK, IsLoggedIn = false };
			}
			catch
			{
				return new LoggedInResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Server error!" };
			}
		}
		#endregion
		#region Logout

		[HttpGet]
		[Route("Logout")]
		public async Task<ActionResult> Logout()
		{
			try
			{
				await HttpContext.Session.LoadAsync().ConfigureAwait(false);
				HttpContext.Session.Clear();
				await HttpContext.Session.CommitAsync().ConfigureAwait(false);
				return Redirect("/");
			} catch
			{
				return Redirect("/logouterror");
			}
		}
		#endregion
		#endregion

		#region verifyemail
		bool IsValidEmail(string email)
		{
			try
			{
				MailAddress addr = new MailAddress(email);
				return addr.Address == email && email.Length <= 50;
			}
			catch
			{
				return false;
			}
		}
#endregion
		#region EmailRandomTokenGeneration
		public string RandomString(int length)
		{
			Random random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}
		#endregion

	}
}