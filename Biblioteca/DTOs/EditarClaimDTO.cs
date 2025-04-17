using System.ComponentModel.DataAnnotations;

namespace Biblioteca.DTOs
{
    public class EditarClaimDTO
    {
        [EmailAddress]
        [Required]
        public required string Email { get; set; }
    }
}
