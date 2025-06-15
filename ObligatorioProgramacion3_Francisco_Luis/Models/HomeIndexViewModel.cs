using System.Collections.Generic;

namespace ObligatorioProgramacion3_Francisco_Luis.Models
{
    public class HomeIndexViewModel
    {
        public RadioProgram CurrentProgram { get; set; }
        public RadioProgram NextProgram { get; set; }
        public List<RadioProgram> ProgramsList { get; set; }
    }
}
