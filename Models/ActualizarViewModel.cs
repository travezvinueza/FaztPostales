using System.ComponentModel.DataAnnotations;
using Mvc.Enum;

namespace Mvc.Models
{
    public class ActualizarViewModel
    {
     
        public string Email { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string TipoIdentificacion { get; set; } = string.Empty;

        public string NumeroIdentificacion { get; set; } = string.Empty;
     
        public string Nombres { get; set; } = string.Empty;
  
        public string Apellidos { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public TipoUsuario TipoUsuario { get; set; }
        
        public List<string> AvailableRoles { get; set; } = new List<string>();

        
        public List<string> SelectedRoles { get; set; } = new List<string>();
        
    }
}