using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3_Francisco_Luis.Models.Metadata
{
    public class SponsorMetaData
    {
        [Required(ErrorMessage = "El nombre del patrocinador es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string SponsorsName { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
        public string SponsorDescription { get; set; }

        [Required(ErrorMessage = "Debe especificar la cantidad de planes.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de planes debe ser un número positivo.")]
        public int CantPlan { get; set; }
    }
}
