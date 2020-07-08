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

namespace SBPWebApi.Services
{
    internal class SBPService : ISBPService
    {
        private string connection;

        public SBPService() {
        }

        public void SetConnectionString(string mysqlConStr)
        {
            connection = mysqlConStr;
        }

        public void UploadFileToS3(IFormFile file, string fileName)
        {
            using (var client = new AmazonS3Client("AKIAI5DLYSC3ETDPGFOQ", "HxyHm3DUdXg81xp/0d3pGC0+g31wOQiUVtaUXxQF", RegionEndpoint.EUWest3))
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












        /*public Task<IEnumerable<Bar>> GetAllBars()
        {
            List<Bar> list = new List<Bar>();

            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("GetAllBars", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Bar()
                        {
                            id = Convert.ToInt32(reader["id"]),
                            name = reader["name"].ToString(),
                            phone = reader["phone"].ToString(),
                            lat = float.Parse(reader["lat"].ToString()),
                            lon = float.Parse(reader["lon"].ToString()),
                            cheaper_pint = float.Parse(reader["cheaper_pint"].ToString()),
                            adress = reader["adress"].ToString()
                        });
                    }
                }
            }
            return Task.FromResult<IEnumerable<Bar>>(list);
        }

        public Task<Bar> GetBar(int id)
        {
            Bar result;
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("GetBar", conn);
                cmd.Parameters.AddWithValue("@bar_ID", id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    result = new Bar()
                    {
                            id = Convert.ToInt32(reader["id"]),
                            name = reader["name"].ToString(),
                            phone = reader["phone"].ToString(),
                            lat = float.Parse(reader["lat"].ToString()),
                            lon = float.Parse(reader["lon"].ToString()),
                            cheaper_pint = float.Parse(reader["cheaper_pint"].ToString()),
                            adress = reader["adress"].ToString()
                    };
                }
            }
            return Task.FromResult(result);
        }

        public Task<Reservations> GetReservations(int id)
        {
            Reservations result = new Reservations();

            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("GetAcceptedRes", conn);
                cmd.Parameters.AddWithValue("@bar_ID", id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.accepted.Add(new Reservation()
                        {
                            id = Convert.ToInt32(reader["id"]),
                            name = reader["name"].ToString(),
                            number = reader["number"].ToString(),
                            email = reader["mail"].ToString(),
                            nb_persons = Convert.ToInt32(reader["nb_persons"]),
                            date = Convert.ToDateTime(reader["date"].ToString())
                        });
                    }
                }

                cmd = new MySqlCommand("GetWaitingRes", conn);
                cmd.Parameters.AddWithValue("@bar_ID", id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.waiting.Add(new Reservation()
                        {
                            id = Convert.ToInt32(reader["id"]),
                            name = reader["name"].ToString(),
                            number = reader["number"].ToString(),
                            email = reader["mail"].ToString(),
                            nb_persons = Convert.ToInt32(reader["nb_persons"]),
                            date = Convert.ToDateTime(reader["date"].ToString())
                        });
                    }
                }

                cmd = new MySqlCommand("GetDeniedRes", conn);
                cmd.Parameters.AddWithValue("@bar_ID", id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.denied.Add(new Reservation()
                        {
                            id = Convert.ToInt32(reader["id"]),
                            name = reader["name"].ToString(),
                            number = reader["number"].ToString(),
                            email = reader["mail"].ToString(),
                            nb_persons = Convert.ToInt32(reader["nb_persons"]),
                            date = Convert.ToDateTime(reader["date"].ToString())
                        });
                    }
                }
            }
            return Task.FromResult(result);
        }

        public Task<IEnumerable<MenuItem>> GetMenu(int id)
        {
            List<MenuItem> list = new List<MenuItem>();

            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("GetMenuItem", conn);
                cmd.Parameters.AddWithValue("@bar_ID", id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new MenuItem()
                        {
                            id = Convert.ToInt32(reader["id"]),
                            name = reader["name"].ToString(),
                            price = float.Parse(reader["lat"].ToString())
                        });
                    }
                }
            }
            return Task.FromResult<IEnumerable<MenuItem>>(list);
        }

        public void CreateMenuItem(MenuItem item, int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("InsertMenuItem", conn);
                cmd.Parameters.AddWithValue("@bar_ID", id);
                cmd.Parameters.AddWithValue("@bar_ID", id);
                cmd.Parameters.AddWithValue("@bar_ID", id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }

        public void CreateReservation(Reservation item, int id, int user_id)
        {
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("InsertReservation", conn);
                cmd.Parameters.AddWithValue("@bar_ID", id);
                cmd.Parameters.AddWithValue("@in_name", item.name);
                cmd.Parameters.AddWithValue("@in_nb", item.nb_persons);
                cmd.Parameters.AddWithValue("@in_date", item.date);
                cmd.Parameters.AddWithValue("@user_ID", user_id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }

        public void AcceptEvent(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("AcceptReservation", conn);
                cmd.Parameters.AddWithValue("@res_ID", id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }

        public void RefuseEvent(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("RefuseReservation", conn);
                cmd.Parameters.AddWithValue("@res_ID", id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteMenuItem(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DeleteMenuItem", conn);
                cmd.Parameters.AddWithValue("@item_ID", id);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }*/
    }
}