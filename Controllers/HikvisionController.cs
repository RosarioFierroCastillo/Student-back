using API_Archivo.Clases;
using CardManagement;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace API_Archivo.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class HikvisionController
    {
        private MySqlCommand comando;

        [HttpPost]
        [Route("Login")]


        public bool Login(int id_fraccionamiento)
        {
            bool puerta = AddDevice.Login(id_fraccionamiento);

            return puerta;

        }

        [HttpPost]
        [Route("Agregar_Hikvision")]

        public bool InsertHikvision(string nombre, int id_fraccionamiento, string user, string password, string port, string ip)
        {
            bool res = false;

           // bool res = AddDevice.Login(id_fraccionamiento);
            // bool res = AddDevice.Login("admin", "Repara123", "5551", "187.216.118.73");

         //   if (res == true)
            //{
                using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
                {
                    //  res = false;
                    int rowsaffected = 0;

       

                        conexion.Open();
                        res = false;
                        comando = new MySqlCommand("insert into controlador (id_fraccionamiento, nombre, user, password, port, ip) values(@id_fraccionamiento, @nombre, @user, @password, @port, @ip);", conexion);

                    //    comando.Parameters.Add("@id_controlador", MySqlDbType.Int32).Value = id_controlador;
                        comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
                        comando.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = nombre;
                        comando.Parameters.Add("@user", MySqlDbType.VarChar).Value = user;
                        comando.Parameters.Add("@password", MySqlDbType.VarChar).Value = password;
                        comando.Parameters.Add("@port", MySqlDbType.VarChar).Value = port;
                        comando.Parameters.Add("@ip", MySqlDbType.VarChar).Value = ip;


                        try
                        {
                            //  conexion.Open();
                            rowsaffected = comando.ExecuteNonQuery();

                            if (rowsaffected >= 1)
                            {
                                res = true;
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


               

//            }

            return res;
        }


        [HttpGet]
        [Route("Consultar_Hikvision")]

        public List<Hikvision> Consultar_Hikvision(int id_fraccionamiento)
        {

            List<Hikvision> Lista_acuerdos = new List<Hikvision>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM controlador WHERE id_fraccionamiento=@id_fraccionamiento ORDER BY CASE WHEN estado = 'habilitado' THEN 0 ELSE 1 END", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Lista_acuerdos.Add(new Hikvision()
                        {
                            id_controlador = reader.GetInt32(0),
                            nombre = reader.GetString(7),
                            user = reader.GetString(2),
                            password = reader.GetString(3),
                            port = reader.GetString(4),
                            ip = reader.GetString(5)

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

                return Lista_acuerdos;
            }


        }



        [HttpDelete]
        [Route("Eliminar_Hikvision")]
        public bool Eliminar_Hikvision (int id_controlador)
        {
            bool Controlador_eliminado = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("DELETE FROM controlador WHERE id_controlador=@id_controlador", conexion);

                comando.Parameters.Add("@id_controlador", MySqlDbType.Int32).Value = id_controlador;

                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Controlador_eliminado = true;
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
                return Controlador_eliminado;

            }
        }


        [HttpPut]
        [Route("Actualizar_Hikvision")]
        public bool Actualizar_Hikvision(int id_controlador,string nombre, string user, string password, string port, string ip)
        {

            bool Controlador_actualizado = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("UPDATE controlador SET nombre=@nombre, user=@user, password=@password, port=@port, ip=@ip WHERE id_controlador=@id_controlador", conexion);

                comando.Parameters.Add("@id_controlador", MySqlDbType.Int32).Value = id_controlador;
                comando.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = nombre;
                comando.Parameters.Add("@user", MySqlDbType.VarChar).Value = user;
                comando.Parameters.Add("@password", MySqlDbType.VarChar).Value = password;
                comando.Parameters.Add("@port", MySqlDbType.VarChar).Value = port;
                comando.Parameters.Add("@ip", MySqlDbType.VarChar).Value = ip;


                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Controlador_actualizado = true;
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
                return Controlador_actualizado;


            }
        }

        [HttpPut]
        [Route("Actualizar_Estado")]
        public bool Actualizar_Estado(int id_controlador, int id_fraccionamiento)
        {

            bool Controlador_actualizado = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("UPDATE controlador SET estado='deshabilitado' WHERE estado='habilitado' AND id_fraccionamiento=@id_fraccionamiento; UPDATE controlador SET estado='habilitado' WHERE id_controlador=@id_controlador;", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
                comando.Parameters.Add("@id_controlador", MySqlDbType.Int32).Value = id_controlador;

                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Controlador_actualizado = true;
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
                return Controlador_actualizado;


            }
        }






        [HttpPost]
        [Route("Abrir_puerta")]


        public bool activar_puerta()
        {
            bool puerta = AddDevice.btnOpen_Click();

            return puerta;

        }



    }
    }

