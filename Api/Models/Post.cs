using System;
using System.Collections.Generic;

namespace Api.Models
{
    public partial class Post
    {
        public int Id { get; set; }
        public int Owner { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public DateTime TimePosted { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime TimeDue { get; set; }

        public virtual User OwnerNavigation { get; set; }
    }
}
