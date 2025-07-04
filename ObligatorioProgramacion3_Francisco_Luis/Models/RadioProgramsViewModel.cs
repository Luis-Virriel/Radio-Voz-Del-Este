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

        public List<RadioProgram> MatutinosPrograms
        {
            get
            {
                return AllPrograms.Where(p =>
                    p.RadioDescription.ToLower().Contains("matutino") ||
                    p.RadioDescription.ToLower().Contains("mañana") ||
                    p.ProgramName.ToLower().Contains("desayuno") ||
                    p.ProgramName.ToLower().Contains("buenos días")).ToList();
            }
        }

        public List<RadioProgram> VespertinosPrograms
        {
            get
            {
                return AllPrograms.Where(p =>
                    p.RadioDescription.ToLower().Contains("tarde") ||
                    p.ProgramName.ToLower().Contains("tardes")).ToList();
            }
        }

        public List<RadioProgram> NocturnosPrograms
        {
            get
            {
                return AllPrograms.Where(p =>
                    p.RadioDescription.ToLower().Contains("noche") ||
                    p.RadioDescription.ToLower().Contains("nocturno") ||
                    p.ProgramName.ToLower().Contains("noche")).ToList();
            }
        }

        public List<RadioProgram> MusicalesPrograms
        {
            get
            {
                return AllPrograms.Where(p =>
                    p.RadioDescription.ToLower().Contains("musical") ||
                    p.RadioDescription.ToLower().Contains("música")).ToList();
            }
        }

        public List<RadioProgram> InformativosPrograms
        {
            get
            {
                return AllPrograms.Where(p =>
                    p.RadioDescription.ToLower().Contains("informativo") ||
                    p.RadioDescription.ToLower().Contains("noticias")).ToList();
            }
        }
    }
}