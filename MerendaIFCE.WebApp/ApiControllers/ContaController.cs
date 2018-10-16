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

namespace MerendaIFCE.WebApp.ApiControllers
{
    [Produces("application/json")]
    [Route("api/Conta")]
    public class ContaController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly TokenConfigurations tokenConfigurations;

        public ContaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, TokenConfigurations tokenConfigurations)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenConfigurations = tokenConfigurations;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            var user = context.Users.Include(u => u.Inscricao)
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
                        Email = user.Email,
                        Token = GetToken(user),
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
                    await signInManager.SignInAsync(user, isPersistent: false);

                    var response = new LoginResult
                    {
                        Email = user.Email,
                        Inscricao = user.Inscricao,
                        Token = GetToken(user)
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

        private string GetToken(ApplicationUser model)
        {
            var now = DateTime.UtcNow;
            ClaimsIdentity identity = new ClaimsIdentity(
                    //new GenericIdentity(login.Username, "Login"),
                    new GenericIdentity(model.Email),
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, model.Email)
                    }
                );

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = tokenConfigurations.Issuer,
                Audience = tokenConfigurations.Audience,
                Expires = now.AddHours(2),
                IssuedAt = now,
                SigningCredentials = tokenConfigurations.SigningCredentials,
                Subject = identity,
            });

            var token = handler.WriteToken(securityToken);

            return token;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}