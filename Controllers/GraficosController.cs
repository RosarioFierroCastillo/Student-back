using API_Archivo.Clases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;

namespace API_Archivo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraficosController : ControllerBase
    {

        [HttpGet]
        [Route("Consultar_DeudasPorCobrar")]

        public List<Graficos> Consultar_DeudasPorCobrar(int id_fraccionamiento, String fecha_inicio, String fecha_final)
        {

            List<Graficos> Graficos = new List<Graficos>();
            List<Graficos> Graficos1 = sum_variables(id_fraccionamiento, fecha_inicio, fecha_final);




            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                double suma = 0;
                int month = 0;
                float egresos = 0;

                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudores WHERE id_fraccionamiento=@id_fraccionamiento AND (dia_registro BETWEEN @fecha_inicio AND @fecha_final) AND estado='Pendiente'", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
                comando.Parameters.Add("@fecha_inicio", MySqlDbType.Date).Value = fecha_inicio;
                comando.Parameters.Add("@fecha_final", MySqlDbType.Date).Value = fecha_final;

                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {


                        suma += reader.GetFloat(9) + reader.GetFloat(8);
                    }


                    if (DateTime.TryParseExact(fecha_inicio, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime date))
                    {
                        // Obtener el número de mes del objeto DateTime
                        month = date.Month;

                    }


                    foreach (var item in Graficos1)
                    {

                        Graficos.Add(new Graficos()
                        {
                            cuentas_cobrar = suma,
                            sum_novariables = item.sum_novariables,
                            novariables = item.novariables,
                            sum_variables = item.sum_variables,
                            variables = item.variables,
                            por_variables = Math.Round(item.por_variables, 2),
                            por_novariables = Math.Round(item.por_novariables, 2),
                            proyeccion_ingresos = proyeccion(id_fraccionamiento, fecha_inicio, fecha_final),
                            mes = month,
                            egresos = egreso(id_fraccionamiento, fecha_inicio, fecha_final)

                        });


                    }
                }

                catch (MySqlException ex)
                {

                }
                finally
                {



                    conexion.Close();
                }

                return Graficos;
            }

        }

        [HttpGet]
        [Route("Consultar_Entradas")]
        public List<Entradas> Consultar_Entradas(String fecha_inicio, String fecha_final)
        {

            List<Entradas> Entradas = new List<Entradas>();



            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {


                MySqlCommand comando = new MySqlCommand("SELECT * FROM entradas WHERE (fecha BETWEEN @fecha_inicio AND @fecha_final);", conexion);
                comando.Parameters.Add("@fecha_inicio", MySqlDbType.Date).Value = fecha_inicio;
                comando.Parameters.Add("@fecha_final", MySqlDbType.Date).Value = fecha_final;

                // comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {

                        Entradas.Add(new Entradas()
                        {
                            id_entrada = reader.GetInt32(0),
                            fecha = reader.GetDateTime(2),
                            nombre = reader.GetString(1),

                        });
                    }

                }

                catch (MySqlException ex)
                {

                }
                finally
                {
                    conexion.Close();
                }

                return Entradas;
            }
        }

        static public List<Graficos> sum_variables(int id_fraccionamiento, String fecha_inicio, String fecha_final)
        {
            List<Graficos> Graficos = new List<Graficos>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int sum_variables = 0, sum_novariables = 0;
                double variables = 0, novariables = 0;

                MySqlCommand comando = new MySqlCommand("SELECT * FROM historial_deudaspagadas WHERE id_fraccionamiento=@id_fraccionamiento AND (dia_registro BETWEEN @fecha_inicio AND @fecha_final)", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
                comando.Parameters.Add("@fecha_inicio", MySqlDbType.Date).Value = fecha_inicio;
                comando.Parameters.Add("@fecha_final", MySqlDbType.Date).Value = fecha_final;
                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        //   string fechaProximoPago = (reader.GetDateTime(9)).ToString("yyyy-MM-ddTHH:mm:ss");


                        if (reader.GetString(6) == "ordinaria")
                        {
                            sum_novariables++;
                            novariables += reader.GetFloat(8) + reader.GetFloat(9);
                        }
                        else if (reader.GetString(6) == "extraordinaria")
                        {
                            sum_variables++;
                            variables += reader.GetFloat(8) + reader.GetFloat(9);
                        }



                    }


                    Graficos.Add(new Graficos()
                    {
                        sum_novariables = sum_novariables,
                        novariables = novariables,
                        sum_variables = sum_variables,
                        variables = variables,
                        por_variables = (variables / (variables + novariables)) * 100,
                        por_novariables = (novariables / (variables + novariables)) * 100,

                    });


                }
                catch (MySqlException ex)
                {

                }
                finally
                {


                    conexion.Close();
                }

                return Graficos;
            }
        }


        static public float proyeccion(int id_fraccionamiento, String fecha_inicio, String fecha_final)
        {
            float proyeccion_ingresos = 0;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {


                MySqlCommand comando = new MySqlCommand("SELECT SUM(monto + recargo) FROM deudores WHERE id_fraccionamiento=@id_fraccionamiento AND (dia_registro BETWEEN @fecha_inicio AND @fecha_final)", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
                comando.Parameters.Add("@fecha_inicio", MySqlDbType.Date).Value = fecha_inicio;
                comando.Parameters.Add("@fecha_final", MySqlDbType.Date).Value = fecha_final;
                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                       // proyeccion_ingresos = reader.GetFloat(0);

                    }



                }
                catch (MySqlException ex)
                {

                }
                finally
                {


                    conexion.Close();
                }

                return proyeccion_ingresos;
            }
        }


        static public float egreso(int id_fraccionamiento, String fecha_inicio, String fecha_final)
        {
            float egresos = 0;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {


                MySqlCommand comando = new MySqlCommand("SELECT SUM(monto) FROM egresos WHERE id_fraccionamiento=@id_fraccionamiento AND (fecha BETWEEN @fecha_inicio AND @fecha_final)", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
                comando.Parameters.Add("@fecha_inicio", MySqlDbType.Date).Value = fecha_inicio;
                comando.Parameters.Add("@fecha_final", MySqlDbType.Date).Value = fecha_final;
                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        egresos = reader.IsDBNull(0) ? 0 : reader.GetFloat(0);

                    }



                }
                catch (MySqlException ex)
                {

                }
                finally
                {


                    conexion.Close();
                }

                return egresos;
            }
        }
    }
}