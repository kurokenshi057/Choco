using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace choco.models
{
    public class Admin
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }

}
