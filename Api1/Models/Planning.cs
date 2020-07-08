using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SBPWebApi.Models
{
    public class Planning
    {
        public string Code;
        public DateTime Date;
        public string HeureDebut;
        public string HeureFin;
        public string HeureDebutAM;
        public string HeureFinAM;
        public string HeureDebutPM;
        public string HeureFinPM;
        public float DureeJournee;
    }
}
