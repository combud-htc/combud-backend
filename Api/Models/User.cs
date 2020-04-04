using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SqlServer.Types;
using NetTopologySuite.Geometries;

namespace Api.Models
{
	[Table("User", Schema = "dbo")]
	public class User
	{
		[Key]
		
		public int Id { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Username { get; set; }
		public string HashedPassword { get; set; }
		public bool ValidatedEmail { get; set; }
		public string EmailValidationToken { get; set; }
		public string Address { get; set; }
	}
}
