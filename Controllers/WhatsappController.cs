using API_Archivo.Clases;
using CardManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;


namespace API_Archivo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsappController : ControllerBase
    {

        [HttpGet]
        [Route("Generar_Token")]
        public IActionResult GenerarToken(int idUsuario)
        {
            var token_generado = Guid.NewGuid().ToString();// Generar un token aleatorio utilizando Guid

            string idFraccionamiento = "";
            string tipoUsuario = "";
            bool tokenAgregado = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("insert into tokens (token,idUsuario, estatus) VALUES (@token,@idUsuario, @estatus)", conexion);
                MySqlCommand comando2 = new MySqlCommand("select id_fraccionamiento,tipo_usuario from personas where id_persona=@idUsuario", conexion);

                comando.Parameters.Add("@idUsuario", MySqlDbType.Int32).Value = idUsuario;
                comando2.Parameters.Add("@idUsuario", MySqlDbType.Int32).Value = idUsuario;

                try
                {
                    conexion.Open();

                    MySqlDataReader reader = comando2.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        tipoUsuario += reader.GetString(1);
                        if (tipoUsuario == "administrador")
                        {
                            idFraccionamiento = idUsuario.ToString();
                        }else{ 
                            idFraccionamiento = reader.GetString(0);
                        }
                        
                    }


                }catch(MySqlException ex)
                {

                }
                finally
                {
                    conexion.Close();
                }

                comando.Parameters.Add("@token", MySqlDbType.VarChar).Value = token_generado+idFraccionamiento;
                comando.Parameters.Add("@estatus", MySqlDbType.VarChar).Value = "Disponible";
                


                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        tokenAgregado = true;
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
            }
            if (tokenAgregado)
            {
                return new ContentResult
                {
                    Content = token_generado,
                    ContentType = "text/plain",
                    StatusCode = 200 // Código de estado OK (200)
                };
            }
            else
            {
                return new ContentResult
                {
                    Content = "error",
                    ContentType = "text/plain",
                    StatusCode = 400 // Código de estado OK (200)
                };
            }

        }

        [HttpGet]
        [Route("Consultar_Token_Activo")]
        public IActionResult Consultar_Token_Activo(int idUsuario)
        {
            String token = "";
            bool tokenExiste = false;
            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM tokens WHERE idUsuario=@idUsuario", conexion);

                comando.Parameters.Add("@idUsuario", MySqlDbType.VarChar).Value = idUsuario;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        token = reader.GetString(2);
                    }


                }
                catch (MySqlException ex)
                {

                }
                finally
                {
                    conexion.Close();
                }

            }
            if (token != "")
            {
                return new ContentResult
                {
                    Content = token,
                    ContentType = "text/plain",
                    StatusCode = 200 // Código de estado OK (200)
                };
            }
            else
            {
                return new ContentResult
                {
                    Content = "error",
                    ContentType = "text/plain",
                    StatusCode = 400 // Código de estado OK (200)
                };
            }

        }

        [HttpDelete]
        [Route("Borrar_Token")]
        public IActionResult Borrar_token(string token)
        {
            bool tokenEliminado = false;

            int idFraccionamiento = Int32.Parse(token.Substring(token.Length - 2));

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("DELETE FROM tokens WHERE token=@token", conexion);

                //@id_usuario, @Tipo_deuda,@Nombre_deuda, @Monto, @Ruta_comprobante, @Estado

                comando.Parameters.Add("@token", MySqlDbType.VarChar).Value = token;



                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        tokenEliminado = true;
                        bool login = AddDevice.Login(idFraccionamiento);
                        bool puerta = AddDevice.btnOpen_Click();

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

                if (tokenEliminado)
                {
                    return new ContentResult
                    {
                        Content = "ok",
                        ContentType = "text/plain",
                        StatusCode = 200 // Código de estado OK (200)
                    };
                }
                else
                {
                    return new ContentResult
                    {
                        Content = "error",
                        ContentType = "text/plain",
                        StatusCode = 400 // Código de estado OK (200)
                    };
                }

            }
        }
    }
}