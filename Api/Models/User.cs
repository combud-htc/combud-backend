using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SqlServer.Types;
namespace Api.Models
{
	public class User
	{
		[Key]
		public int Id { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string Username { get; set; }
		public string HashedPassword { get; set; }
		public bool ValidatedEmail { get; set; }
		public string Address { get; set; }
		public SqlGeography Location { get; set; }

	}
}
