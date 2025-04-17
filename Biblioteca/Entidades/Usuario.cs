using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Entidades
{
    public class Usuario : IdentityUser
    {
        public DateTime FechaNacimiento { get; set; }
    }
}
