using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3_Francisco_Luis.Models.Metadata
{
    public class UsersMetaData
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede tener más de 50 caracteres.")]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [Display(Name = "Contraseña")]
        public string UserPassword { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un rol válido.")]
        [Display(Name = "Rol de Usuario")]
        public int RoleID { get; set; }
    }
}
