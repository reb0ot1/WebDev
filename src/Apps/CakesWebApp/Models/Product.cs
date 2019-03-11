using System;
using System.Collections.Generic;
using System.Text;

namespace CakesWebApp.Models
{
    public class Product : BaseModel<int>
    {
        public Product()
        {
            this.Orders = new HashSet<OrderProduct>();
        }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Url { get; set; }

        public virtual ICollection<OrderProduct> Orders { get; set; }
    }
}
