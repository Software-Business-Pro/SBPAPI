using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SBPWebApi.Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using Amazon.S3;
using System.IO;
using Amazon;
using Amazon.S3.Transfer;
using System.Data.SqlClient;
using System.Data;
using SBPWebApi.Configuration;

namespace SBPWebApi.Services
{
    internal class SBPService : ISBPService
    {
        private string connection;
        private string awskey;
        private string awsmdp;

        public SBPService() {
        }

        public void SetConnectionString(ConnectionStrings mysql)
        {
            connection = mysql.ApiDb;
            awskey = mysql.Aws;
            awsmdp = mysql.Awsmdp;
        }

        public void UploadFileToS3(IFormFile file, string fileName)
        {
            using (var client = new AmazonS3Client(awskey, awsmdp, RegionEndpoint.EUWest3))
            {
                using (var newMemoryStream = new MemoryStream())
                {
                    file.CopyTo(newMemoryStream);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = fileName,
                        BucketName = "projet-annuel-esgi",
                        CannedACL = S3CannedACL.PublicRead
                    };

                    var fileTransferUtility = new TransferUtility(client);
                    fileTransferUtility.Upload(uploadRequest);
                }
            }
        }

        public Task<IEnumerable<Vehicule>> GetAllVehicules()
        {
            SqlConnection con = new SqlConnection(connection);
            con.Open();
            string procedureName = "[dbo].[SP_GetVehicules]";
            var result = new List<Vehicule>();
            using (SqlCommand command = new SqlCommand(procedureName, con))
            {
                command.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Vehicule tmpRecord = new Vehicule()
                        {
                            MatRef = reader[0].ToString(),
                            MatLibelle = reader[1].ToString(),
                            CliRef = reader[2].ToString(),
                            CodeProprietaire = reader[3].ToString(),
                            CatProprietaire = reader[4].ToString(),
                            MatImatriculation = reader[5].ToString(),
                            MatChauffeur = reader[6].ToString(),
                            MatNumSerie = reader[7].ToString(),
                            MatPTAC = reader[8].ToString(),
                            MatLongueur = reader[9].ToString(),
                            MatLargeur = reader[10].ToString(),
                            MatHauteur = reader[11].ToString(),
                            MatPoids = reader[12].ToString(),
                            Remarque = reader[13].ToString(),
                            MatChauffeurTel = reader[14].ToString()
                        };
                        result.Add(tmpRecord);
                    }
                }
            }
            con.Close();
            return Task.FromResult<IEnumerable<Vehicule>>(result);
        }

        public Task<IEnumerable<Planning>> GetPlanning(string id)
        {
            SqlConnection con = new SqlConnection(connection);
            con.Open();
            string procedureName = "[dbo].[SP_GetPlanning]";
            var result = new List<Planning>();
            using (SqlCommand command = new SqlCommand(procedureName, con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@PRP_CODE", id));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Planning tmpRecord = new Planning()
                        {
                            Code = reader[0].ToString(),
                            Date = Convert.ToDateTime(reader[1].ToString()),
                            HeureDebut = reader[2].ToString(),
                            HeureFin = reader[3].ToString(),
                            HeureDebutAM = reader[4].ToString(),
                            HeureFinAM = reader[5].ToString(),
                            HeureDebutPM = reader[6].ToString(),
                            HeureFinPM = reader[7].ToString(),
                            DureeJournee = float.Parse(reader[8].ToString())
                        };
                        result.Add(tmpRecord);
                    }
                }
            }
            con.Close();
            return Task.FromResult<IEnumerable<Planning>>(result);
        }

        public Task<IEnumerable<LienImage>> GetImageLinks(string id)
        {
            SqlConnection con = new SqlConnection(connection);
            con.Open();
            string procedureName = "[dbo].[SP_GetImageLinks]";
            var result = new List<LienImage>();
            using (SqlCommand command = new SqlCommand(procedureName, con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@PRP_CODE", id));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string datetest = reader[3].ToString();
                        LienImage tmpRecord = new LienImage()
                        {
                            IdDb = reader[0].ToString(),
                            Code = reader[1].ToString(),
                            lienImage = reader[2].ToString(),
                            DateUpload = Convert.ToDateTime(reader[3].ToString())
                        };
                        result.Add(tmpRecord);
                    }
                }
            }
            con.Close();
            return Task.FromResult<IEnumerable<LienImage>>(result);
        }

        public void uploadImage(IFormFile file, string id)
        {
            //prepare DATA
            DateTime update = DateTime.Now;
            string baseurl = "https://projet-annuel-esgi.s3.eu-west-3.amazonaws.com/";
            string name = id + update.ToString() + file.FileName;
            name = name.Replace(' ', '_');
            name = name.Replace('/', '_');

            //upload file
            UploadFileToS3(file, name);

            //save link
            SqlConnection con = new SqlConnection(connection);
            con.Open();
            string procedureName = "[dbo].[SP_SaveImageLinks]";
            var result = new List<Vehicule>();
            using (SqlCommand command = new SqlCommand(procedureName, con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@PRP_CODE", id));
                command.Parameters.Add(new SqlParameter("@LINK", baseurl+name));
                command.Parameters.Add(new SqlParameter("@DATE", update));
                command.ExecuteNonQuery();
            }
            con.Close();
            return;
        }
    }
}
