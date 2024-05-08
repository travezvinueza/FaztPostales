using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mvc.Data;
using Mvc.Models;
using Mvc.Models.Entity;
using Mvc.Service;

namespace Mvc.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        protected readonly IFileService _fileService;
        private readonly INotyfService _notifyService;
        private readonly DatabaseContext _context;
        private readonly UserManager<Usuario> _userManager;


        public SupervisorController(
            DatabaseContext context,
            IUsuarioService usuarioService,
            INotyfService notifyService,
            UserManager<Usuario> userManager,
            IFileService fileService)
        {
            _context = context;
            _usuarioService = usuarioService;
            _notifyService = notifyService;
            _fileService = fileService;
            _userManager = userManager;

        }

        public async Task<IActionResult> Lista()
        {
            var usuarios = await _usuarioService.GetAll();
            return View(usuarios);
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            var model = new UsuarioViewModel
            {
                AvailableRoles = _context.Roles.Select(r => r.Name).ToList()!
            };

            return View(model);
        }

        //agrega y guarda el usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(UsuarioViewModel newUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(newUser);
                }

                if (newUser.ImageFile != null)
                {
                    var imageResult = _fileService.SaveImage(newUser.ImageFile);

                    if (imageResult.Item1 != 1)
                    {
                        ModelState.AddModelError("", imageResult.Item2);
                        return View(newUser);
                    }

                    newUser.ProfilePicture = imageResult.Item2;
                }

                var user = new Usuario
                {
                    ProfilePicture = newUser.ProfilePicture,
                    UserName = newUser.UserName,
                    TipoIdentificacion = newUser.TipoIdentificacion,
                    NumeroIdentificacion = newUser.NumeroIdentificacion,
                    Nombres = newUser.Nombres,
                    Apellidos = newUser.Apellidos,
                    Email = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };

                var result = await _userManager.CreateAsync(user, newUser.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, newUser.TipoUsuario.ToString());
                    return RedirectToAction("Lista");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(newUser);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error inesperado: " + ex.Message);
                return View(newUser);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EliminarUsuario(string username)
        {
            try
            {
                var result = await _usuarioService.EliminarUsuario(username);

                if (result.Succeeded)
                {
                    _notifyService.Success("Usuario eliminado correctamente.");
                }
                else
                {
                    _notifyService.Error("Error al eliminar el usuario.");
                }
            }
            catch (Exception)
            {
                _notifyService.Error("Ocurrió un error al procesar la solicitud.");
            }

            return RedirectToAction("Lista");
        }


        //actualizar Cliente
      [HttpGet]
        public async Task<IActionResult> Editar(string username)
        {
            var usuario = await _usuarioService.ObtenerUsuario(username);
            if (usuario == null)
            {
                return NotFound();
            }
            
            // Aquí puedes obtener los roles del usuario y pasarlos al modelo de vista si es necesario
            var rolesUsuario = await _usuarioService.ObtenerRolesUsuario(usuario.UserName);

            var model = new ActualizarViewModel
            {
                UserName = usuario.UserName,
                TipoIdentificacion = usuario.TipoIdentificacion!,
                NumeroIdentificacion = usuario.NumeroIdentificacion!,
                Nombres = usuario.Nombres!,
                Apellidos = usuario.Apellidos!,
                Email = usuario.Email!,
                PhoneNumber = usuario.PhoneNumber!,
                AvailableRoles = rolesUsuario
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ActualizarViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _usuarioService.UpdateUser(model);
            if (result.Succeeded)
            {
                return RedirectToAction("Lista", "Supervisor"); // Redirige a la acción Lista del controlador Usuario
            }

            // Si hay algún error al actualizar el usuario, añade los errores al ModelState y vuelve a mostrar el formulario
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }




        public IActionResult Roles()
        {
            var roles = _context.Roles.ToList();
            return View(roles);
        }




    }
}