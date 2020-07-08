using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SBPWebApi.Models;

namespace SBPWebApi.Services
{
    /// <summary>
    /// Represents the set of methods for Foo manipulation.
    /// </summary>
    public interface ISBPService
    {
        void SetConnectionString(string mysqlConStr);


        /// <summary>
        /// Tries to retrieve all Vehicule objects.
        /// </summary>
        /// <returns>A collection of Vehicule objects (collection might be empty, but never null).</returns>
        Task<IEnumerable<Vehicule>> GetAllVehicules();


        /// <summary>
        /// Tries to retrieve all plannings objects of a Vehicule.
        /// </summary>
        /// <returns>A collection of plannings objects (collection might be empty, but never null).</returns>
        Task<IEnumerable<Planning>> GetPlanning(string id);

        /// <summary>
        /// Tries to retrieve all image links of a Vehicule.
        /// </summary>
        /// <returns>A collection of plannings objects (collection might be empty, but never null).</returns>
        Task<IEnumerable<LienImage>> GetImageLinks(string id);

        /// <summary>
        /// upload an image to S3 and send link in db.
        /// </summary>
        void uploadImage(IFormFile file, string id);
    }
}