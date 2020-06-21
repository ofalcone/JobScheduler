using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public AccountController(SignInManager<User> signInManager, IConfiguration configuration, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _configuration = configuration;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, [FromQuery(Name = "ReturnUrl")] string returnUrl)
        {
            if (!ModelState.IsValid) return View();

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid UserName or Password");

                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    if (await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_configuration["Tokens:Key"]);
                        //meta informazioni da inserire dei token jwt 
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.Name, user.Id.ToString()),
                                new Claim(ClaimTypes.Role, Constants.ADMIN_ROLE)
                            }),
                            //se voglio che i token non scadano, possono mettere un valore molto alto
                            Expires = DateTime.UtcNow.AddDays(7),
                            //algoritmo di cifratura
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        };
                        var token = tokenHandler.CreateToken(tokenDescriptor);

                        return Ok(new
                        {
                            //converto oggetto tokendescri in stringa per passarla al client
                            Token = tokenHandler.WriteToken(token),
                            Expiration = token.ValidTo,
                        });
                    }
                }
            }

            return BadRequest();
        }
    }
}