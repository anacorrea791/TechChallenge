using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechChallenge.Models;

namespace TechChallenge.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager,RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    // =====================================
    // GET: /Account/Register
    // Exibe formulário de cadastro
    // =====================================
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // =====================================
    // POST: /Account/Register
    // Cria novo usuário
    // =====================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model){
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        // Verifica se email já existe
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("","Este email já está cadastrado.");
            return View(model);
        }

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Senha);

        if (result.Succeeded)
        {
            // Garante que a Role existe
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            }
            // Adiciona Role ao usuário
            await _userManager.AddToRoleAsync(user,model.Role);
            // Login automático após cadastro
            await _signInManager.SignInAsync(user,isPersistent: false);

            return RedirectToAction("Index","Home");
        }

        foreach(var error in result.Errors)
        {
            ModelState.AddModelError(
                "",
                error.Description
            );
        }

        return View(model);
    }




    // =====================================
    // GET: /Account/Login
    // Exibe formulário login
    // =====================================
    [HttpGet]
    public IActionResult Login(
        string? returnUrl = null)
    {
        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };
        return View(model);
    }




    // =====================================
    // POST: /Account/Login
    // Autentica usuário
    // =====================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model)
    {

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager
            .PasswordSignInAsync(model.Email,model.Senha,model.Lembrar,
                lockoutOnFailure: false);
            
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            return RedirectToAction("Index","Home");
        }



        ModelState.AddModelError(
            "",
            "Email ou senha inválidos."
        );



        return View(model);
    }

    // =====================================
    // POST: /Account/Logout
    // Finaliza sessão
    // =====================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {

        await _signInManager.SignOutAsync();


        return RedirectToAction("Index","Tech");
    }
    // =====================================
    // GET: /Account/AccessDenied
    // Usuário sem permissão
    // =====================================
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}