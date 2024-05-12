using System.Diagnostics;
using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mvc.Data;
using Mvc.Models;
using Mvc.Models.Entity;
using Mvc.Service;

namespace Mvc.Controllers;

public class HomeController : Controller
{
    private readonly IEnvioService _envioService;
    private readonly IUsuarioService _usuarioService;
    private readonly DatabaseContext _context;
    private readonly UserManager<Usuario> _userManager;
    private readonly INotyfService _notifyService;


    public HomeController(
        IEnvioService envioService,
          IUsuarioService usuarioService,
         UserManager<Usuario> userManager,
        DatabaseContext context,
        INotyfService notifyService)
    {
        _envioService = envioService;
        _usuarioService = usuarioService;
        _context = context;
        _userManager = userManager;
        _notifyService = notifyService;
    }


    [HttpGet]
    public IActionResult Consultar()
    {

        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var envios = _envioService.GetByUsuarioId(usuarioId);

        return View(envios);
    }
    //metodo para crear 
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Envio envio)
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            using (var dbContext = _context)
            {
                var nuevoEnvio = new Envio
                {
                    Codigo = envio.Codigo,
                    Title = envio.Title,
                    Telefono = envio.Telefono,
                    Description = envio.Description,
                    UsuarioId = currentUser!.Id,
                };

                _context.Envios.Add(nuevoEnvio);
                await _context.SaveChangesAsync();

                _notifyService.Success("Envio creado correctamente");

                return RedirectToAction("Consultar");
            }
        }
        catch (Exception ex)
        {
            _notifyService.Error($"Error al crear el envío: {ex.Message}");
            return RedirectToAction("Consultar");
        }
    }

    // Metodo para eliminar
    [HttpGet]
    public IActionResult Delete(int id)
    {
        var eventToDelete = _envioService.GetById(id);
        _envioService.Delete(id);
        _notifyService.Success("Envío eliminado exitosamente.");
        return RedirectToAction("Consultar");
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
