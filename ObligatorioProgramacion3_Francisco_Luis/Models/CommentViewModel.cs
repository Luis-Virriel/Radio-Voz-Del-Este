// REEMPLAZA tu CommentViewModel existente con esta versión corregida

using System;

public class CommentViewModel
{
    public int ID { get; set; }
    public string ClientIDString { get; set; } // Para manejar el string de la BD
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