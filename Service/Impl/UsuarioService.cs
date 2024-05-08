using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mvc.Data;
using Mvc.Models;
using Mvc.Models.Entity;

namespace Mvc.Service.Impl
{
    public class UsuarioService : IUsuarioService
    {
        private readonly DatabaseContext _context;
        protected readonly IFileService _fileService;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Usuario> _signInManager;

        public UsuarioService(
            DatabaseContext context,
            UserManager<Usuario> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<Usuario> signInManager,
            IFileService fileService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _fileService = fileService;
        }

        public async Task<IdentityResult> SupervisorCrearUsuario(Usuario usuario, string password)
        {
            return await _userManager.CreateAsync(usuario, password);
        }

        public async Task<Usuario> ObtenerUsuario(string username)
        {
            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            return usuario;
        }

        public async Task VerificarRol(string nombreRol)
        {
            bool roleExiste = await _roleManager.RoleExistsAsync(nombreRol);
            if (!roleExiste)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = nombreRol
                });
            }

        }

        public async Task AsignarRol(Usuario usuario, string nombreRol)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario), "El usuario no puede ser nulo.");
            }

            await _userManager.AddToRoleAsync(usuario, nombreRol);
        }

        public async Task<bool> UsuarioEnRol(Usuario usuario, string nombreRol)
        {
            return await _userManager.IsInRoleAsync(usuario, nombreRol);
        }

        public async Task<SignInResult> IniciarSesion(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
            model.UserName,
            model.Password,
            model.RememberMe,
            true);
        }

         public async Task CerrarSesion()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<Usuario> CrearUsuario(UsuarioViewModel model)
        {
            Usuario usuario = new Usuario
            {
                NumeroIdentificacion = model.NumeroIdentificacion,
                Nombres = model.Nombres,
                Apellidos = model.Apellidos,
                UserName = model.UserName,
                Email = model.Email ?? throw new ArgumentNullException(nameof(model.Email), "El correo electrónico no puede ser nulo."),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                TipoUsuario = model.TipoUsuario
            };

            IdentityResult result = await _userManager.CreateAsync(usuario, model.Password);

            if (result != IdentityResult.Success)
            {
                throw new InvalidOperationException($"Error al crear el usuario: {string.Join(", ", result.Errors)}");
            }

            Usuario nuevoUsuario = await ObtenerUsuario(usuario.UserName!);

            if (nuevoUsuario == null)
            {
                throw new ArgumentNullException(nameof(nuevoUsuario), "El nuevo usuario no puede ser nulo.");
            }

            await AsignarRol(nuevoUsuario, usuario.TipoUsuario.ToString());

            return nuevoUsuario;
        }

        public async Task<IdentityResult> ActualizarUsuario(Usuario usuario)
        {
            return await _userManager.UpdateAsync(usuario);
        }

        public async Task<Usuario> AdminObtenerUsuario(Guid userId)
        {
            return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId.ToString());
        }

        public async Task<List<Usuario>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IdentityResult> EliminarUsuario(string username)
        {
            var usuario = await _userManager.FindByNameAsync(username);
            if (usuario == null)
            {
                // Usuario no encontrado
                return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado." });
            }

            var result = await _userManager.DeleteAsync(usuario);

            return result;
        }

        public async Task<IdentityResult> AgregarUsuario(UsuarioViewModel model)
        {
            Tuple<int, string> imageResult = null!;

            if (model.ImageFile != null)
            {
                imageResult = _fileService.SaveImage(model.ImageFile);

                if (imageResult.Item1 != 1)
                {

                    return IdentityResult.Failed(new IdentityError { Description = imageResult.Item2 });
                }

                model.ProfilePicture = imageResult.Item2;
            }

            var user = new Usuario
            {
                ProfilePicture = model.ProfilePicture,
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,

            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                foreach (var role in model.TipoUsuario.ToString().Split(','))
                {
                    await VerificarRol(role.Trim());
                }

                await _userManager.AddToRolesAsync(user, model.TipoUsuario.ToString().Split(','));
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return result;
        }

       

        public async Task<IdentityResult> UpdateUser(ActualizarViewModel model)
        {
            // Obtener el usuario existente
            var usuario = await _userManager.FindByNameAsync(model.UserName);

            if (usuario == null)
            {
                // Manejar el caso en que el usuario no se encuentre
                return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado." });
            }

            // Actualizar las propiedades del usuario
            usuario.Email = model.Email;
            usuario.UserName = model.UserName;
            usuario.TipoIdentificacion = model.TipoIdentificacion;
            usuario.NumeroIdentificacion = model.NumeroIdentificacion;
            usuario.Nombres = model.Nombres;
            usuario.Apellidos = model.Apellidos;
            usuario.PhoneNumber = model.PhoneNumber;
           
            var result = await _userManager.UpdateAsync(usuario);

            if (result.Succeeded)
            {
                // Actualizar roles del usuario
                var roles = await _userManager.GetRolesAsync(usuario);
               

                var resultRoles = await _userManager.AddToRolesAsync(usuario, model.AvailableRoles);
                if (!resultRoles.Succeeded)
                {
                    // Manejar el caso en que no se puedan actualizar los roles
                    return IdentityResult.Failed(new IdentityError { Description = "Error al actualizar los roles del usuario." });
                }
            }

            return result;
        }

        public async Task<List<string>> ObtenerRolesUsuario(string username)
        {
            var usuario = await _userManager.FindByNameAsync(username);
            if (usuario == null)
            {
                return new List<string>();
            }

            return (await _userManager.GetRolesAsync(usuario)).ToList();
        }
    }
}