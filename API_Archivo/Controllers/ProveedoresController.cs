using API_Archivo.Clases;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace API_Archivo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProveedoresController :ControllerBase
    {

        [HttpPost]
        [Route("Agregar_Proveedor")]


        public bool Agregar_Proveedor(int id_fraccionamiento, string nombre, string apellido_paterno, string apellido_materno, string telefono, string tipo, string direccion, string funcion)
        {
            bool Proveedor_agregado = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("insert into proveedores (id_fraccionamiento,Nombre, Apellido_paterno, Apellido_materno, Telefono, Tipo, Direccion, Funcion) VALUES (@id_fraccionamiento, @Nombre, @Apellido_paterno, @Apellido_materno, @Telefono, @Tipo, @Direccion, @Funcion)", conexion);

                //@Nombre, @Apellido_paterno, @Apellido_materno, @Telefono, @Tipo, @Direccion, @Funcion

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
                comando.Parameters.Add("@Nombre", MySqlDbType.VarChar).Value = nombre;
                comando.Parameters.Add("@Apellido_paterno", MySqlDbType.VarChar).Value = apellido_paterno;
                comando.Parameters.Add("@Apellido_materno", MySqlDbType.VarChar).Value = apellido_materno;
                comando.Parameters.Add("@Telefono", MySqlDbType.VarChar).Value = telefono;
                comando.Parameters.Add("@Tipo", MySqlDbType.VarChar).Value = tipo;
                comando.Parameters.Add("@Direccion", MySqlDbType.VarChar).Value = direccion;
                comando.Parameters.Add("@Funcion", MySqlDbType.VarChar).Value = funcion;




                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Proveedor_agregado = true;
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
                return Proveedor_agregado;
            }
        }

        [HttpDelete]
        [Route("Eliminar_Proveedor")]

        public bool Eliminar_Proveedor(int id_fraccionamiento, int id_proveedor)
        {
            bool Proveedor_eliminado = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("DELETE FROM proveedores WHERE id_Proveedor=@id_proveedor && id_fraccionamiento = @id_fraccionamiento", conexion);



                comando.Parameters.Add("@id_proveedor", MySqlDbType.Int32).Value = id_proveedor;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;



                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Proveedor_eliminado = true;
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
                return Proveedor_eliminado;

            }
        }


        [HttpPatch]
        [Route("Actualizar_Proveedor")]

        public bool Actualizar_Proveedor(int id_proveedor, int id_fraccionamiento, string nombre, string apellido_paterno, string apellido_materno, string telefono, string tipo, string direccion, string funcion)
        {
            bool Proveedor_actualizado = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("UPDATE proveedores " +
                    "SET id_fraccionamiento=@id_fraccionamiento, Nombre=@Nombre, Apellido_paterno=@Apellido_paterno, Apellido_materno=@Apellido_materno, Telefono=@Telefono, Tipo=@Tipo, Direccion=@Direccion, Funcion=@Funcion " +
                    "WHERE id_Proveedor=@id_proveedor", conexion);
                //id_fraccionamiento=@id_fraccionamiento, Tipo=@Tipo, Destinatario=@Destinatario, Asunto=@Asunto, Mensaje=@Mensaje

                comando.Parameters.Add("@id_proveedor", MySqlDbType.Int32).Value = id_proveedor;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
                comando.Parameters.Add("@Nombre", MySqlDbType.VarChar).Value = nombre;
                comando.Parameters.Add("@Apellido_paterno", MySqlDbType.VarChar).Value = apellido_paterno;
                comando.Parameters.Add("@Apellido_materno", MySqlDbType.VarChar).Value = apellido_materno;
                comando.Parameters.Add("@Telefono", MySqlDbType.VarChar).Value = telefono;
                comando.Parameters.Add("@Tipo", MySqlDbType.VarChar).Value = tipo;
                comando.Parameters.Add("@Direccion", MySqlDbType.VarChar).Value = direccion;
                comando.Parameters.Add("@Funcion", MySqlDbType.VarChar).Value = funcion;



                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Proveedor_actualizado = true;
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
                return Proveedor_actualizado;
            }
        }


        [HttpGet]
        [Route("Consultar_Proveedor")]


        public List<Proveedores> Consultar_Proveedores(int id_fraccionamiento)
        {
            List<Proveedores> Lista_proveedores = new List<Proveedores>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM proveedores WHERE id_fraccionamiento=@id_fraccionamiento", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Lista_proveedores.Add(new Proveedores() { id_proveedor = reader.GetInt32(0), id_fraccionamiento = reader.GetInt32(1), nombre = reader.GetString(2), apellido_paterno = reader.GetString(3), apellido_materno = reader.GetString(4), telefono = reader.GetString(5), tipo = reader.GetString(6), direccion = reader.GetString(7), funcion = reader.GetString(8) });
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

                return Lista_proveedores;
            }
        }


    }
}
