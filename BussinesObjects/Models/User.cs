using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObjects.Models
{
    public class User
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Password { get; set; }
        public virtual DateTime LastLogin { get; set; }
    }
}
