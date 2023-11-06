using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace choco.models
{
    public class Acheteur
    {
        public Guid Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Adresse { get; set; }
        public int Telephone { get; set; }
    }

}
