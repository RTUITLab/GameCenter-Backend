using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests
{
    public class AuthRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
