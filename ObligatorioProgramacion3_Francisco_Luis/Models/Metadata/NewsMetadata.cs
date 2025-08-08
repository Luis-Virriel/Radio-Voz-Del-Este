using System;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3_Francisco_Luis.Models.Metadata
{
    public class NewsMetaData
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(4000, ErrorMessage = "La descripción no puede superar los 4000 caracteres.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "La fecha de publicación es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }

        [Required(ErrorMessage = "La portada es obligatoria.")]
        [StringLength(500, ErrorMessage = "La URL de la portada no puede superar los 500 caracteres.")]
        public string ImageURL { get; set; }
    }
}
