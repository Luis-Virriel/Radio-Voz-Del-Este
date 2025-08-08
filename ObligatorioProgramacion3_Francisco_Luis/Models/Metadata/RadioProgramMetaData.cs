using System;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3_Francisco_Luis.Models.Metadata
{
    public class RadioProgramMetaData
    {
        [Required(ErrorMessage = "El nombre del programa es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string ProgramName { get; set; }

        [Display(Name = "Portada")]
        public string ImageFile { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
        public string RadioDescription { get; set; }

        [Required(ErrorMessage = "La programación es obligatoria.")]
        [DataType(DataType.DateTime)]
        public DateTime? Schedule { get; set; }
    }
}
