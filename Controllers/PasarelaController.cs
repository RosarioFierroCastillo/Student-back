using API_Archivo.Clases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Security.Policy;

namespace API_Archivo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasarelaController : ControllerBase
    {
        [HttpPost]
        [Route("Agregar_Pasarela")]

        public bool Agregar_Pasarela(int id_fraccionamiento, string url, string client_key, string secret_key)
        {
            bool pasarela_agregada = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("insert into pasarela_pagos (id_fraccionamiento, modo, url, client_key, secret_key) VALUES (@id_fraccionamiento, 'sandbox', @url, @client_key, @secret_key)", conexion);


                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
               // comando.Parameters.Add("@modo", MySqlDbType.VarChar).Value = modo;
                comando.Parameters.Add("@url", MySqlDbType.VarChar).Value = url;
                comando.Parameters.Add("@client_key", MySqlDbType.VarChar).Value = client_key;
                comando.Parameters.Add("@secret_key", MySqlDbType.VarChar).Value = secret_key;





                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        pasarela_agregada = true;
                    }

                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
                finally
                {
                    conexion.Close();
                }
                return pasarela_agregada;
            }

        }


        [HttpGet]
        [Route("Consultar_Pasarela")]

        public List<Pasarela> Consultar_Pasarela(int id_fraccionamiento)
        {

            List<Pasarela> Lista_pasarela = new List<Pasarela>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM pasarela_pagos WHERE id_fraccionamiento=@id_fraccionamiento", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Lista_pasarela.Add(new Pasarela()
                        {
                            id_fraccionamiento = reader.GetInt32(1),
                            modo = reader.GetString(2),
                            url = reader.GetString(3),
                            client_key = reader.GetString(4),
                            secret_key = reader.GetString(5),

                        });
                        // MessageBox.Show();
                    }


                }
                catch (MySqlException ex)
                {

                }
                finally
                {
                    conexion.Close();
                }

                return Lista_pasarela;
            }

        }


        [HttpPut]
        [Route("Actualizar_Pasarela")]


        public bool Actualizar_Pasarela([FromBody] Pasarela request)
        {
            bool Pasarela_actualizada = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("UPDATE pasarela_pagos SET url=@url, client_key=@client_key, secret_key=@secret_key WHERE id_fraccionamiento=@id_fraccionamiento", conexion);



                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = request.id_fraccionamiento;
                // comando.Parameters.Add("@modo", MySqlDbType.VarChar).Value = modo;
                comando.Parameters.Add("@url", MySqlDbType.VarChar).Value = request.url;
                comando.Parameters.Add("@client_key", MySqlDbType.VarChar).Value = request.client_key;
                comando.Parameters.Add("@secret_key", MySqlDbType.VarChar).Value = request.secret_key;

                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Pasarela_actualizada = true;
                    }

                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
                finally
                {
                    conexion.Close();
                }
                return Pasarela_actualizada;

            }


        }

    }
}
