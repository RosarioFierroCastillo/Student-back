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
using CardManagement;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Bcpg;
using System.Data;

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
                DataTable dt = new DataTable();

              //  string ip="", port="", user = "", password = "";

                MySqlCommand comando = new MySqlCommand("sp_login", conexion);

                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@correo", correo);
                comando.Parameters.AddWithValue("@pass", contrasenia);

                try
                {
                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();
                     
                    dt.Load(reader);


                    int tesorero = 0;
                    bool var = false;

                    foreach (DataRow row in dt.Rows)
                    {
                        int id_fraccionamiento = row.Field<int>("id_persona");

                        //   tesorero = Obtener_Tesorero(row.Field<int>("id_persona"));
                        // var = AddDevice.Login(id_fraccionamiento);

                        var = AddDevice.Login(row.Field<int>("id_fraccionamiento")); //esta es la que estaba descomentada


                        list_sesion.Add(new Sesion()
                        {
                            id_usuario = row.Field<int>("id_persona"),
                            correo = row.Field<string>("correo"),
                            tipo_usuario = row.Field<string>("tipo_usuario"),
                            id_fraccionamiento = row.IsNull("id_fraccionamiento") ? 0 : row.Field<int>("id_fraccionamiento"),
                            id_lote = row.IsNull("id_lote") ? 0 : row.Field<int>("id_lote"),
                            fraccionamiento = row.IsNull("codigo_acceso") ? "" : row.Field<string>("codigo_acceso"),
                            id_tesorero = row.IsNull("id_tesorero") ? 0 : row.Field<int>("id_tesorero"),
                            nombre = row.Field<string>("nombre"),
                            con_nombre = row.Field<string>("con_nombre"),

                            ip = row.Field<string>("ip"),
                            port = row.Field<string>("port"),
                            password = row.Field<string>("password"),
                            user = row.Field<string>("user"),


                            conexion = var,
                            //dark_mode = row.Field<int>("dark_mode")
                            dark_mode = (row.Field<int>("dark_mode") == 1) ? true : false,

                            client_key = row.IsNull("client_key") ? "" : row.Field<string>("client_key"),
                            secret_key = row.IsNull("secret_key") ? "" : row.Field<string>("secret_key"),
                            hikvision = row.IsNull("hikvision") ? "" : row.Field<string>("hikvision"),


                        });

                        /*
                        ip = row.Field<string>("ip");
                        port = row.Field<string>("port");
                        password = row.Field<string>("password");
                        user = row.Field<string>("user");
                        */

                    }


                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
                finally
                {
                    conexion.Close();
                  //  AddDevice.Login(user, password, port, ip);
                }

                return list_sesion;
            }

        }


        /*
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

        [HttpGet]
        [Route("Conexion_Hikvision")]

        public bool Conexion_Hikvision(int id_fraccionamiento)
        {

            bool var = AddDevice.Login(id_fraccionamiento);

            return var;
        }

        */
        [HttpGet]
        [Route("Actualizar_apariencia")]
        public bool Actualizar_apariencia(int id_persona, int dark_mode)
        {

            bool var = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;

                MySqlCommand comando = new MySqlCommand("UPDATE personas SET dark_mode = @dark_mode WHERE id_persona = @id_persona;", conexion);

                comando.Parameters.Add("@id_persona", MySqlDbType.Int32).Value = id_persona;
                comando.Parameters.Add("@dark_mode", MySqlDbType.Int32).Value = dark_mode;


                try
                {

                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        var = true;
                    }

                }
                catch (MySqlException ex)
                {

                }
                finally
                {
                    conexion.Close();
                }

                return var;
            }
        }

    }


}
