using API_Archivo.Clases;
using API_Archivo.Deudores;
using AppO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using PdfSharp.Drawing;
using PdfSharp.UniversalAccessibility.Drawing;
using System.Data;
using System.Threading;

namespace API_Archivo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeudasController : ControllerBase
    {
        //AQUÍ VAN TODAS LAS DEUDAS
        [HttpPost]
        [Route("Agregar_Deuda")]

        public bool Agregar_Deuda([FromBody] Deudas request)
         {

            bool fraccionamiento_agregado = false;
            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("insert into deudas " +
                    "(id_deudas, id_fraccionamiento, id_tesorero, monto, nombre, descripcion," +
                    "dias_gracia, periodicidad, recargo, proximo_pago, deudor) " +
                    "VALUES (@id_deudas, @id_fraccionamiento, @id_tesorero, @monto, @nombre, @descripcion, @dias_gracia, " + 
                    "@periodicidad, @recargo, @proximo_pago, @deudor)", conexion);

                DateTime now = DateTime.Now;
                DateTime Dateproximo_pago = request.proximo_pago;


                comando.Parameters.Add("@id_deudas", MySqlDbType.Int32).Value = request.id_deudas;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = request.id_fraccionamiento;
                comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = request.id_tesorero;
                comando.Parameters.Add("@monto", MySqlDbType.Float).Value = request.monto;
                comando.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = request.nombre;
                comando.Parameters.Add("@descripcion", MySqlDbType.VarChar).Value = request.descripcion;
                comando.Parameters.Add("@dias_gracia", MySqlDbType.Int32).Value = request.dias_gracia;
                comando.Parameters.Add("@periodicidad", MySqlDbType.Int32).Value = request.periodicidad;
                comando.Parameters.Add("@recargo", MySqlDbType.Float).Value = request.recargo;
                comando.Parameters.Add("@proximo_pago", MySqlDbType.Date).Value = Dateproximo_pago;
                comando.Parameters.Add("@deudor", MySqlDbType.Int32).Value = request.deudor;

                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        fraccionamiento_agregado = true;

                        DateTime fechaActual = DateTime.Now;
                        string fechaString = fechaActual.ToString("yyyy-MM-dd HH:mm"); // Formato: "2024-11-04 14:30
                        NotificacionesController obj_notificaciones = new NotificacionesController();
                        obj_notificaciones.Agregar_Notificacion(request.id_fraccionamiento, "General", 0, "!Nueva deuda :(!", $"El administrador de tu comunidad ha agregado una nueva deuda por el concepto de '{request.nombre.ToLower()}' por un monto de ${request.monto} cada {request.periodicidad} días", fechaString);
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



                Deudas obj_deudas = new Deudas();
                if (fraccionamiento_agregado)
                {
                    if (obj_deudas.AsignarDeudaNuevaATodos(request.id_fraccionamiento, request.deudor))
                    {
                        fraccionamiento_agregado = true;
                    }
                    else
                    {
                        fraccionamiento_agregado = false;
                    }
                }


                return fraccionamiento_agregado;

            }


        }
        /*
        [HttpGet]
        [Route("Consultar_Deuda")]

        public List<Deudas> Consultar_DeudasOrdinarias(int id_tesorero)
        {

            List<Deudas> Deuda = new List<Deudas>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudas WHERE id_tesorero=@id_tesorero && periodicidad>0", conexion);

                comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = id_tesorero;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        //   string fechaProximoPago = (reader.GetDateTime(9)).ToString("yyyy-MM-ddTHH:mm:ss");

                        Deuda.Add(new Deudas()
                        {
                            id_deudas = reader.GetInt32(0),
                            monto = reader.GetFloat(3),
                            nombre = reader.GetString(4),
                            descripcion = reader.GetString(5),
                            dias_gracia = reader.GetInt32(6),
                            periodicidad = reader.GetInt32(7),
                            recargo = reader.GetFloat(8),
                            proximo_pago1 = (reader.GetDateTime(9)).ToString("yyyy-MM-dd")

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

                return Deuda;
            }

        }

        */


        [HttpGet]
        [Route("GenerarRecibo")]
        public IActionResult GenerarRecibo(int id_deuda)
        {
            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                string consulta = "SELECT * FROM deudores WHERE id=@id_deuda";
                MySqlCommand comando = new MySqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@id_deuda", id_deuda);

                conexion.Open();

                using (MySqlDataReader reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Crear documento PDF
                        PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
                        PdfSharp.Pdf.PdfPage page = document.AddPage();
                        XGraphics gfx = XGraphics.FromPdfPage(page);
                        double yPos = 40; // Posición inicial

                        // Logo de la empresa
                        XImage logo = XImage.FromFile("assets/disenio.png");
                        gfx.DrawImage(logo, 20, yPos, 100, 50);

                        // Título y nombre de la empresa
                        XFont fontTitulo = new XFont("Arial", 18);
                        XFont fontEmpresa = new XFont("Arial", 12);
                        XFont fontContacto = new XFont("Arial", 10);

                        gfx.DrawString("Recibo de pago", fontTitulo, XBrushes.Black,
                            new XRect(0, yPos, page.Width, 50), XStringFormats.TopCenter);

                        yPos += 60;
                        gfx.DrawString("Comunidad: ", fontEmpresa, XBrushes.Black, 20, yPos);

                        yPos += 20;
                        gfx.DrawString("Contacto: contacto@student.com", fontContacto, XBrushes.Black, 20, yPos);
                        yPos += 15;
                        gfx.DrawString("Teléfono: (555) 123-4567", fontContacto, XBrushes.Black, 20, yPos);

                        yPos += 30;
                        gfx.DrawLine(XPens.Black, 20, yPos, page.Width - 20, yPos);

                        // Información del pago
                        yPos += 30;
                        gfx.DrawString($"Nombre del deudor:", fontEmpresa, XBrushes.Black, 20, yPos);
                        gfx.DrawString(reader.GetString(4), fontEmpresa, XBrushes.Black, 150, yPos);

                        yPos += 20;
                        gfx.DrawString($"Deuda no.:", fontEmpresa, XBrushes.Black, 20, yPos);
                        gfx.DrawString((reader.GetInt32(2)).ToString(), fontEmpresa, XBrushes.Black, 150, yPos);

                        yPos += 20;
                        gfx.DrawString($"Fecha de pago:", fontEmpresa, XBrushes.Black, 20, yPos);
                        gfx.DrawString(Convert.ToDateTime(reader.GetDateTime(14)).ToString("dd/MM/yyyy"), fontEmpresa, XBrushes.Black, 150, yPos);

                        yPos += 20;
                        gfx.DrawString($"Estado de la deuda:", fontEmpresa, XBrushes.Black, 20, yPos);
                        gfx.DrawString(reader.GetString(12), fontEmpresa, XBrushes.Black, 150, yPos);

                        yPos += 40;
                        gfx.DrawLine(XPens.Gray, 20, yPos, page.Width - 20, yPos);
                        yPos += 10;

                        XFont fontResumen = new XFont("Arial", 14);
                        gfx.DrawString("Resúmen de pago", fontResumen, XBrushes.Black,
                            new XRect(0, yPos, page.Width, 20), XStringFormats.TopCenter);

                        yPos += 30;
                        gfx.DrawString("Monto total:", fontEmpresa, XBrushes.Black, 20, yPos);
                        gfx.DrawString($"${Convert.ToDouble(reader.GetFloat(8)).ToString("F2")}", fontEmpresa, XBrushes.Black, page.Width - 150, yPos);

                        yPos += 20;
                        gfx.DrawString("Recargos:", fontEmpresa, XBrushes.Black, 20, yPos);
                        gfx.DrawString($"${Convert.ToDouble(reader.GetFloat(9)).ToString("F2")}", fontEmpresa, XBrushes.Black, page.Width - 150, yPos);

                        yPos += 20;
                        gfx.DrawString("Días de gracia:", fontEmpresa, XBrushes.Black, 20, yPos);
                        gfx.DrawString((reader.GetInt32(10)).ToString(), fontEmpresa, XBrushes.Black, page.Width - 150, yPos);

                        yPos += 40;
                        gfx.DrawString("________________________", fontEmpresa, XBrushes.Black, 20, yPos);
                        yPos += 20;
                        gfx.DrawString("Firma del emisor", fontContacto, XBrushes.Black, 20, yPos);

                        yPos += 30;
                        gfx.DrawString("Gracias por su pago. Si tiene alguna pregunta, comuníquese con nosotros.", fontContacto, XBrushes.Black, 20, yPos);

                        MemoryStream stream = new MemoryStream();
                        document.Save(stream, false);
                        stream.Position = 0;

                        // Devolver el PDF con el nombre idDeuda_Nombrepersona
                        string nombreArchivo = $"{reader.GetInt32(2)}_{reader.GetString(4)}.pdf";
                        return File(stream, "application/pdf", nombreArchivo);
                    }
                    else
                    {
                        return NotFound("No se encontraron registros para la persona y deuda especificada.");
                    }
                }

            }
        }

                [HttpPut]
        [Route("Actualizar_Deuda")]

        public bool actualizar_DeudasOrdinarias([FromBody] Deudas request)
        {
            bool Propiedad_actualizada = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("UPDATE deudas " +
                    "SET monto=@monto, " +
                    "nombre=@nombre, " +
                    "descripcion=@descripcion, " +
                    "dias_gracia=@dias_gracia, " +
                    "periodicidad=@periodicidad, " +
                    "recargo=@recargo " +
                    "WHERE id_deudas=@id_deudas", conexion);

                comando.Parameters.Add("@monto", MySqlDbType.Float).Value = request.monto;
                comando.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = request.nombre;
                comando.Parameters.Add("@descripcion", MySqlDbType.VarChar).Value = request.descripcion;
                comando.Parameters.Add("@dias_gracia", MySqlDbType.Int32).Value = request.dias_gracia;
                comando.Parameters.Add("@periodicidad", MySqlDbType.Int32).Value = request.periodicidad;
                comando.Parameters.Add("@recargo", MySqlDbType.Float).Value = request.recargo;
                comando.Parameters.Add("@id_deudas", MySqlDbType.Int32).Value = request.id_deudas;

                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Propiedad_actualizada = true;
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
                return Propiedad_actualizada;

            }


        }


        [HttpDelete]
        [Route("Eliminar_Deuda")]
        public bool Eliminar_Deudas(int id_deudas)
        {
            bool Persona_eliminada = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("DELETE FROM deudas WHERE id_deudas=@id_deudas", conexion);

                comando.Parameters.Add("@id_deudas", MySqlDbType.Int32).Value = id_deudas;




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



        [HttpPost]
        [Route("Agregar_DeudaExtra")]

        public bool Agregar_DeudaExtra([FromBody] Deudas request)
        {
            Console.WriteLine(request.destinatario);

            bool fraccionamiento_agregado = false;
            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("insert into deudas " +
                    "(id_deudas, id_fraccionamiento, id_tesorero, monto, nombre, descripcion," +
                    "proximo_pago, deudor) " +
                    "VALUES (@id_deudas, @id_fraccionamiento, @id_tesorero, @monto, @nombre, @descripcion," +
                    "@proximo_pago, @deudor)", conexion);

                //int destinatario = int.Parse(request.destinatario);

                comando.Parameters.Add("@id_deudas", MySqlDbType.Int32).Value = request.id_deudas;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = request.id_fraccionamiento;
                comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = request.id_tesorero;
                comando.Parameters.Add("@monto", MySqlDbType.Float).Value = request.monto;
                comando.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = request.nombre;
                comando.Parameters.Add("@descripcion", MySqlDbType.VarChar).Value = request.descripcion;
                comando.Parameters.Add("@proximo_pago", MySqlDbType.Date).Value = request.proximo_pago;
                comando.Parameters.Add("@deudor", MySqlDbType.Int32).Value = request.destinatario;


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

                Deudas obj_deudas = new Deudas();
                if (fraccionamiento_agregado)
                {
                    if (obj_deudas.AsignarDeudaNuevaATodos(request.id_fraccionamiento, request.destinatario))
                    {
                        fraccionamiento_agregado = true;
                    }
                    else
                    {
                        fraccionamiento_agregado = false;
                    }
                }

                return fraccionamiento_agregado;
            }
        }


        [HttpGet]
        [Route("Consultar_DeudaExtra")]

        public List<Deudas> Consultar_DeudaExtra(int id_tesorero)
        {

            List<Deudas> Deuda = new List<Deudas>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudas WHERE id_tesorero=@id_tesorero && periodicidad=0", conexion);

                comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = id_tesorero;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Deuda.Add(new Deudas()
                        {
                            id_deudas = reader.GetInt32(0),
                            monto = reader.GetFloat(3),
                            nombre = reader.GetString(4),
                            descripcion = reader.GetString(5),
                            proximo_pago = reader.GetDateTime(9)

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

                return Deuda;
            }

        }

        [HttpGet]
        [Route("Consultar_DeudasOrdinarias")]

        public List<Deudas> Consultar_DeudasOrdinarias(int id_tesorero)
        {

            List<Deudas> Deuda = new List<Deudas>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudas WHERE id_tesorero=@id_tesorero && periodicidad>0", conexion);

                comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = id_tesorero;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        //   string fechaProximoPago = (reader.GetDateTime(9)).ToString("yyyy-MM-ddTHH:mm:ss");

                        Deuda.Add(new Deudas()
                        {
                            id_deudas = reader.GetInt32(0),
                            monto = reader.GetFloat(3),
                            nombre = reader.GetString(4),
                            descripcion = reader.GetString(5),
                            dias_gracia = reader.GetInt32(6),
                            periodicidad = reader.GetInt32(7),
                            recargo = reader.GetFloat(8),
                            proximo_pago1 = (reader.GetDateTime(9)).ToString("yyyy-MM-dd")

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

                return Deuda;
            }

        }

        [HttpGet]
        [Route("Consultar_Deuda")]
        public List<Deudas> Consultar_Deuda(int id_tesorero, int tipo_deuda)
        {
            List<Deudas> Deuda = new List<Deudas>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                MySqlCommand comando = new MySqlCommand("sp_deudas", conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                comando.Parameters.AddWithValue("@id_tesorero", id_tesorero);
                comando.Parameters.AddWithValue("@periodicidad", tipo_deuda);

                try
                {
                    conexion.Open();

                    using (MySqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Deuda.Add(new Deudas()
                            {
                                id_deudas = reader.GetInt32(0),
                                monto = reader.GetFloat(3),
                                nombre = reader.GetString(4),
                                descripcion = reader.GetString(5),
                                proximo_pago = reader.GetDateTime(9),
                                deudor = reader.GetInt32(10), // Cambia el índice si es necesario
                                nombre_deudor = reader.GetString(11), // Cambia el índice si es necesario
                                recargo = reader.GetFloat(8),
                                periodicidad = reader.GetInt32(7),
                                dias_gracia = reader.GetInt32(6),
                                proximo_pago1 = (reader.GetDateTime(9)).ToString("yyyy-MM-dd")


                            });
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    // Manejo de errores (puedes registrar el error o lanzar una excepción)
                    Console.WriteLine("Error: " + ex.Message);
                }

                return Deuda;
            }
        }


        /*
        [HttpGet]
        [Route("Consultar_Deudores")]

        public List<Deudoress> Consultar_Deudores(int id_tesorero)
        {



            List<Deudoress> Deuda = new List<Deudoress>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudas WHERE id_tesorero=@id_tesorero && periodicidad>0", conexion);

                comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = id_tesorero;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Deuda.Add(new Deudoress()
                        {
                            concepto = reader.GetString(4),
                            persona = reader.GetString(5),
                            monto = reader.GetInt32(6),
                            proximo_pago = (reader.GetDateTime(11)).ToString("yyyy-MM-ddTHH:mm:ss")

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

                return Deuda;
            }

        }
        */

        [HttpGet]
        [Route("Consultar_Deudores")]

        public List<Deudoress> Consultar_Deudores(int id_fraccionamiento)
        {



            List<Deudoress> Deuda = new List<Deudoress>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudores WHERE id_fraccionamiento=@id_fraccionamiento && estado='Pendiente'", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Deuda.Add(new Deudoress()
                        {
                            id_deudor = reader.GetInt32(1),
                            id_deuda = reader.GetInt32(2),
                            id_fraccionamiento = reader.GetInt32(3),
                            nombre_persona = !reader.IsDBNull(4) ? reader.GetString(4) : "",
                            lote = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0,
                            tipo_deuda = reader.GetString(6),
                            nombre_deuda = reader.GetString(7),
                            monto = reader.GetFloat(8),
                            recargo = reader.GetFloat(9),
                            dias_gracia = reader.GetInt32(10),
                            proximo_pago = (reader.GetDateTime(11)).ToString("yyyy-MM-ddTHH:mm:ss"),
                            estado = reader.GetString(12),
                            periodicidad = reader.GetInt32(13),
                            monto_restante = reader.GetFloat(15)

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

                return Deuda;
            }

        }



        [HttpGet]
        [Route("Consultar_DeudoresUsuario")]

        public List<Deudoress> Consultar_DeudoresUsuario(int id_deudor, int estado)
        {
            List<Deudoress> Deuda = new List<Deudoress>();

            string query = "";

            if (estado == 1)
            {
                query = " AND estado='pagado'";
            }
            else if (estado == 2)
            {
                query = " AND estado='Pendiente'";

            }

            //   Console.WriteLine(id_lote);

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudores WHERE (id_deudor=@id_deudor)" + query, conexion);

                comando.Parameters.Add("@id_deudor", MySqlDbType.Int32).Value = id_deudor;



                try
                {
                    conexion.Open();
                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Deudoress deuda = new Deudoress()
                        {
                            id_deudor = reader.GetInt32(1),
                            id_deuda = reader.GetInt32(2),
                            id_fraccionamiento = reader.GetInt32(3),
                            nombre_persona = !reader.IsDBNull(4) ? reader.GetString(4) : "",
                            lote = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0,
                            tipo_deuda = reader.GetString(6),
                            nombre_deuda = reader.GetString(7),
                            monto = reader.GetFloat(8),
                            recargo = reader.GetFloat(9),
                            dias_gracia = reader.GetInt32(10),
                            proximo_pago = (reader.GetDateTime(11)).ToString("yyyy-MM-ddTHH:mm:ss"),
                            estado = reader.GetString(12),
                            periodicidad = reader.GetInt32(13)

                        };
                        DateTime fechaLimitePago = (reader.GetDateTime(11)).AddDays(deuda.periodicidad);

                        if (deuda.tipo_deuda == "ordinaria")
                        {
                            Deuda.Add(deuda);
                            // Verificar si la deuda está vencida utilizando DateTime.Compare
                            if (DateTime.Compare(fechaLimitePago, DateTime.Today) <= 0)
                            {

                            }
                        }
                        else if (deuda.tipo_deuda == "extraordinaria")
                        {
                            if (deuda.estado != "pagado")
                            {
                                Deuda.Add(deuda);
                            }
                        }


                    }
                }
                catch (MySqlException ex)
                {
                    // Manejar la excepción
                }
                finally
                {
                    conexion.Close();
                }

                return Deuda;
            }
        }

        //  [HttpGet]
        //    [Route("Consultar_TotalDeuda")]
        public static int CalcularDiasPasados(string fecha)
        {
            // Intentar parsear la fecha
            if (DateTime.TryParse(fecha, out DateTime fechaParsed))
            {
                // Calcular la diferencia en días
                TimeSpan diferencia = DateTime.Now - fechaParsed;
                return (int)diferencia.TotalDays;
            }
            else
            {
                throw new ArgumentException("Formato de fecha no válido. Debe ser 'yyyy-MM-ddTHH:mm:ss'.");
            }
        }



        [HttpGet]
        [Route("Consultar_TotalDeuda")]

        public float Consultar_TotalDeuda(int id_persona)
        {
            //    List<Deudoress> Deuda = new List<Deudoress>();
            //    List<Deudoress> deuda = new List<Deudoress>();
            float monto = 0, recargo = 0, deuda = 0;
            int periodicidad = 0;

            string fecha = "";


            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudores WHERE id_deudor=@id_persona and estado='Pendiente'", conexion);

                comando.Parameters.Add("@id_persona", MySqlDbType.Int32).Value = id_persona;



                try
                {
                    conexion.Open();
                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        fecha = (reader.GetDateTime(11)).ToString("yyyy-MM-ddTHH:mm:ss");

                        periodicidad = reader.IsDBNull(13) ? 0 : reader.GetInt32(13);
                        monto = reader.IsDBNull(8) ? 0 : reader.GetFloat(8);
                        recargo = reader.IsDBNull(9) ? 0 : reader.GetFloat(9);

                        if (periodicidad > 0)
                        {
                            deuda += ((CalcularDiasPasados(fecha) / periodicidad) * recargo) + ((CalcularDiasPasados(fecha) / periodicidad) * monto);
                        }
                        else
                        {
                            deuda += monto;
                        }




                    }
                }
                catch (MySqlException ex)
                {
                    // Manejar la excepción
                }
                finally
                {
                    conexion.Close();
                }

                return deuda;
            }
        }


        [HttpGet]
        [Route("Consultar_DeudoresOrdinarios")]

        public List<Deudoress> Consultar_DeudoresOrdinarios(int id_fraccionamiento)
        {
            List<Deudoress> Deuda = new List<Deudoress>();
            List<Deudoress> deuda = new List<Deudoress>();


            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudores WHERE id_fraccionamiento=@id_fraccionamiento and estado='Pendiente'", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;



                try
                {
                    conexion.Open();
                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        DateTime now = DateTime.Now;
                        //   DateTime Dateproximo_pago = DateTime.Now.AddDays(90);
                        DateTime fechaProximoPago = reader.GetDateTime(11);
                        //   string fechaActual = now.ToString("yyyy-MM-ddTHH:mm:ss");

                        //    Deudoress Deuda = new Deudoress()

                        Deuda.Add(new Deudoress()
                        {
                            id_deudor = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                            id_deuda = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                            id_fraccionamiento = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                            nombre_persona = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            lote = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                            tipo_deuda = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            nombre_deuda = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            monto = reader.IsDBNull(15) ? 0 : reader.GetFloat(15),
                            recargo = reader.IsDBNull(9) ? 0 : reader.GetFloat(9),
                            dias_gracia = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),
                            proximo_pago = fechaProximoPago.ToString("yyyy-MM-ddTHH:mm:ss"),
                            estado = reader.IsDBNull(12) ? "" : reader.GetString(12),
                            periodicidad = reader.IsDBNull(13) ? 0 : reader.GetInt32(13)

                        });


                        DateTime fechaLimitePago = fechaProximoPago.AddDays(reader.GetInt32(13));

                        // Agregar impresiones para depurar
                        //    Console.WriteLine($"Fecha límite para el pago: {fechaLimitePago}, Fecha actual: {DateTime.Now}");

                        // Verificar si la deuda está vencida utilizando DateTime.Compare


                        /*
                        if (DateTime.Compare(fechaLimitePago, DateTime.Now) < 0)
                        {

                        }
                        */


                    }
                }
                catch (MySqlException ex)
                {
                    // Manejar la excepción
                }
                finally
                {
                    conexion.Close();
                }

                return Deuda;
            }
        }


        [HttpGet]
        [Route("Consultar_DeudorOrdinario")]

        public List<Deudoress> Consultar_DeudorOrdinario(int id_fraccionamiento, int id_usuario)
        {
            List<Deudoress> Deuda = new List<Deudoress>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudores WHERE (id_fraccionamiento=@id_fraccionamiento && id_deudor=@id_deudor && periodicidad>0)", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
                comando.Parameters.Add("@id_deudor", MySqlDbType.Int32).Value = id_usuario;

                try
                {
                    conexion.Open();
                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Deudoress deuda = new Deudoress()
                        {
                            id_deudor = reader.GetInt32(1),
                            id_deuda = reader.GetInt32(2),
                            id_fraccionamiento = reader.GetInt32(3),
                            nombre_persona = !reader.IsDBNull(4) ? reader.GetString(4) : "",
                            lote = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0,
                            tipo_deuda = reader.GetString(6),
                            nombre_deuda = reader.GetString(7),
                            monto = reader.GetFloat(8),
                            recargo = reader.GetFloat(9),
                            dias_gracia = reader.GetInt32(10),
                            proximo_pago = (reader.GetDateTime(11)).ToString("yyyy-MM-ddTHH:mm:ss"),
                            estado = reader.GetString(12),
                            periodicidad = reader.GetInt32(13)
                        };
                        DateTime fechaLimitePago = (reader.GetDateTime(11)).AddDays(deuda.periodicidad);

                        // Agregar impresiones para depurar
                        Console.WriteLine($"Fecha límite para el pago: {fechaLimitePago}, Fecha actual: {DateTime.Today}");

                        // Verificar si la deuda está vencida utilizando DateTime.Compare
                        if (DateTime.Compare(fechaLimitePago, DateTime.Today) < 0)
                        {
                            Deuda.Add(deuda);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    // Manejar la excepción
                }
                finally
                {
                    conexion.Close();
                }

                return Deuda;
            }
        }

        [HttpPost]
        [Route("Pagar_DeudaOrdinaria")]

        public bool Pagar_DeudaOrdinaria([FromForm] PagoDeudaOrdinariaRequest request)
        {
            /*
            byte[] archivoEnBytes = new byte[0];

            if (request.file.Length > 0)
            {

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    request.file.CopyTo(memoryStream);
                    archivoEnBytes = memoryStream.ToArray(); // Convertir a byte[]
                }
            }
            */
            Deudas obj_deudas = new Deudas();

            List<Deudoress> ListaParaHistorial = obj_deudas.Consultar_DeudaPorId(request.id_deudor, request.id_deuda, request.id_fraccionamiento);

            bool historial_actualizado = false;
            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("insert into historial_deudaspagadas (id_deudor, id_deuda, id_fraccionamiento, nombre_persona, lote, tipo_deuda, nombre_deuda, monto, recargo, dias_gracia, estado, periodicidad, comprobante, periodo) VALUES (@id_deudor, @id_deuda, @id_fraccionamiento, @nombre_persona, @lote, @tipo_deuda, @nombre_deuda, @monto, @recargo, @dias_gracia, @estado, @periodicidad, @comprobante, @periodo)", conexion);

                comando.Parameters.Add("@id_deudor", MySqlDbType.Int32).Value = ListaParaHistorial[0].id_deudor;
                comando.Parameters.Add("@id_deuda", MySqlDbType.Int32).Value = ListaParaHistorial[0].id_deuda;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = ListaParaHistorial[0].id_fraccionamiento;
                comando.Parameters.Add("@nombre_persona", MySqlDbType.VarChar).Value = ListaParaHistorial[0].nombre_persona;
                comando.Parameters.Add("@lote", MySqlDbType.Int32).Value = ListaParaHistorial[0].lote;
                comando.Parameters.Add("@tipo_deuda", MySqlDbType.VarChar).Value = ListaParaHistorial[0].tipo_deuda;
                comando.Parameters.Add("@nombre_deuda", MySqlDbType.VarChar).Value = ListaParaHistorial[0].nombre_deuda;
                comando.Parameters.Add("@monto", MySqlDbType.Float).Value = ListaParaHistorial[0].monto;
                comando.Parameters.Add("@recargo", MySqlDbType.Float).Value = ListaParaHistorial[0].recargo;
                comando.Parameters.Add("@dias_gracia", MySqlDbType.Int32).Value = ListaParaHistorial[0].dias_gracia;
                comando.Parameters.Add("@estado", MySqlDbType.VarChar).Value = "pagado";
                comando.Parameters.Add("@periodicidad", MySqlDbType.Int32).Value = ListaParaHistorial[0].periodicidad;
                comando.Parameters.AddWithValue("@comprobante", request.file);
                comando.Parameters.Add("@periodo", MySqlDbType.VarChar).Value = ListaParaHistorial[0].proximo_pago + " - " + request.proximo_pago;


                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        historial_actualizado = true;
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


            bool Propiedad_actualizada = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("UPDATE deudores " +
                    "SET proximo_pago=@proximo_pago " +
                    "WHERE id_deudor=@id_deudor && id_deuda=@id_deuda && id_fraccionamiento=@id_fraccionamiento", conexion);

                comando.Parameters.Add("@proximo_pago", MySqlDbType.Date).Value = request.proximo_pago;
                comando.Parameters.Add("@id_deudor", MySqlDbType.Int64).Value = request.id_deudor;
                comando.Parameters.Add("@id_deuda", MySqlDbType.Int64).Value = request.id_deuda;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int64).Value = request.id_fraccionamiento;


                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Propiedad_actualizada = true;
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
                return Propiedad_actualizada;

            }


        }


        [HttpGet]
        [Route("Consultar_DeudorExtraordinario")]

        public List<Deudoress> Consultar_DeudoresExtraordinarios(int id_fraccionamiento, int id_usuario)
        {
            List<Deudoress> Deuda = new List<Deudoress>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                //MySqlCommand comando = new MySqlCommand("SELECT * FROM deudores WHERE (id_fraccionamiento=@id_fraccionamiento && id_deudor=@id_deudor && periodicidad=0 && estado='Pendiente')", conexion);
                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudores WHERE (id_fraccionamiento=@id_fraccionamiento && id_deudor=@id_deudor && estado='Pendiente')", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
                comando.Parameters.Add("@id_deudor", MySqlDbType.Int32).Value = id_usuario;

                try
                {
                    conexion.Open();
                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Deudoress deuda = new Deudoress()
                        {
                            id_deudor = reader.GetInt32(1),
                            id_deuda = reader.GetInt32(2),
                            id_fraccionamiento = reader.GetInt32(3),
                            nombre_persona = !reader.IsDBNull(4) ? reader.GetString(4) : "",
                            lote = !reader.IsDBNull(5) ? reader.GetInt32(5) : 0,
                            tipo_deuda = reader.GetString(6),
                            nombre_deuda = reader.GetString(7),
                            monto = reader.GetFloat(8),
                            recargo = reader.GetFloat(9),
                            dias_gracia = reader.GetInt32(10),
                            proximo_pago = (reader.GetDateTime(11)).ToString("yyyy-MM-ddTHH:mm:ss"),
                            estado = reader.GetString(12),
                            periodicidad = reader.GetInt32(13),
                            monto_restante = reader.GetFloat(15)



                        };

                        Deuda.Add(deuda);

                        // Agregar impresiones para depurar

                    }
                }
                catch (MySqlException ex)
                {
                    // Manejar la excepción
                }
                finally
                {
                    conexion.Close();
                }

                return Deuda;
            }
        }


        [HttpPost]
        [Route("Pagar_DeudaExtraordinaria")]

        public bool Pagar_DeudaExtraordinaria([FromForm] PagoDeudaOrdinariaRequest request)
        {

            Deudas obj_deudas = new Deudas();

            List<Deudoress> ListaParaHistorial = obj_deudas.Consultar_DeudaPorId(request.id_deudor, request.id_deuda, request.id_fraccionamiento);

            bool historial_actualizado = false;

            if (request.tipo_pago == "pagado")
            {
                /*
                byte[] archivoEnBytes = new byte[0];

                if (request.file.Length > 0)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        request.file.CopyTo(memoryStream);
                        archivoEnBytes = memoryStream.ToArray(); // Convertir a byte[]
                    }
                }
                */
                using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
                {
                    int rowsaffected = 0;
                    MySqlCommand comando = new MySqlCommand("insert into historial_deudaspagadas (id_deudor, id_deuda, id_fraccionamiento, nombre_persona, lote, tipo_deuda, nombre_deuda, monto, recargo, dias_gracia, estado, periodicidad, comprobante, periodo) VALUES (@id_deudor, @id_deuda, @id_fraccionamiento, @nombre_persona, @lote, @tipo_deuda, @nombre_deuda, @monto, @recargo, @dias_gracia, @estado, @periodicidad, @comprobante, @periodo)", conexion);

                    comando.Parameters.Add("@id_deudor", MySqlDbType.Int32).Value = ListaParaHistorial[0].id_deudor;
                    comando.Parameters.Add("@id_deuda", MySqlDbType.Int32).Value = ListaParaHistorial[0].id_deuda;
                    comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = ListaParaHistorial[0].id_fraccionamiento;
                    comando.Parameters.Add("@nombre_persona", MySqlDbType.VarChar).Value = ListaParaHistorial[0].nombre_persona;
                    comando.Parameters.Add("@lote", MySqlDbType.Int32).Value = ListaParaHistorial[0].lote;
                    comando.Parameters.Add("@tipo_deuda", MySqlDbType.VarChar).Value = ListaParaHistorial[0].tipo_deuda;
                    comando.Parameters.Add("@nombre_deuda", MySqlDbType.VarChar).Value = ListaParaHistorial[0].nombre_deuda;
                    comando.Parameters.Add("@monto", MySqlDbType.Float).Value = ListaParaHistorial[0].monto;
                    // comando.Parameters.Add("@recargo", MySqlDbType.Float).Value = ListaParaHistorial[0].recargo;
                    comando.Parameters.Add("@recargo", MySqlDbType.Float).Value = request.recargo; //se va a poner el recargo si la fecha ya pasó
                    comando.Parameters.Add("@dias_gracia", MySqlDbType.Int32).Value = ListaParaHistorial[0].dias_gracia;
                    comando.Parameters.Add("@estado", MySqlDbType.VarChar).Value = "pagado";
                    comando.Parameters.Add("@periodicidad", MySqlDbType.Int32).Value = ListaParaHistorial[0].periodicidad;
                    comando.Parameters.AddWithValue("@comprobante", request.file);
                    //comando.Parameters.Add("@comprobante", MySqlDbType.VarChar).Value = request.file;

                    //  comando.Parameters.AddWithValue("@comprobante", "n");
                    comando.Parameters.Add("@periodo", MySqlDbType.VarChar).Value = ListaParaHistorial[0].proximo_pago + " - " + request.proximo_pago;


                    try
                    {
                        conexion.Open();
                        rowsaffected = comando.ExecuteNonQuery();

                        if (rowsaffected >= 1)
                        {
                            historial_actualizado = true;

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





                //
                bool Propiedad_actualizada = false;

                using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
                {
                    int rowsaffected = 0;
                    MySqlCommand comando = new MySqlCommand("UPDATE deudores " +
                        "SET estado='pagado', dia_registro=@dia_registro, monto = @abono, recargo = @recargo, comprobante = @comprobante " +
                        "WHERE id_deudor=@id_deudor && id_deuda=@id_deuda && id_fraccionamiento=@id_fraccionamiento && estado='Pendiente';", conexion);

                    //insert into deudores (id_deudor, id_deuda, id_fraccionamiento, nombre_persona, lote, tipo_deuda, nombre_deuda, monto, recargo, dias_gracia, estado, periodicidad, comprobante, monto_pendiente) VALUES (@id_deudor, @id_deuda, @id_fraccionamiento, @nombre_persona, @lote, @tipo_deuda, @nombre_deuda, @abono, @recargo, @dias_gracia, @estado, @periodicidad, @comprobante, @monto_pendiente)

                    /*
                    comando.Parameters.Add("@proximo_pago", MySqlDbType.Date).Value = request.proximo_pago;
                    comando.Parameters.Add("@id_deudor", MySqlDbType.Int64).Value = request.id_deudor;
                    comando.Parameters.Add("@id_deuda", MySqlDbType.Int64).Value = request.id_deuda;
                    comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int64).Value = request.id_fraccionamiento;
                    comando.Parameters.AddWithValue("@comprobante", "n");
                    */

                    comando.Parameters.Add("@id_deudor", MySqlDbType.Int32).Value = ListaParaHistorial[0].id_deudor;
                    comando.Parameters.Add("@id_deuda", MySqlDbType.Int32).Value = ListaParaHistorial[0].id_deuda;
                    comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = ListaParaHistorial[0].id_fraccionamiento;
                    comando.Parameters.Add("@nombre_persona", MySqlDbType.VarChar).Value = ListaParaHistorial[0].nombre_persona;
                    comando.Parameters.Add("@lote", MySqlDbType.Int32).Value = ListaParaHistorial[0].lote;
                    comando.Parameters.Add("@tipo_deuda", MySqlDbType.VarChar).Value = ListaParaHistorial[0].tipo_deuda;
                    comando.Parameters.Add("@nombre_deuda", MySqlDbType.VarChar).Value = ListaParaHistorial[0].nombre_deuda;
                    //comando.Parameters.Add("@recargo", MySqlDbType.Float).Value = ListaParaHistorial[0].recargo;
                    comando.Parameters.Add("@recargo", MySqlDbType.Float).Value = request.recargo;
                    comando.Parameters.Add("@dias_gracia", MySqlDbType.Int32).Value = ListaParaHistorial[0].dias_gracia;
                    comando.Parameters.Add("@estado", MySqlDbType.VarChar).Value = "abono";
                    comando.Parameters.Add("@periodicidad", MySqlDbType.Int32).Value = ListaParaHistorial[0].periodicidad;
                    comando.Parameters.AddWithValue("@comprobante", request.file);
                    comando.Parameters.Add("@abono", MySqlDbType.Float).Value = request.monto_pendiente;
                    comando.Parameters.Add("@monto_pendiente", MySqlDbType.Float).Value = ListaParaHistorial[0].monto_restante - request.monto_pendiente;
                    comando.Parameters.Add("@dia_registro", MySqlDbType.VarChar).Value = (DateTime.Now).ToString("yyyy-MM-dd");


                    try
                    {
                        conexion.Open();
                        rowsaffected = comando.ExecuteNonQuery();

                        if (rowsaffected >= 1)
                        {
                            Propiedad_actualizada = true;
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
                    return Propiedad_actualizada;

                }
            }
            else
            {
                using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
                {
                    int rowsaffected = 0;
                    MySqlCommand comando = new MySqlCommand("UPDATE deudores SET monto_pendiente = @monto_pendiente, comprobante = @comprobante WHERE id_deudor=@id_deudor and id_deuda=@id_deuda and estado='Pendiente'; " +
                        "insert into deudores (id_deudor, id_deuda, id_fraccionamiento, nombre_persona, lote, tipo_deuda, nombre_deuda, monto, recargo, dias_gracia, estado, periodicidad, comprobante, monto_pendiente) VALUES (@id_deudor, @id_deuda, @id_fraccionamiento, @nombre_persona, @lote, @tipo_deuda, @nombre_deuda, @abono, @recargo, @dias_gracia, @estado, @periodicidad, @comprobante, @monto_pendiente)", conexion);

                    /*
                    comando.Parameters.Add("@id_deudor", MySqlDbType.Int32).Value = ListaParaHistorial[0].id_deudor;
                    comando.Parameters.Add("@monto", MySqlDbType.Int32).Value = request.monto;

                    comando.Parameters.Add("@id_deuda", MySqlDbType.Int32).Value = ListaParaHistorial[0].id_deuda;
                    */

                    comando.Parameters.Add("@id_deudor", MySqlDbType.Int32).Value = ListaParaHistorial[0].id_deudor;
                    comando.Parameters.Add("@id_deuda", MySqlDbType.Int32).Value = ListaParaHistorial[0].id_deuda;
                    comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = ListaParaHistorial[0].id_fraccionamiento;
                    comando.Parameters.Add("@nombre_persona", MySqlDbType.VarChar).Value = ListaParaHistorial[0].nombre_persona;
                    comando.Parameters.Add("@lote", MySqlDbType.Int32).Value = ListaParaHistorial[0].lote;
                    comando.Parameters.Add("@tipo_deuda", MySqlDbType.VarChar).Value = ListaParaHistorial[0].tipo_deuda;
                    comando.Parameters.Add("@nombre_deuda", MySqlDbType.VarChar).Value = ListaParaHistorial[0].nombre_deuda;
               //     comando.Parameters.Add("@monto", MySqlDbType.Float).Value = ListaParaHistorial[0].monto;
                   


                    comando.Parameters.Add("@recargo", MySqlDbType.Float).Value = ListaParaHistorial[0].recargo;
                    comando.Parameters.Add("@dias_gracia", MySqlDbType.Int32).Value = ListaParaHistorial[0].dias_gracia;
                    comando.Parameters.Add("@estado", MySqlDbType.VarChar).Value = "abono";
                    comando.Parameters.Add("@periodicidad", MySqlDbType.Int32).Value = ListaParaHistorial[0].periodicidad;
                    comando.Parameters.AddWithValue("@comprobante", request.file);

                    comando.Parameters.Add("@abono", MySqlDbType.Float).Value = request.monto_pendiente;


                    comando.Parameters.Add("@monto_pendiente", MySqlDbType.Float).Value =  ListaParaHistorial[0].monto_restante - request.monto_pendiente;



                    try
                    {
                        conexion.Open();
                        rowsaffected = comando.ExecuteNonQuery();

                        if (rowsaffected >= 1)
                        {
                            historial_actualizado = true;
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


            }

            return historial_actualizado;
        }

        [HttpGet]
        [Route("Consultar_HistorialDeudasUsuario")]

        public List<h_deuda> Consultar_HistorialDeudasUsuario(int id_deudor, int id_fraccionamiento)
        {
            List<h_deuda> historial_deudas = new List<h_deuda>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                MySqlCommand comando = new MySqlCommand("SELECT * FROM historial_deudaspagadas WHERE (id_deudor=@id_deudor && id_fraccionamiento=@id_fraccionamiento)", conexion);

                comando.Parameters.Add("@id_deudor", MySqlDbType.Int32).Value = id_deudor;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;

                try
                {
                    conexion.Open();
                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        h_deuda deuda = new h_deuda()
                        {
                            id = reader.GetInt32(0),
                            id_deudor = reader.GetInt32(1),
                            id_deuda = reader.GetInt32(2),
                            id_fraccionamiento = reader.GetInt32(3),
                            nombre_persona = reader.GetString(4),
                            lote = reader.GetInt32(5),
                            tipo_deuda = reader.GetString(6),
                            nombre_deuda = reader.GetString(7),
                            monto = reader.GetFloat(8),
                            recargo = reader.GetFloat(9),
                            dias_gracia = reader.GetInt32(10),
                            estado = reader.GetString(11),
                            periodicidad = reader.GetInt32(12),
                            periodo = reader.GetString(14),
                            monto_restante = reader.GetFloat(15)

                        };

                        historial_deudas.Add(deuda);
                    }
                }
                catch (MySqlException ex)
                {
                    // Manejar la excepción
                }
                finally
                {
                    conexion.Close();
                }

                return historial_deudas;
            }
        }

        [HttpGet]
        [Route("Consultar_Comprobante")]
        public IActionResult Consultar_Comprobante(int id_historial)
        {
            Deudas obj_deuda = new Deudas();

            byte[] imagenBytes = obj_deuda.Consultar_Comprobante(id_historial);

            // Devolver los bytes como contenido binario
            return File(imagenBytes, "image/jpeg"); // Cambia el tipo de contenido según el formato de tu imagen
        }


        [HttpGet]
        [Route("Consultar_HistorialDeudas")]

        public List<h_deuda> Consultar_HistorialDeudas(int id_fraccionamiento)
        {
            List<h_deuda> historial_deudas = new List<h_deuda>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudores WHERE id_fraccionamiento=@id_fraccionamiento && estado!='Pendiente'", conexion);

                //  comando.Parameters.Add("@id_deudor", MySqlDbType.Int32).Value = id_deudor;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;

                try
                {
                    conexion.Open();
                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                       // DateTime fechaProximoPago = reader.GetDateTime(11);

                        h_deuda deuda = new h_deuda()
                        {
                            id = reader.GetInt32(0),
                            id_deudor = reader.GetInt32(1),
                            id_deuda = reader.GetInt32(2),
                            id_fraccionamiento = reader.GetInt32(3),
                            nombre_persona = reader.GetString(4),
                            lote = reader.GetInt32(5),
                            tipo_deuda = reader.GetString(6),
                            nombre_deuda = reader.GetString(7),
                            monto = reader.GetFloat(8),
                            recargo = reader.GetFloat(9),
                            dias_gracia = reader.GetInt32(10),
                            estado = reader.GetString(12),
                            periodicidad = reader.GetInt32(13),
                            dia_registro = (reader.GetDateTime(14)).ToString("yyyy-MM-dd"),
                            //  periodo = reader.GetString(14),
                            comprobante = reader.GetString(16)

                        };

                        historial_deudas.Add(deuda);
                    }
                }
                catch (MySqlException ex)
                {
                    // Manejar la excepción
                }
                finally
                {
                    conexion.Close();
                }

                return historial_deudas;
            }
        }

    }
}