using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3_Francisco_Luis.Models.Metadata
{
    public class HostMetaData
    {
        [Required(ErrorMessage = "El nombre del host es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string HostName { get; set; }

        [Required(ErrorMessage = "La biografía es obligatoria.")]
        [StringLength(500, ErrorMessage = "La biografía no puede superar los 500 caracteres.")]
        public string Bio { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un programa.")]
        public int ProgramID { get; set; }
    }
}
