using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Web.Http;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace WebAPITeste.Controllers
{
    public class results
    {
        public string name { get; set; }
        public string password {get; set; }

        public string error { get; set; }
    
        public results(string name, string password, string error)
        {
            this.name = name;
            this.password = password;
            this.error = error;
        }
    }

    public class ValuesController : ApiController
    {
        // GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/values
        [HttpGet]
        public List<results> GetAll()
        {
            MySqlConnection conn = WebApiConfig.conn();

            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "SELECT username, password FROM users";


            var results = new List<results>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new results(null, null, ex.ToString()));
            }

            MySqlDataReader fetch_query = query.ExecuteReader();

            while(fetch_query.Read())
            {
                results.Add(new results(fetch_query["username"].ToString(), fetch_query["password"].ToString(), null));
            }

            conn.Close();

            return results;
        }

        // GET api/values/5
        [HttpGet]
        public List<results> GetUser(int id)
        {
            MySqlConnection conn = WebApiConfig.conn();

            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "SELECT username, password FROM users WHERE id = @id";

            query.Parameters.AddWithValue("@id", id);

            var results = new List<results>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new results(null, null, ex.ToString()));
            }

            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                results.Add(new results(fetch_query["username"].ToString(), fetch_query["password"].ToString(), null));
            }

            conn.Close();

            return results;
        }

        
        // POST api/values
        [HttpPost]
        public string IncludeUser([FromBody]JObject value)
        {
            try
            {

                MySqlConnection conn = WebApiConfig.conn();

                MySqlCommand query = conn.CreateCommand();


                query.CommandText = "INSERT INTO `users` (`username`, `password`) VALUES (@name, @password);";

                query.Parameters.AddWithValue("@name", value["name"]);
                query.Parameters.AddWithValue("@password", value["password"]);

                conn.Open();

                query.ExecuteNonQuery();

                conn.Close();

                return "Usuario inserido";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        // PUT api/values/5
        [HttpPut]
        public string UpdateUser(int id, [FromBody]JObject value)
        {

            var results = this.GetUser(id);

            if (results.Count != 0)
            {
                try
                {
                    MySqlConnection conn = WebApiConfig.conn();

                    MySqlCommand query = conn.CreateCommand();


                    query.CommandText = "UPDATE `users` SET `username` = @name, `password` = @password WHERE id = @id;";

                    query.Parameters.AddWithValue("@name", value["name"]);
                    query.Parameters.AddWithValue("@password", value["password"]);
                    query.Parameters.AddWithValue("@id", id);

                    conn.Open();

                    query.ExecuteNonQuery();

                    conn.Close();

                    return "Usuario inserido";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else
            {
                return "Usuario nao encontrado";
            }
        }

        // DELETE api/values/5
        [HttpDelete]
        public string DeleteUser(int id)
        {
            var results = this.GetUser(id);

            if (results.Count != 0)
            {
                try
                {
                    MySqlConnection conn = WebApiConfig.conn();

                    MySqlCommand query = conn.CreateCommand();


                    query.CommandText = "DELETE FROM `users` WHERE id = @id;";

                    query.Parameters.AddWithValue("@id", id);

                    conn.Open();

                    query.ExecuteNonQuery();

                    conn.Close();

                    return "Usuario deletado";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else
            {
                return "Usuario nao encontrado";
            }
        }

        [HttpPost]
        public string Email([FromBody]JObject email)
        {
            MailMessage Mail = new MailMessage();
            Mail.To.Add(email["email"].ToString());

            // 
            MailAddress MailAdress = new MailAddress("email@email.com.br");

            Mail.From = MailAdress;
            Mail.Subject = "teste de envio.....";
            Mail.Body = "a mensagem foi enviado?";

            // SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            SmtpClient smtp = new SmtpClient("SMTP.office365.com", 587);
            smtp.EnableSsl = true;

            NetworkCredential credencial = new NetworkCredential("email@email.com.br", "senha");

            smtp.Credentials = credencial;

            try
            {
                smtp.Send(Mail);
                return "Email enviado";
            }
            catch (Exception ex)
            {
                return ""+ ex;
            }

            
        }

        [HttpPost]
        public string EmailFile([FromBody]JObject email)
        {
            MailMessage Mail = new MailMessage();
            Mail.To.Add(email["email"].ToString());

            MailAddress MailAdress = new MailAddress("email@email.com.br");

            Mail.From = MailAdress;
            Mail.Subject = "teste de envio.....";
            Mail.Body = "a mensagem foi enviado?";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;

            NetworkCredential credencial = new NetworkCredential("email@email.com.br", "senha");

            smtp.Credentials = credencial;

            string file = "C:/Users/giova/Desktop/Projetos/Documentos de ativação/Documento_Atri-Fiat.pdf";

            Attachment data = new Attachment(file, MediaTypeNames.Application.Pdf); 

            ContentDisposition disposition = data.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(file);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(file);

            Mail.Attachments.Add(data);

            try
            {
                smtp.Send(Mail);
                return "Email enviado";
            }
            catch (Exception ex)
            {
                return "" + ex;
            }
        }
    }
}
