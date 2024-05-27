/*
using API_Archivo.Clases;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;

namespace API_Archivo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SesionController
    {

        [HttpGet]
        [Route("Iniciar_Sesion")]


        public List<Sesion> Iniciar_Sesion(string correo, string contrasenia)
        {
            List<Sesion> list_sesion = new List<Sesion>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("SELECT * from personas WHERE Correo = @Correo && Contrasenia = @Contrasenia", conexion);

                //@id_fraccionamiento, @Nombre_deuda, @Descripción, @Monto, @Fecha_corte, @Periodicidad_dias

                comando.Parameters.Add("@Correo", MySqlDbType.VarChar).Value = correo;
                comando.Parameters.Add("@Contrasenia", MySqlDbType.VarChar).Value = contrasenia;


                try
                {
                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    //    if(AddDevice.Login("admin", "Repara123", "5551", "187.216.118.73") == true)
                    //    {
                    while (reader.Read())
                    {
                        list_sesion.Add(new Sesion() { correo = reader.GetString(1), id_usuario = reader.GetInt32(0), tipo_usuario = reader.GetString(13) });
                        // AddDevice.Login("admin", "Repara123", "5551", "187.216.118.73");
                    }

                    //    }


                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
                finally
                {
                    conexion.Close();
                }
                return list_sesion;
            }

        }


    }
}
*/


using API_Archivo.Clases;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;

namespace API_Archivo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SesionController
    {

        [HttpGet]
        [Route("Iniciar_Sesion")]


        public List<Sesion> Iniciar_Sesion(string correo, string contrasenia)
        {
            List<Sesion> list_sesion = new List<Sesion>();
            string frac;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("SELECT * from personas WHERE correo = @correo && contrasenia = @contrasenia", conexion);

                //@id_fraccionamiento, @Nombre_deuda, @Descripción, @Monto, @Fecha_corte, @Periodicidad_dias

                comando.Parameters.Add("@correo", MySqlDbType.VarChar).Value = correo;
                comando.Parameters.Add("@contrasenia", MySqlDbType.VarChar).Value = contrasenia;


                try
                {
                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    //    if(AddDevice.Login("admin", "Repara123", "5551", "187.216.118.73") == true)
                    //    {
                    while (reader.Read())
                    {
                        Obtener_Tesorero(reader.GetInt32(0));
                        /*
                        if(reader.GetString(9)=="" || reader.GetString(9) == null || reader.GetString(9) == " ")
                        {
                             frac = "n";
                        }
                        else
                        {
                            frac = reader.GetString(9);
                        }
                        */
                        list_sesion.Add(new Sesion() {
                            id_usuario = reader.GetInt32(0),
                            correo = reader.GetString(1),  
                            tipo_usuario = reader.GetString(13), 
                            id_fraccionamiento = reader.IsDBNull(6) ? 0 : reader.GetInt32(6), 
                            id_lote = reader.IsDBNull(7) ? 0 : reader.GetInt32(7), 
                            fraccionamiento = reader.IsDBNull(9) ? "" : reader.GetString(9) ,
                            id_tesorero = Obtener_Tesorero(reader.GetInt32(0))
                          });
                        // AddDevice.Login("admin", "Repara123", "5551", "187.216.118.73");
                    }

                    //    }


                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
                finally
                {
                    conexion.Close();
                }

                //Deudas obj_deudas = new Deudas();

                //obj_deudas.AsignarDeudasAUltimaPersona(list_sesion[0].id_fraccionamiento);
                return list_sesion;
            }

        }




        [HttpGet]
        [Route("Obtener_Tesorero")]
        public int Obtener_Tesorero(int id_administrador)
        {
            int tesorero = 0;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT id_persona FROM personas WHERE id_fraccionamiento=@id_administrador && tipo_usuario='tesorero'", conexion);

                comando.Parameters.Add("@id_administrador", MySqlDbType.Int32).Value = id_administrador;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        tesorero = reader.GetInt32(0);
                    }


                }
                catch (MySqlException ex)
                {

                }
                finally
                {
                    conexion.Close();
                }

                return tesorero;
            }
        }
    }


}
