using System.Diagnostics;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mvc.Data;
using Mvc.Models;
using Mvc.Models.Entity;
using Mvc.Service;

namespace Mvc.Controllers;

public class HomeController : Controller
{
    private readonly IEnvioService _envioService;
    private readonly DatabaseContext _context;
    private readonly UserManager<Usuario> _userManager;
    private readonly INotyfService _notifyService;


    public HomeController(
        IEnvioService envioService,
         UserManager<Usuario> userManager,
        DatabaseContext context,
        INotyfService notifyService)
    {
        _envioService = envioService;
        _context = context;
        _userManager = userManager;
        _notifyService = notifyService;
    }

    public IActionResult Index()
    {
        var envios = _envioService.GetAll();
        return View(envios);
    }

    [HttpGet]
    public async Task<IActionResult> Consultar(string numeroCedula)
    {
        if (!string.IsNullOrEmpty(numeroCedula))
        {
            var usuario = await _userManager.FindByEmailAsync(numeroCedula);

            if (usuario != null)
            {
                // Filtrar los envíos por el número de identificación del usuario encontrado
                var enviosUsuario = _envioService.GetAll().Where(e => e.Usuario.NumeroIdentificacion == usuario.NumeroIdentificacion);
                return View(enviosUsuario);
            }
        }

        // Si no se encuentra el usuario o no se proporciona un número de cédula válido, mostrar un mensaje o redirigir a otra página
        return View(Enumerable.Empty<Envio>());
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
            var nuevoEnvio = new Envio
            {
                Codigo = envio.Codigo,
                Title = envio.Title,
                Telefono = envio.Telefono,
                Description = envio.Description,
            };

            _context.Envios.Add(nuevoEnvio);
            await _context.SaveChangesAsync();

            _notifyService.Success("Envio creado correctamente");

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _notifyService.Error($"Error al crear el envío: {ex.Message}");
            return RedirectToAction("Index");
        }
    }

    // Metodo para eliminar
    [HttpGet]
    public IActionResult Delete(int id)
    {
        var eventToDelete = _envioService.GetById(id);
        _envioService.Delete(id);
        _notifyService.Success("Envío eliminado exitosamente.");
        return RedirectToAction("Index");
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
