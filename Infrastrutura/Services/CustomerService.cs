using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Infrastrutura.Services
{
    public class CustomerService : ICustomerService
    {
        private string _user = "anonimo";

        private string _connection =
            "Data Source=ea-sql.eastus2.cloudapp.azure.com;Initial Catalog=ECommerceDB;User ID=sa;Password=Pa55w.rd";
        public CustomerService(IHttpContextAccessor context)
        {
            if (context!=null)
            {
                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    _user = context.HttpContext.User.Identity.Name;
                }
            }
        }
        public int AddCustomerRole(Guid id, string roleName)
        {
            throw new NotImplementedException();
        }

        public int DelCustomerRole(Guid id, string roleName)
        {
            throw new NotImplementedException();
        }

        public int DeleteCustomer(Guid id)
        {
            using (IDbConnection db = new SqlConnection(_connection))
            {
                return db.Execute(" DELETE FROM Customers WHERE CustomerId=@id", new {id=id});
            }
        }

        public int RegisterNewCustomer(Customer item)
        {
            //  item.CustomerId=Guid.NewGuid();
            string txt = "SELECT * FROM Customers WHERE UserName =@UserName OR NIF=@NIF OR Email=@Email";
            using (IDbConnection db = new SqlConnection(_connection))
            {
                Customer r = db.QueryFirstOrDefault<Customer>(txt, item);
                if (r == null)
                {
                    txt = "INSERT INTO Customers (CustomerId, Name, Email, NIF, UserName, PassHash, Country, Logged) VALUES (@CustomerId, @Name, @Email, @NIF, @UserName, @PassHash, @Country, @Logged)";
                    item.PassHash = CodificaPassword(item.PassHash);
                    return db.Execute(txt, item);   //  number of objects added
                }
            }
            return -1;
        }

        public Customer SelectCustomer(Guid id)
        {
            using (IDbConnection db = new SqlConnection(_connection))
            {
                return db.QueryFirstOrDefault<Customer>("SELECT * FROM Customers WHERE CustomerId=@id", new {id = id});
            }
        }

        private string CodificaPassword(string password)    //  nao usar MD5
        {
            //  AZURE KEY VAULT
            var saltBytes = Encoding.UTF8.GetBytes("Saudade");
            string result="";
            var hashBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested:256/8
                );
            result = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            return result;
        }
        public Customer SelectCustomer(string userName, string password)
        {
            using (IDbConnection db = new SqlConnection(_connection))
            {
                var c= db.QueryFirstOrDefault<Customer>("SELECT * FROM Customers WHERE UserName=@UserName AND PassHash=@PassHash", 
                    new { UserName = userName, PassHash=CodificaPassword(password) });
                c.CustomerRoles.Add(new CustomerRole(){RoleName = "Guest"});
                return c;
            }
        }

        public ICollection<Customer> SelectCustomers()
        {
            throw new NotImplementedException();
        }

        public int UpdateCustomer(Customer item)
        {
            throw new NotImplementedException();
        }
    }
}
