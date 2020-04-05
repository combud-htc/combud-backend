using System;
using System.Collections.Generic;

namespace Api.Models
{
    public partial class PostQueue
    {
        public int PostId { get; set; }
        public int HelperId { get; set; }
        public int Position { get; set; }

        public virtual User Helper { get; set; }
        public virtual Post Post { get; set; }
    }
}
