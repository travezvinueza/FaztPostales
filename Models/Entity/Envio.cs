using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mvc.Models.Entity
{
    public class Envio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }   
        public string Codigo { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [ForeignKey("UsuarioId")] 
        public string? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        
    }
}

