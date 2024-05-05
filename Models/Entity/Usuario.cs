using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Mvc.Enum;

namespace Mvc.Models.Entity
{
    public class Usuario : IdentityUser
    {
       
        public string? TipoIdentificacion { get; set; }

        public string? NumeroIdentificacion { get; set; }

        public string? Nombres { get; set; }

        public string? Apellidos { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        
        public string? ProfilePicture { get; set; }

        public TipoUsuario TipoUsuario { get; set; }
        
        
    }
}

