using Microsoft.AspNetCore.Identity;
using Mvc.Models;
using Mvc.Models.Entity;

namespace Mvc.Service
{
    public interface IUsuarioService
    {
        Task<IdentityResult> SupervisorCrearUsuario(Usuario usuario, string password);
        Task<Usuario> ObtenerUsuario(string username);
        Task VerificarRol(string nombreRol);
        Task AsignarRol(Usuario usuario, string nombreRol);
        Task<bool> UsuarioEnRol(Usuario usuario, string nombreRol);
        Task<SignInResult> IniciarSesion(LoginViewModel model);
        Task CerrarSesion();
        Task<Usuario> CrearUsuario(UsuarioViewModel model);
        Task<IdentityResult> ActualizarUsuario(Usuario usuario);
        Task<Usuario> AdminObtenerUsuario(Guid userId);

        Task<List<Usuario>> GetAll(); 
        Task<IdentityResult> EliminarUsuario(string username);

        Task<IdentityResult> AgregarUsuario(UsuarioViewModel model);
        
    }
}