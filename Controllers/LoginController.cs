using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mvc.Data;
using Mvc.Enum;
using Mvc.Models;
using Mvc.Models.Entity;
using Mvc.Service;

namespace Mvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        protected readonly IFileService _fileService;
        private readonly INotyfService _notifyService;

        public LoginController(
            IUsuarioService usuarioService,
           UserManager<Usuario> userManager,
           SignInManager<Usuario> signInManager,
            IFileService fileService,
            INotyfService notifyService)
        {
            _usuarioService = usuarioService;
            _userManager = userManager;
            _signInManager = signInManager;
            _fileService = fileService;
            _notifyService = notifyService;
        }


        public IActionResult IniciarSesion()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Supervisor"))
                {
                    return RedirectToAction("Lista", "Supervisor");
                }
                return RedirectToAction("Consultar", "Home");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _usuarioService.IniciarSesion(model);
                if (result.Succeeded)
                {
                    var usuario = await _userManager.FindByNameAsync(model.UserName);
                    if (usuario != null)
                    {
                        if (usuario.LockoutEnabled)
                        {
                            if (User.IsInRole("Supervisor"))
                            {
                                return RedirectToAction("Lista", "Supervisor");
                            }
                            return RedirectToAction("Consultar", "Home");
                        }
                        else
                        {
                            await _signInManager.SignOutAsync();
                            _notifyService.Error("Su cuenta ha sido desactivada. Contacte al Supervisor del sitema para más información.");
                            return RedirectToAction("IniciarSesion", "Login");
                        }
                    }
                }
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            }
            return View(model);
        }


        public IActionResult Registro()
        {
            UsuarioViewModel model = new()
            {
                Id = Guid.Empty.ToString(),
                TipoUsuario = TipoUsuario.Cliente,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(UsuarioViewModel model)
        {

            if (ModelState.IsValid)
            {
                var existingUser = await _usuarioService.ObtenerPorNumeroIdentificacion(model.NumeroIdentificacion);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, $"El número de identificación '{model.NumeroIdentificacion}' ya está asociado a otro usuario.");
                    return View(model);
                }

                var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError(string.Empty, $"El correo electrónico '{model.Email}' ya existe, Intenta con otro");
                    return View(model);
                }

                Usuario usuario = await _usuarioService.CrearUsuario(model);
                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado.");
                    return View(model);
                }

                LoginViewModel loginViewModel = new()
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    RememberMe = true,

                };
                var result = await _usuarioService.IniciarSesion(loginViewModel);
                if (result.Succeeded)
                {
                    return RedirectToAction("Consultar", "Home");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await _usuarioService.CerrarSesion();
            return RedirectToAction("IniciarSesion", "Login");
        }


        [HttpGet]
        public async Task<IActionResult> EditarUsuario()
        {
            Console.WriteLine("Iniciando acción EditarUsuario");

            Usuario usuario = await _usuarioService.ObtenerUsuario(User.Identity.Name);
            Console.WriteLine($"Usuario autenticado: {User.Identity.IsAuthenticated}");

            if (usuario == null)
            {
                Console.WriteLine("Usuario no encontrado. Devolviendo NotFound().");

                return NotFound();
            }

            Usuario model = new()
            {
                ProfilePicture = usuario.ProfilePicture,
                UserName = usuario.UserName,
                TipoIdentificacion = usuario.TipoIdentificacion,
                NumeroIdentificacion = usuario.NumeroIdentificacion,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Email = usuario.Email,
                PhoneNumber = usuario.PhoneNumber,
                Id = usuario.Id,
            };

            Console.WriteLine("Usuario encontrado. Mostrando la vista con el modelo.");

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(Usuario model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var imageResult = _fileService.SaveImage(imageFile);

                    if (imageResult.Item1 == 1)
                    {
                        model.ProfilePicture = imageResult.Item2;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Error al guardar la imagen: {imageResult.Item2}");
                        return View(model);
                    }
                }
                else
                {
                    // Si no se carga una nueva imagen, mantener la misma imagen actual
                    Usuario usuarioActual = await _usuarioService.ObtenerUsuario(User.Identity.Name);
                    model.ProfilePicture = usuarioActual.ProfilePicture;
                }

                Usuario usuario = await _usuarioService.ObtenerUsuario(User.Identity.Name);
                usuario.UserName = model.UserName;
                usuario.TipoIdentificacion = model.TipoIdentificacion;
                usuario.NumeroIdentificacion = model.NumeroIdentificacion;
                usuario.Nombres = model.Nombres;
                usuario.Apellidos = model.Apellidos;
                usuario.PhoneNumber = model.PhoneNumber;
                usuario.Email = model.Email;
                usuario.ProfilePicture = model.ProfilePicture;

                await _usuarioService.ActualizarUsuario(usuario);
                return RedirectToAction("Consultar", "Home");
            }

            return View(model);
        }

    }
}