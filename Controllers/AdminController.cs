using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Gerenciar usuarios
    public async Task<IActionResult> Usuarios()
    {
        var usuarios = _userManager.Users.ToList();
        var rolesPorUsuario = new Dictionary<string,string>();

        foreach(var usuario in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(usuario); 
            var role = roles.FirstOrDefault() ?? "Sem Perfil";
            rolesPorUsuario.Add(usuario.Id, role);
        }
        ViewBag.RolesUsuarios = rolesPorUsuario;
        return View(usuarios);
    }

    // Gerenciar roles
    public async Task<IActionResult> Roles()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    public async Task<IActionResult> ExcluirUsuario(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario != null)
        {
            var resultado = await _userManager.DeleteAsync(usuario);
            if (resultado.Succeeded)
            {
                return RedirectToAction("Usuarios");
            }
            else
            {
                ModelState.AddModelError("","Erro ao Excluir o usuário.");
            }
        }
        else
        {
            ModelState.AddModelError("","Usuário não encontrado.");
        }
        return RedirectToAction("Usuarios");
    }

    // Criar Role - GET
[HttpGet]
public IActionResult CriarRole()
{
    return View();
}

// Criar Role - POST
[HttpPost]
public async Task<IActionResult> CriarRole(string nome)
{
    if (string.IsNullOrWhiteSpace(nome))
    {
        ModelState.AddModelError("", "Digite o nome da Role.");
        return View();
    }

    if (await _roleManager.RoleExistsAsync(nome))
    {
        ModelState.AddModelError("", "Essa Role já existe.");
        return View();
    }

    var resultado = await _roleManager.CreateAsync(
        new IdentityRole(nome)
    );

    if (resultado.Succeeded)
    {
        return RedirectToAction("Roles");
    }

    foreach (var erro in resultado.Errors)
    {
        ModelState.AddModelError("", erro.Description);
    }

    return View();
}

     public async Task<IActionResult> TornarAdmin(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario != null)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            await _userManager.RemoveFromRolesAsync(usuario,roles);
            await _userManager.AddToRoleAsync(usuario,"Admin");
        }
        return RedirectToAction("Usuarios");
    }

    //Trocar perfil GET
    public async Task<IActionResult> TrocarPerfil(string id)
    {
        var rolesDisponiveis = _roleManager.Roles.ToList();
        var usuario = await _userManager.FindByIdAsync(id);
        ViewBag.RolesDisponiveis = rolesDisponiveis;
        return View(usuario);
    }
    //Trocar perfil POST
[HttpPost]
public async Task<IActionResult> TrocarPerfil(string id, string role)
{
    var usuario = await _userManager.FindByIdAsync(id);

    if (usuario == null)
    {
        return RedirectToAction("Usuarios");
    }

    var rolesAtuais = await _userManager.GetRolesAsync(usuario);

    if (rolesAtuais.Any())
    {
        await _userManager.RemoveFromRolesAsync(usuario, rolesAtuais);
    }

    if (!string.IsNullOrEmpty(role))
    {
        await _userManager.AddToRoleAsync(usuario, role);
    }

    return RedirectToAction("Usuarios");
}

// Criar usuário - GET
[HttpGet]
public IActionResult CriarUsuario()
{
    var roles = _roleManager.Roles.ToList();
    ViewBag.RolesDisponiveis = roles;

    return View();
}

// Criar usuário - POST
[HttpPost]
public async Task<IActionResult> CriarUsuario(
    string nome,
    string email,
    string senha,
    string role)
{
    if (string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrWhiteSpace(senha))
    {
        ModelState.AddModelError("", "E-mail e senha são obrigatórios.");

        ViewBag.RolesDisponiveis = _roleManager.Roles.ToList();

        return View();
    }

    var usuarioExistente = await _userManager.FindByEmailAsync(email);

    if (usuarioExistente != null)
    {
        ModelState.AddModelError("", "Este e-mail já está cadastrado.");

        ViewBag.RolesDisponiveis = _roleManager.Roles.ToList();

        return View();
    }

    var usuario = new IdentityUser
    {
        UserName = email,
        Email = email
    };

    var resultado = await _userManager.CreateAsync(usuario, senha);

    if (resultado.Succeeded)
    {
        if (!string.IsNullOrEmpty(role))
        {
            await _userManager.AddToRoleAsync(usuario, role);
        }

        return RedirectToAction("Usuarios");
    }

    foreach (var erro in resultado.Errors)
    {
        ModelState.AddModelError("", erro.Description);
    }

    ViewBag.RolesDisponiveis = _roleManager.Roles.ToList();

    return View();
}

// Editar usuário - GET
[HttpGet]
public async Task<IActionResult> EditarUsuario(string id)
{
    var usuario = await _userManager.FindByIdAsync(id);

    if (usuario == null)
    {
        return RedirectToAction("Usuarios");
    }

    return View(usuario);
}

// Editar usuário - POST
[HttpPost]
public async Task<IActionResult> EditarUsuario(string id, string email)
{
    var usuario = await _userManager.FindByIdAsync(id);

    if (usuario == null)
    {
        return RedirectToAction("Usuarios");
    }

    if (string.IsNullOrWhiteSpace(email))
    {
        ModelState.AddModelError("", "O e-mail é obrigatório.");
        return View(usuario);
    }

    usuario.Email = email;
    usuario.UserName = email;

    var resultado = await _userManager.UpdateAsync(usuario);

    if (resultado.Succeeded)
    {
        return RedirectToAction("Usuarios");
    }

    foreach (var erro in resultado.Errors)
    {
        ModelState.AddModelError("", erro.Description);
    }

    return View(usuario);
}

// Editar Role - GET
[HttpGet]
public async Task<IActionResult> EditarRole(string id)
{
    var role = await _roleManager.FindByIdAsync(id);

    if (role == null)
    {
        return RedirectToAction("Roles");
    }

    return View(role);
}

// Editar Role - POST
[HttpPost]
public async Task<IActionResult> EditarRole(string id, string nome)
{
    var role = await _roleManager.FindByIdAsync(id);

    if (role == null)
    {
        return RedirectToAction("Roles");
    }

    if (string.IsNullOrWhiteSpace(nome))
    {
        ModelState.AddModelError("", "O nome da Role é obrigatório.");
        return View(role);
    }

    if (await _roleManager.RoleExistsAsync(nome) &&
        role.Name != nome)
    {
        ModelState.AddModelError("", "Essa Role já existe.");
        return View(role);
    }

    role.Name = nome;
    role.NormalizedName = nome.ToUpper();

    var resultado = await _roleManager.UpdateAsync(role);

    if (resultado.Succeeded)
    {
        return RedirectToAction("Roles");
    }

    foreach (var erro in resultado.Errors)
    {
        ModelState.AddModelError("", erro.Description);
    }

    return View(role);
}
 
// Excluir Role
[HttpGet]
public async Task<IActionResult> ExcluirRole(string id)
{
    var role = await _roleManager.FindByIdAsync(id);

    if (role == null)
    {
        return RedirectToAction("Roles");
    }

    var usuarios = await _userManager.GetUsersInRoleAsync(role.Name);

    if (usuarios.Any())
    {
        TempData["Erro"] = "Não é possível excluir esta Role porque existem usuários associados a ela.";
        return RedirectToAction("Roles");
    }

    var resultado = await _roleManager.DeleteAsync(role);

    if (!resultado.Succeeded)
    {
        TempData["Erro"] = "Não foi possível excluir a Role.";
    }

    return RedirectToAction("Roles");
}
}
