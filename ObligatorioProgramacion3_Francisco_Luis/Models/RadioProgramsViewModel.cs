using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ObligatorioProgramacion3_Francisco_Luis.Models
{
    public class RadioProgramsViewModel
    {
        public List<RadioProgram> AllPrograms { get; set; } = new List<RadioProgram>();
        public RadioProgram CurrentProgram { get; set; }
        public int TotalPrograms { get; set; }

        // Nueva propiedad para comentarios por programa
        public Dictionary<int, List<CommentViewModel>> ProgramComments { get; set; } = new Dictionary<int, List<CommentViewModel>>();

        // Propiedades existentes de categorías
        public List<RadioProgram> MatutinosPrograms
        {
            get
            {
                return AllPrograms.Where(p =>
                    (p.RadioDescription != null && p.RadioDescription.ToLower().Contains("matutino")) ||
                    (p.RadioDescription != null && p.RadioDescription.ToLower().Contains("mañana")) ||
                    (p.ProgramName != null && p.ProgramName.ToLower().Contains("desayuno")) ||
                    (p.ProgramName != null && p.ProgramName.ToLower().Contains("buenos días"))).ToList();
            }
        }

        public List<RadioProgram> VespertinosPrograms
        {
            get
            {
                return AllPrograms.Where(p =>
                    (p.RadioDescription != null && p.RadioDescription.ToLower().Contains("tarde")) ||
                    (p.ProgramName != null && p.ProgramName.ToLower().Contains("tardes"))).ToList();
            }
        }

        public List<RadioProgram> NocturnosPrograms
        {
            get
            {
                return AllPrograms.Where(p =>
                    (p.RadioDescription != null && p.RadioDescription.ToLower().Contains("noche")) ||
                    (p.RadioDescription != null && p.RadioDescription.ToLower().Contains("nocturno")) ||
                    (p.ProgramName != null && p.ProgramName.ToLower().Contains("noche"))).ToList();
            }
        }

        public List<RadioProgram> MusicalesPrograms
        {
            get
            {
                return AllPrograms.Where(p =>
                    (p.RadioDescription != null && p.RadioDescription.ToLower().Contains("musical")) ||
                    (p.RadioDescription != null && p.RadioDescription.ToLower().Contains("música"))).ToList();
            }
        }

        public List<RadioProgram> InformativosPrograms
        {
            get
            {
                return AllPrograms.Where(p =>
                    (p.RadioDescription != null && p.RadioDescription.ToLower().Contains("informativo")) ||
                    (p.RadioDescription != null && p.RadioDescription.ToLower().Contains("noticias"))).ToList();
            }
        }

        // Métodos helper para comentarios
        public List<CommentViewModel> GetCommentsForProgram(int programId)
        {
            return ProgramComments.ContainsKey(programId)
                ? ProgramComments[programId]
                : new List<CommentViewModel>();
        }

        public int GetCommentCountForProgram(int programId)
        {
            return GetCommentsForProgram(programId).Count;
        }
    }

    // ViewModel para comentarios individuales
    public class CommentViewModel
    {
        public int ID { get; set; }
        public int ClientID { get; set; } // Mantener como int para el ViewModel
        public int ProgramID { get; set; }
        public string Comment { get; set; }
        public DateTime CommentDate { get; set; }

        // Información del cliente que comenta
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }

        // Propiedades de formato
        public string FormattedDate
        {
            get
            {
                return CommentDate.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - CommentDate;

                if (timeSpan.TotalMinutes < 1)
                    return "Hace un momento";
                else if (timeSpan.TotalMinutes < 60)
                    return $"Hace {(int)timeSpan.TotalMinutes} minuto{((int)timeSpan.TotalMinutes != 1 ? "s" : "")}";
                else if (timeSpan.TotalHours < 24)
                    return $"Hace {(int)timeSpan.TotalHours} hora{((int)timeSpan.TotalHours != 1 ? "s" : "")}";
                else if (timeSpan.TotalDays < 7)
                    return $"Hace {(int)timeSpan.TotalDays} día{((int)timeSpan.TotalDays != 1 ? "s" : "")}";
                else
                    return FormattedDate;
            }
        }
    }

    // ViewModel para crear nuevos comentarios
    public class CreateCommentViewModel
    {
        public int ProgramID { get; set; }
        public string Comment { get; set; }
    }
}