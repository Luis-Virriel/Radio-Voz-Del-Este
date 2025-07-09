using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ObligatorioProgramacion3_Francisco_Luis.Models
{
    public class SponsorsViewModel
    {
        public List<SponsorItem> Sponsors { get; set; } = new List<SponsorItem>();
        public int TotalSponsors { get; set; }
        public int TotalPlans { get; set; }
        public double AveragePlans { get; set; }
    }

    public class SponsorItem
    {
        public int ID { get; set; }
        public string SponsorsName { get; set; }
        public string SponsorsDescription { get; set; }
        public int CantPlan { get; set; }
    }
}