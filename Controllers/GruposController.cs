using API_Archivo.Clases;
using CardManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;

namespace API_Archivo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GruposController : ControllerBase
    {

        [HttpPost]
        [Route("Agregar_Grupo")]

        public bool Agregar_Grupo([FromBody] Grupos request)
        {


            bool res = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
              

                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("insert into grupos (id_fraccionamiento, nombre, descripcion, usuarios) VALUES (@id_fraccionamiento, @nombre, @descripcion, @usuarios)", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = request.id_fraccionamiento;
                comando.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = request.nombre;
                comando.Parameters.Add("@descripcion", MySqlDbType.VarChar).Value = request.descripcion;
                comando.Parameters.Add("@usuarios", MySqlDbType.Int32).Value = request.usuarios;



                try
                {
                    conexion.Open();
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

                }

                return res;
            }
        }

    
        [HttpPost]
        [Route("Agregar_Persona_Grupo")]

        public bool Agregar_Persona_Grupo([FromBody] List<Persona> personas)
        {


            bool res = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                int rowsaffected = 0;
                var comando = new MySqlCommand();



                string cadena = "";


                comando = new MySqlCommand("insert into personaxgrupo (id_grupo, id_persona, nombre) VALUES ", conexion);


                for (int i = 0; i < personas.Count; i++)
                {
                    var persona = personas[i];
                    if (i > 0)
                    {
                        comando.CommandText += ", ";
                    }

                    comando.CommandText += $"(@id_grupo{i}, @id_persona{i}, @nombre{i})";

                    // Agregar parámetros para cada persona
                    comando.Parameters.AddWithValue($"@id_grupo{i}", persona.id_grupo);
                    comando.Parameters.AddWithValue($"@id_persona{i}", persona.id_persona);
                    comando.Parameters.AddWithValue($"@nombre{i}", persona.nombre + " " + persona.apellido_pat + " " + persona.apellido_mat);
                }




                /*
                comando.Parameters.Add("@id_grupo", MySqlDbType.Int32).Value = request.id_grupo;
                comando.Parameters.Add("@id_persona", MySqlDbType.Int32).Value = request.id_persona;
                comando.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = request.nombre + " " + request.apellido_pat + " " + request.apellido_mat;
                */
                try
                {
                    conexion.Open();
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

                }

                return res;
            }
        }
        

        [HttpGet]
        [Route("Consultar_Grupos")]

        public List<Grupos> Consultar_Grupos(int id_fraccionamiento)
        {


            List<Grupos> Lista_grupos = new List<Grupos>();



            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM grupos WHERE id_fraccionamiento=@id_fraccionamiento", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Lista_grupos.Add(new Grupos()
                        {
                            id_grupo = reader.GetInt32(0),
                            id_fraccionamiento = reader.GetInt32(1),
                            nombre = reader.GetString(2),
                            descripcion = reader.GetString(3),
                            usuarios = reader.GetInt32(4)

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

                return Lista_grupos;
            }

        }


        [HttpGet]
        [Route("Consultar_Miembros")]

        public List<Persona> Consultar_Miembros(int id_grupo)
        {


            List<Persona> Lista_personas = new List<Persona>();



            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM personaxgrupo WHERE id_grupo=@id_grupo", conexion);

                comando.Parameters.Add("@id_grupo", MySqlDbType.Int32).Value = id_grupo;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Lista_personas.Add(new Persona()
                        {
                            id = reader.GetInt32(0),
                            id_grupo = reader.GetInt32(1),
                            id_persona = reader.GetInt32(3),
                            nombre = reader.GetString(2)

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

                return Lista_personas;
            }

        }



        [HttpGet]
        [Route("Consultar_Persona")]

        public List<Persona> Consultar_Usuarios(int id_administrador)
        {

            List<Persona> Persona = new List<Persona>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM personas where id_administrador=@id_administrador", conexion);

                comando.Parameters.Add("@id_administrador", MySqlDbType.Int32).Value = id_administrador;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {



                        Persona.Add(new Persona()
                        {
                            id_persona = reader.GetInt32(0),
                            nombre = reader.GetString(1),
                            apellido_pat = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            apellido_mat = reader.IsDBNull(3) ? "" : reader.GetString(3)



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

                return Persona;
            }
        }



        [HttpDelete]
        [Route("Eliminar_Grupo")]
        public bool Eliminar_Grupo(int id_grupo)
        {
            bool Grupo_eliminado = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("DELETE FROM grupos WHERE id_grupo=@id_grupo", conexion);

                comando.Parameters.Add("@id_grupo", MySqlDbType.Int32).Value = id_grupo;



                //AddDevice.Login("admin", "Repara123", "5551", "187.216.118.73");
                //AddDevice.DeleteAllUser(id_persona.ToString());

                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Grupo_eliminado = true;
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
                return Grupo_eliminado;

            }
        }




        [HttpDelete]
        [Route("Eliminar_Miembro")]
        public bool Eliminar_Miembro(int id_persona)
        {
            bool Persona_eliminada = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("DELETE FROM personaxgrupo WHERE id=@id_persona", conexion);


                comando.Parameters.Add("@id_persona", MySqlDbType.Int32).Value = id_persona;



                //AddDevice.Login("admin", "Repara123", "5551", "187.216.118.73");
                //AddDevice.DeleteAllUser(id_persona.ToString());

                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Persona_eliminada = true;
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
                return Persona_eliminada;

            }
        }

    }
}
