using System;
using System.Collections.Generic;

namespace Api.Models
{
    public partial class User
    {
        public User()
        {
            Post = new HashSet<Post>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public bool ValidatedEmail { get; set; }
        public string EmailValidationToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Town { get; set; }
        public int Radius { get; set; }

        public virtual ICollection<Post> Post { get; set; }
    }
}
