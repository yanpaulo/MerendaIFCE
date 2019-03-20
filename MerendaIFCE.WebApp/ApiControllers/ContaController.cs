using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using MerendaIFCE.WebApp.Models;
using MerendaIFCE.WebApp.Models.AccountViewModels;
using MerendaIFCE.WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MerendaIFCE.WebApp.ApiControllers
{
    [AllowAnonymous]
    [Route("api/Conta")]
    [Produces("application/json")]
    public class ContaController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly TokenService tokenService;

        public ContaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, TokenService tokenService)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginViewModel model)
        {
            var user = context.Users.Include(u => u.Inscricao).ThenInclude(i => i.Dias)
                .SingleOrDefault(u => u.UserName == model.UserName || u.Inscricao.Matricula == model.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "Não existe usuário com a matrícula ou email especificados.");
            }

            if (user != null)
            {
                var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (signInResult.Succeeded)
                {
                    var result = new LoginResult
                    {
                        Login = user.UserName,
                        Token = await tokenService.GetToken<ApplicationUser>(user, userManager),
                        Inscricao = user.Inscricao
                    };
                    return Ok(result);
                }
                else
                {
                    ModelState.AddModelError("", "Usuário ou Senha inválidos");
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost("Cadastro")]
        public async Task<IActionResult> PostAsync([FromBody]RegisterViewModel model)
        {
            Valida(model);
            await CriaRoleAsync();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Inscricao = new Inscricao
                    {
                        Matricula = model.Matricula
                    }
                };
                var result = await userManager.CreateAsync(user, model.Senha);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Constants.UserRole);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    var response = new LoginResult
                    {
                        Login = user.Email,
                        Inscricao = user.Inscricao,
                        Token = await tokenService.GetToken<ApplicationUser>(user, userManager)
                    };

                    return Ok(response);
                }

                AddErrors(result);
            }


            return BadRequest(ModelState);

        }

        private void Valida(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existsUserName = context.Users.Any(u => u.UserName == model.Email || u.Inscricao.Matricula == model.Matricula);
                if (existsUserName)
                {
                    ModelState.AddModelError("Email", "Email já cadastrado");
                }
                var existsMatricula = context.Users.Any(u => u.Inscricao.Matricula == model.Matricula);
                if (existsMatricula)
                {
                    ModelState.AddModelError("Matricula", "Matrícula já cadastrada");
                }
            }
        }
        
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task CriaRoleAsync()
        {
            var role = await roleManager.FindByNameAsync(Constants.UserRole);
            if (role == null)
            {
                role = new IdentityRole { Name = Constants.UserRole };
                var roleResult = await roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelError("", $"Erro ao criar Role {Constants.UserRole}");
                }
            }
        }
    }
}