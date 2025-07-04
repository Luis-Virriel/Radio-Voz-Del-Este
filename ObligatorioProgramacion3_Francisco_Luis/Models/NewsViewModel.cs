using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ObligatorioProgramacion3_Francisco_Luis.Models
{
    public class NewsViewModel
    {
        public List<NewsItem> NewsItems { get; set; } = new List<NewsItem>();
        public List<NewsItem> TrendingNews { get; set; } = new List<NewsItem>();
        public string SelectedCategory { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
        public bool HasMoreNews => TotalCount > (CurrentPage * PageSize);
    }

    public class NewsItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime PublishDate { get; set; }

        [StringLength(500)]
        public string ImageURL { get; set; }

        // Propiedades adicionales para la vista (no están en BD)
        public string Category { get; set; } = "General";
        public string Author { get; set; } = "Redacción";
        public string AuthorRole { get; set; } = "Editor";
        public int ViewCount { get; set; } = 0;
        public bool IsFeatured { get; set; } = false;

        // Propiedades calculadas
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - PublishDate;

                if (timeSpan.TotalMinutes < 60)
                    return $"Hace {(int)timeSpan.TotalMinutes} minutos";

                if (timeSpan.TotalHours < 24)
                    return $"Hace {(int)timeSpan.TotalHours} horas";

                if (timeSpan.TotalDays < 7)
                    return $"Hace {(int)timeSpan.TotalDays} días";

                return PublishDate.ToString("dd/MM/yyyy");
            }
        }

        public string AuthorInitials
        {
            get
            {
                if (string.IsNullOrEmpty(Author)) return "R";

                var parts = Author.Split(' ');
                if (parts.Length >= 2)
                    return $"{parts[0][0]}{parts[1][0]}".ToUpper();

                return Author.Length >= 2 ? Author.Substring(0, 2).ToUpper() : Author.Substring(0, 1).ToUpper();
            }
        }

        public string Excerpt
        {
            get
            {
                if (string.IsNullOrEmpty(Content)) return "";

                // Extraer los primeros 150 caracteres como excerpt
                var plainText = Content.Length > 150 ? Content.Substring(0, 150) + "..." : Content;
                return plainText;
            }
        }

        // Método para determinar categoría basada en palabras clave del título
        public void SetCategoryFromTitle()
        {
            if (string.IsNullOrEmpty(Title)) return;

            var titleLower = Title.ToLower();

            if (titleLower.Contains("maldonado") || titleLower.Contains("punta del este") || titleLower.Contains("rocha") ||
                titleLower.Contains("centro cultural") || titleLower.Contains("municipal") || titleLower.Contains("local"))
                Category = "Local";
            else if (titleLower.Contains("fútbol") || titleLower.Contains("deporte") || titleLower.Contains("peñarol") ||
                     titleLower.Contains("nacional") || titleLower.Contains("copa") || titleLower.Contains("partido"))
                Category = "Deportes";
            else if (titleLower.Contains("turismo") || titleLower.Contains("turista") || titleLower.Contains("hotel") ||
                     titleLower.Contains("playa") || titleLower.Contains("verano") || titleLower.Contains("visitante"))
                Category = "Turismo";
            else if (titleLower.Contains("clima") || titleLower.Contains("tormenta") || titleLower.Contains("lluvia") ||
                     titleLower.Contains("temperatura") || titleLower.Contains("meteorológic"))
                Category = "Clima";
            else if (titleLower.Contains("internacional") || titleLower.Contains("uruguay") || titleLower.Contains("europa") ||
                     titleLower.Contains("acuerdo") || titleLower.Contains("comercial") || titleLower.Contains("export"))
                Category = "Internacional";
            else if (titleLower.Contains("cultura") || titleLower.Contains("festival") || titleLower.Contains("arte") ||
                     titleLower.Contains("música") || titleLower.Contains("jazz") || titleLower.Contains("cine"))
                Category = "Cultura";
            else
                Category = "General";
        }

        // Método para asignar autor aleatorio
        public void SetRandomAuthor()
        {
            var authors = new[]
            {
                new { Name = "María Rodríguez", Role = "Periodista Cultural" },
                new { Name = "Juan López", Role = "Corresponsal Deportivo" },
                new { Name = "Ana Silva", Role = "Editora Internacional" },
                new { Name = "Carlos Méndez", Role = "Especialista en Turismo" },
                new { Name = "Laura Fernández", Role = "Meteoróloga" },
                new { Name = "Diego Ruiz", Role = "Crítico Musical" },
                new { Name = "Patricia González", Role = "Editora Local" },
                new { Name = "Roberto Santos", Role = "Corresponsal" }
            };

            var random = new Random(Id); // Usar ID como semilla para consistencia
            var selectedAuthor = authors[random.Next(authors.Length)];

            Author = selectedAuthor.Name;
            AuthorRole = selectedAuthor.Role;
        }

        // Método para asignar imagen por defecto si no tiene
        public void SetDefaultImageIfEmpty()
        {
            if (string.IsNullOrEmpty(ImageURL))
            {
                // Asignar imagen por defecto según categoría
                switch (Category.ToLower())
                {
                    case "local":
                        ImageURL = "https://images.unsplash.com/photo-1581553673739-c4908841d3b5?w=600&h=400&fit=crop";
                        break;
                    case "deportes":
                        ImageURL = "https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=400&h=220&fit=crop";
                        break;
                    case "internacional":
                        ImageURL = "https://images.unsplash.com/photo-1494790108755-2616c27d8090?w=400&h=220&fit=crop";
                        break;
                    case "turismo":
                        ImageURL = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=400&h=220&fit=crop";
                        break;
                    case "clima":
                        ImageURL = "https://images.unsplash.com/photo-1504608524841-42fe6f032b4b?w=400&h=220&fit=crop";
                        break;
                    case "cultura":
                        ImageURL = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400&h=220&fit=crop";
                        break;
                    default:
                        ImageURL = "https://images.unsplash.com/photo-1585829365295-ab7cd400c167?w=400&h=220&fit=crop";
                        break;
                }
            }
        }
    }
}