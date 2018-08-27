using BusinessObjects.Models;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.ObjectMap
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Map(x => x.LastLogin).Column("last_login");
            Map(x => x.Name).Column("name");
            Map(x => x.Password).Column("password");
            Id(x => x.Id).Column("id").GeneratedBy.Increment();
        }
    }
}
