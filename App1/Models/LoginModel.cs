using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App1.Models
{
    public class LoginModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string ErrorMessage { get; set; }
    }
}
