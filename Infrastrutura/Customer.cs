using System;
using System.Collections.Generic;

namespace Infrastrutura
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string NIF { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PassHash { get; set; }
        public bool Logged { get; set; } = false;
        public ICollection<CustomerRole> CustomerRoles { get; set; }=new List<CustomerRole>();
    }
}
