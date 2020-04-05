using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.WebTypes.Requests;
using Api.WebTypes.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using bcrypt = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Http;
using FormatWith;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthenticationController : ControllerBase
	{
		private readonly ILogger<AuthenticationController> _logger;
		private readonly Db _db;
		private readonly ComBudConfiguration _config;

		public AuthenticationController(ILogger<AuthenticationController> logger, Db db, ComBudConfiguration config)
		{
			this._logger = logger;
			this._db = db;
			this._config = config;
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
			try
			{
				string email = req.Email;
				string username = req.Username;
				string password = req.Password;
				string fname = req.FirstName;
				string lname = req.LastName;
				string country = req.Country;
				string town = req.Town;

				#region validation

				// Verify user submitted data
				if (email == null || string.IsNullOrWhiteSpace(email) || !Helper.IsValidEmail(email)) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Not a valid email!" };
				if (username == null || string.IsNullOrWhiteSpace(username) || username.Length > 20) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Not a valid username!" };
				if (this._db.User.Where(u => u.Email == email).Any()) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "This email is already in use!" };
				if (this._db.User.Where(u => u.Username == username).Any()) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "This username is already in use!" };
				if (fname == null || string.IsNullOrWhiteSpace(fname) || fname.Length < 2 || fname.Length > 30) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "First name is not valid!" };
				if (lname == null || string.IsNullOrWhiteSpace(lname) || lname.Length < 2 || lname.Length > 30) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Last name is not valid!" };
				if (country == null || string.IsNullOrWhiteSpace(country) || country.Length != 2) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Country is not valid!" };
				if (town == null || string.IsNullOrWhiteSpace(town) || lname.Length > 20) return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Town is not valid!" };

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
					EmailValidationToken = Helper.RandomString(50),
					Radius = 50,
					Country = req.Country,
					Town = req.Town
				};

				this._db.User.Add(user);
				_ = this._db.SaveChangesAsync().ConfigureAwait(false);


				_ = Helper.SendEmail(
					this._config.SENDER_ADDRESS,
					email,
					"Combud verification",
					"<html><head><link href=\"https://fonts.googleapis.com/css?family=Poppins:600,700&display=swap\" rel=\"stylesheet\"><style>body{font-family:'Poppins',sans-serif;text-align:center}button{padding:0.5rem 2.5rem;background:#2ecc71;font-size:1.2rem;color:#FFF;border:0;border-radius:0.5rem}button a{color:#FFF;text-decoration:none}</style></head><body><h1>ComBud</h1><h3>Verify your email.</h3> <button><a href=\"" + $"{this._config.BASE_URL}/api/Authentication/Verify?Token={user.EmailValidationToken}" + "\">Verify</a></button></body></html>",
					this._config.AWS_ID,
					this._config.AWS_KEY)
					.ConfigureAwait(false);
				

				return new RegisterResponse() { statusCode = WebTypes.StatusCode.OK };
			} catch(Exception e)
			{
				this._logger.LogError(e, "", "");
				return new RegisterResponse() { statusCode = WebTypes.StatusCode.ERROR, errorMessage = "Server error!" };

			}
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

		#region VerifyEmail
		[HttpGet]
		[Route("Verify")]
		public async Task<ActionResult> Verify([FromQuery] VerifyEmailRequest req)
		{
			try
			{
				string token = req.Token;
				User u = this._db.User.Where(user => user.EmailValidationToken == token).FirstOrDefault();

				if(u == null || u == default)
				{
					return Unauthorized();
				} else
				{
					u.ValidatedEmail = true;
					_ = this._db.SaveChangesAsync().ConfigureAwait(false);
					return Redirect("/");
				}
			} catch
			{
				return StatusCode(500, "Error");
			}

		}
		#endregion

	}
}