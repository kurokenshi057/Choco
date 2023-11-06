using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace choco.models
{
    public class ArticleAchete
    {
        public Guid IdAcheteur { get; set; }
        public Guid IdArticle { get; set; }
        public int Quantite { get; set; }
        public DateTime DateAchat { get; set; }
    }

}
