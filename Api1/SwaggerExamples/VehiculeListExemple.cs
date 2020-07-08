using System;
using System.Collections.Generic;
using SBPWebApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace SBPWebApi.SwaggerExamples
{
    public class VehiculeListExemple : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<Vehicule>
            {
                new Vehicule {
                    MatRef = "P774",
                    MatLibelle = "pelleteuse",
                    CliRef = "",
                    CodeProprietaire = "STD",
                    CatProprietaire = "C",
                    MatImatriculation = "82-ASV-987",
                    MatChauffeur = "",
                    MatNumSerie = "71451616457",
                    MatPTAC = "16250",
                    MatLongueur = "325",
                    MatLargeur = "300",
                    MatHauteur = "400",
                    MatPoids = "12000",
                    Remarque = "",
                    MatChauffeurTel = ""
                },
                new Vehicule {
                    MatRef = "P778",
                    MatLibelle = "pelleteuse Deblayeuse",
                    CliRef = "",
                    CodeProprietaire = "STD",
                    CatProprietaire = "C",
                    MatImatriculation = "82-VQS-987",
                    MatChauffeur = "",
                    MatNumSerie = "71451616457",
                    MatPTAC = "16250",
                    MatLongueur = "325",
                    MatLargeur = "300",
                    MatHauteur = "400",
                    MatPoids = "12000",
                    Remarque = "",
                    MatChauffeurTel = ""
                }
            };
        }
    }
}