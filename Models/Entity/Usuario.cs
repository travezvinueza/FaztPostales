using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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

        [JsonIgnore]
        public virtual ICollection<Envio> Envios { get; set; } = new List<Envio>();
        
        
    }
}

