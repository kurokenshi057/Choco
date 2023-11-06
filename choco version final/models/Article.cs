using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace choco.models
{
    public class Article
    {
        public Guid Id { get; set; }
        public string Reference { get; set; }
        public float Prix { get; set; }
    }

}
