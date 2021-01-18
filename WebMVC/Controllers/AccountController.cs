using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastrutura;
using Infrastrutura.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using WebMVC.Models.CustomerCommand;

namespace WebMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICustomerService _db;

        public AccountController(ICustomerService db)
        {
            _db = db;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminPage()
        {
            return View();
        }
        [Route("/logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            return Redirect("/");
        }
        // GET: AccountController
        [Route("/login")]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [Route("/login")]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            var r = _db.SelectCustomer(model.UserName, model.Password);
            if (r != null)  //  Sessao scoped
            {
                //  Criar claim e registrar HttpContext
                var identity=new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Country,r.Country));
                identity.AddClaim(new Claim("Theme", r.Country));
                foreach (var cr in r.CustomerRoles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role,cr.RoleName));
                }
                DateTime dt=DateTime.UtcNow.AddSeconds(10);
                var principal=new ClaimsPrincipal(identity);
                var properties=new AuthenticationProperties(){IsPersistent = model.RememberMe};
                await this.HttpContext.SignInAsync(principal, properties).ConfigureAwait(false);
                return Redirect("/");
            }
            ModelState.AddModelError("", "Credenciais invalidas");
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            var r = _db.RegisterNewCustomer(new Customer(){CustomerId = Guid.NewGuid(), Name = model.Name, NIF = model.NIF, UserName = model.UserName, Country = model.Country, PassHash = model.Password});
            if (r == 1)
            {
                return Redirect("/");
            }
            ModelState.AddModelError("","Nao foi possivel criar um registo");
            return View(model);
        }
        // GET: AccountController/Details/5
        [Route("/details")]
        public ActionResult Details(Guid id)
        {
            return View(_db.SelectCustomer());
        }

        public IActionResult Index()
        {
            if (HttpContext.User.IsInRole("Admin"))
            {
                return Redirect("Admin");
            }
            _db.SelectCustomer();
            return Redirect("Details");
        }

        // GET: AccountController/Create
        public ActionResult AddRole()
        {
            return View();
        }

        // POST: AccountController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRole(CustomerRole item)
        {
            try
            {
                _db.AddCustomerRole(Guid.Empty, item.RoleName);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(item);
            }
        }

        // GET: AccountController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AccountController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
