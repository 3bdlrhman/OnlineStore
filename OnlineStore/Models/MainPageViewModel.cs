using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineStore.Models
{
    public class MainPageViewModel
    {
        public ICollection<Product> section1 { get; set; }
        public ICollection<Product> section2 { get; set; }
        public ICollection<Product> section3 { get; set; }
        public ICollection<Product> BestSeller { get; set; }
    }
}