using System;
using System.Collections.Generic;
using SBPWebApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace SBPWebApi.SwaggerExamples
{
    public class PlanningExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<Planning>
            {
                new Planning {
                Code = "P774",
                Date = DateTime.Now.AddDays(-1).Date,
                HeureDebut = "08h30",
                HeureFin = "17h00",
                HeureDebutAM = "",
                HeureFinAM = "",
                HeureDebutPM = "",
                HeureFinPM = "",
                DureeJournee = float.Parse("8,5")
                },
                new Planning {
                Code = "P774",
                Date = DateTime.Now.AddDays(8).Date,
                HeureDebut = "08h30",
                HeureFin = "17h00",
                HeureDebutAM = "",
                HeureFinAM = "",
                HeureDebutPM = "",
                HeureFinPM = "",
                DureeJournee = float.Parse("8,5")
                },
                new Planning {
                Code = "P774",
                Date = DateTime.Now.AddDays(12).Date,
                HeureDebut = "",
                HeureFin = "",
                HeureDebutAM = "08h30",
                HeureFinAM = "12h00",
                HeureDebutPM = "14h00",
                HeureFinPM = "19h00",
                DureeJournee = float.Parse("8,5")
                }
            };
        }
    }
}