using System;
using System.ComponentModel.DataAnnotations;
using ObligatorioProgramacion3_Francisco_Luis.Models.Validations;

namespace ObligatorioProgramacion3_Francisco_Luis.Models.Metadata
{
    public class ClientMetaData
    {
        [Required(ErrorMessage = "El número de identificación es obligatorio.")]
        [StringLength(20, ErrorMessage = "El número de identificación no puede tener más de 20 caracteres.")]
        [Display(Name = "Número de Identificación")]
        public string IDNumber { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede tener más de 50 caracteres.")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede tener más de 100 caracteres.")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        [MayorEdad(ErrorMessage ="Debe tener 18 años o mas")]
        public Nullable<DateTime> BirthDate { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un usuario.")]
        [Display(Name = "Usuario Asociado")]
        public Nullable<int> UserID { get; set; }
    }
}
