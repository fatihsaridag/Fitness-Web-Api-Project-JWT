using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessAuthSever.Core.Entity
{
    public class UserApp : IdentityUser
    {
        public string City { get; set; }
        public string CompanyName { get; set; }
    }
}
