using API_Archivo.Clases;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace API_Archivo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FraccionamientosController : ControllerBase
    {

        [HttpPost]
        [Route("Agregar_Fraccionamiento")]

        public bool Agregar_Fraccionamiento([FromBody] Fraccionamientos request)
        {

            bool fraccionamiento_agregado = false;
            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("insert into fraccionamientos (nombre_fraccionamiento, direccion, coordenadas, id_administrador, id_tesorero, dia_pago) VALUES (@nombre_fraccionamiento, @direccion, @coordenadas, @id_administrador, @id_tesorero, @dia_pago)", conexion);

                //Nombre_fraccionamiento=@Nombre_fraccionamiento, Direccion=@Direccion, Coordenadas=@Coordenadas, id_administrador=@id_administrador, id_tesorero=@id_tesorero)

                comando.Parameters.Add("@nombre_fraccionamiento", MySqlDbType.VarChar).Value = request.nombre;
                comando.Parameters.Add("@direccion", MySqlDbType.VarChar).Value = request.direccion;
                comando.Parameters.Add("@coordenadas", MySqlDbType.VarChar).Value = request.coordenadas;
                comando.Parameters.Add("@id_administrador", MySqlDbType.Int32).Value = request.id_administrador;
                comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = request.id_tesorero;
                comando.Parameters.Add("@dia_pago", MySqlDbType.Int32).Value = request.dia_pago;


                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        fraccionamiento_agregado = true;
                    }

                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
                finally
                {

                }

                return fraccionamiento_agregado;
            }
        }


        [HttpDelete]
        [Route("Eliminar_Fraccionamiento")]

        public bool Eliminar_Fraccionamiento(int id_fraccionamiento)
        {
            bool eliminar_fraccionamiento = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("DELETE FROM fraccionamientos WHERE id_fraccionamiento=@id_fraccionamiento", conexion);

                //@id_usuario, @Tipo_deuda,@Nombre_deuda, @Monto, @Ruta_comprobante, @Estado

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;



                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        eliminar_fraccionamiento = true;
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
                return eliminar_fraccionamiento;

            }
        }

        [HttpPut]
        [Route("Actualizar_Fraccionamiento")]

        public bool Actualizar_Fraccionamiento([FromBody] Fraccionamientos request)
        {

                bool Fraccionamiento_actualizado = false;

                using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
                {
                    int rowsaffected = 0;
                    MySqlCommand comando = new MySqlCommand("UPDATE fraccionamientos " +
                        "SET nombre_fraccionamiento=@nombre_fraccionamiento, direccion=@direccion, coordenadas=@coordenadas, id_administrador=@id_administrador, id_tesorero=@id_tesorero, dia_pago=@dia_pago " +
                        "WHERE id_fraccionamiento=@id_fraccionamiento", conexion);


                    comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = request.id_fraccionamiento;
                    comando.Parameters.Add("@nombre_fraccionamiento", MySqlDbType.VarChar).Value = request.nombre;
                    comando.Parameters.Add("@direccion", MySqlDbType.VarChar).Value = request.direccion;
                    comando.Parameters.Add("@coordenadas", MySqlDbType.VarChar).Value = request.coordenadas;
                    comando.Parameters.Add("@id_administrador", MySqlDbType.Int32).Value = request.id_administrador;
                    comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = request.id_tesorero;
                    comando.Parameters.Add("@dia_pago", MySqlDbType.Int32).Value = request.dia_pago;


                try
                {
                        conexion.Open();
                        rowsaffected = comando.ExecuteNonQuery();

                        if (rowsaffected >= 1)
                        {
                            Fraccionamiento_actualizado = true;
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
                    return Fraccionamiento_actualizado;

                }
            
        }


        [HttpGet]
        [Route("Consultar_Fraccionamiento")]

        public List<Fraccionamientos> Consultar_Fraccionamiento(int id_administrador)
        {

            List<Fraccionamientos> Lista_acuerdos = new List<Fraccionamientos>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM fraccionamientos WHERE id_administrador=@id_administrador", conexion);

                comando.Parameters.Add("@id_administrador", MySqlDbType.Int32).Value = id_administrador;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Lista_acuerdos.Add(new Fraccionamientos() { 
                            id_fraccionamiento = reader.GetInt32(0), 
                            nombre = reader.GetString(1), 
                            direccion = reader.GetString(2), 
                            coordenadas = reader.GetString(3),
                            id_administrador = reader.GetInt32(4),
                            id_tesorero = reader.GetInt32(5),
                            dia_pago = reader.GetInt32(6) });
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


        [HttpPut]
        [Route("Actualizar_Tesorero")]

        public bool Actualizar_Tesorero([FromBody] Fraccionamientos request)
        {

            bool Fraccionamiento_actualizad = false;
            //en fraccionamiento
            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("UPDATE fraccionamientos " +
                    "SET id_tesorero=@id_tesorero " +
                    "WHERE id_fraccionamiento=@id_fraccionamiento", conexion);


                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = request.id_fraccionamiento;
                comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = request.id_tesorero;


                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Fraccionamiento_actualizad = true;
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
                return Fraccionamiento_actualizad;

            }



        }


    }
}
