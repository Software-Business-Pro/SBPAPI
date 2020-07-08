using System;
using System.Collections.Generic;
using SBPWebApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace SBPWebApi.SwaggerExamples
{
    public class LienImageExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<LienImage>
            {
                new LienImage {
                    IdDb = "25",
                    Code = "P774",
                    lienImage = "https://projet-annuel-esgi.s3.eu-west-3.amazonaws.com/Corpscassee.png",
                    DateUpload = DateTime.Now.AddDays(-1)
                },
                new LienImage {
                    IdDb = "39",
                    Code = "P774",
                    lienImage = "https://projet-annuel-esgi.s3.eu-west-3.amazonaws.com/brasreparation.png",
                    DateUpload = DateTime.Now
                }
            };
        }
    }
}