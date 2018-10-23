using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MerendaIFCE.WebApp.Models;
using MerendaIFCE.WebApp.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MerendaIFCE.WebApp.Controllers
{
    [NonController]
    [Route("Admin/Sync/Conta")]
    public class ContaSyncController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ContaSyncController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Cadastra()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastra(RegisterUsernameViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await CriaRole())
                {
                    return await CriaUsuario(model);
                }
            }
            return View(model);
        }

        private async Task<IActionResult> CriaUsuario(RegisterUsernameViewModel model)
        {
            var user = await userManager.FindByNameAsync(model.Login);
            if (user != null)
            {
                ModelState.AddModelError("Login", "Usuário já existe");
                return View(model);
            }

            user = new ApplicationUser
            {
                UserName = model.Login
            };

            var result = await userManager.CreateAsync(user, model.Senha);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Errors.FirstOrDefault()?.Description ?? "Erro desconhecido ao criar Usuário");
                return View(model);
            }

            result = await userManager.AddToRoleAsync(user, Constants.SyncRole);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Errors.FirstOrDefault()?.Description ?? "Erro desconhecido ao adicionar ao Role");
                return View(model);
            }


            return RedirectToAction("Index", "Home");
        }

        private async Task<bool> CriaRole()
        {
            var role = await roleManager.FindByNameAsync(Constants.SyncRole);
            if (role == null)
            {
                role = new IdentityRole { Name = Constants.SyncRole };
                var roleResult = await roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelError("", "Erro desconhecido");
                    return false;
                }
            }

            return true;
        }
    }
}