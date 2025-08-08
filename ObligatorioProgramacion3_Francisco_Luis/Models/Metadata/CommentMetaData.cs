using System;
using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3_Francisco_Luis.Models.Metadata
{
    public class CommentMetaData
    {
        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        public int ClientID { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un programa.")]
        public int ProgramID { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio.")]
        [StringLength(1000, ErrorMessage = "El comentario no puede superar los 1000 caracteres.")]
        public string Comment1 { get; set; }

        [Required(ErrorMessage = "Debe ingresar la fecha del comentario.")]
        [DataType(DataType.DateTime)]
        public DateTime CommentDate { get; set; }
    }
}
