using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastrutura.Services
{
    public interface ICustomerService
    {
        int RegisterNewCustomer(Customer item);
        ICollection<Customer> SelectCustomers();
        Customer SelectCustomer(Guid id);
        Customer SelectCustomer();  //  DI Parameter
        Customer SelectCustomer(string userName, string password);
        int DeleteCustomer(Guid id);
        int UpdateCustomer(Customer item);
        int AddCustomerRole(Guid id, string roleName);
        int DelCustomerRole(Guid id, string roleName);
    }
}
