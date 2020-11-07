using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastrutura;
using Infrastrutura.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AccountController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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
