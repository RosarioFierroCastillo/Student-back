using Microsoft.AspNetCore.Mvc;
using API_Archivo.Clases;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace API_Archivo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportesController : ControllerBase
    {
        [HttpGet]
        [Route("Reporte_Entradas")]
        public IActionResult GenerarReporteEntradas()
        {
            Entradas objEntradas = new Entradas();

            List<Entradas> entradas = objEntradas.Consultar_Entradas();

            // Crear un nuevo documento PDF
            PdfDocument document = new PdfDocument();

            // Añadir una página al documento
            PdfPage page = document.AddPage();

            // Obtener el objeto XGraphics para dibujar en la página
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Dibujar el encabezado
            XFont fontEncabezado = new XFont("Arial", 16);
            XSize textSize = gfx.MeasureString("Reporte de Entradas", fontEncabezado);
            double xPos = (page.Width - textSize.Width) / 2; // Centrar horizontalmente
            double yPos = 40; // Posición fija en la parte superior de la página
            gfx.DrawString("Reporte de Entradas", fontEncabezado, XBrushes.Black,
                new XRect(xPos, yPos, page.Width, page.Height),
                XStringFormats.TopLeft);

            // Dibujar la tabla de datos
            double tableWidth = 400;
            yPos += 30; // Ajuste de la posición Y
            xPos = (page.Width - tableWidth) / 2;
            double cellHeight = 30;

            // Encabezados de columna
            string[] headers = { "ID", "Fecha", "Nombre de la Persona" };
            XFont fontHeader = new XFont("Arial", 12);
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawRectangle(XBrushes.LightGray, xPos, yPos, tableWidth / 3, cellHeight);
                gfx.DrawString(headers[i], fontHeader, XBrushes.Black,
                    new XRect(xPos, yPos, tableWidth / 3, cellHeight),
                    XStringFormats.Center);
                xPos += tableWidth / 3;
            }
            yPos += cellHeight;

            // Datos de las entradas
            XFont fontData = new XFont("Arial", 12);
            foreach (var entrada in entradas)
            {
                xPos = (page.Width - tableWidth) / 2;

                gfx.DrawRectangle(XBrushes.White, xPos, yPos, tableWidth / 3, cellHeight);
                gfx.DrawString(entrada.id_entrada.ToString(), fontData, XBrushes.Black,
                    new XRect(xPos, yPos, tableWidth / 3, cellHeight),
                    XStringFormats.Center);
                xPos += tableWidth / 3;

                gfx.DrawRectangle(XBrushes.White, xPos, yPos, tableWidth / 3, cellHeight);
                gfx.DrawString(entrada.fecha.ToString("dd/MM/yyyy HH:mm"), fontData, XBrushes.Black,
                    new XRect(xPos, yPos, tableWidth / 3, cellHeight),
                    XStringFormats.Center);
                xPos += tableWidth / 3;

                gfx.DrawRectangle(XBrushes.White, xPos, yPos, tableWidth / 3, cellHeight);
                gfx.DrawString(entrada.nombre, fontData, XBrushes.Black,
                    new XRect(xPos, yPos, tableWidth / 3, cellHeight),
                    XStringFormats.Center);

                yPos += cellHeight;
            }

            // Guardar el documento en un MemoryStream
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;

            // Devolver el PDF como un archivo para descargar
            return File(stream, "application/pdf", "reporte_SinFiltros.pdf");
        }

        [HttpGet]
        [Route("Reporte_EntradasPorPersona")]
        public IActionResult GenerarReporteEntradasPorPersona(string nombre_persona)
        {
            Entradas objEntradas = new Entradas();

            List<Entradas> entradas = objEntradas.Consultar_EntradasPorPersona(nombre_persona);

            // Crear un nuevo documento PDF
            PdfDocument document = new PdfDocument();

            // Añadir una página al documento
            PdfPage page = document.AddPage();

            // Obtener el objeto XGraphics para dibujar en la página
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Dibujar el encabezado
            XFont fontEncabezado = new XFont("Arial", 16);
            XSize textSize = gfx.MeasureString("Reporte de Entradas", fontEncabezado);
            double xPos = (page.Width - textSize.Width) / 2; // Centrar horizontalmente
            double yPos = 40; // Posición fija en la parte superior de la página
            gfx.DrawString("Reporte de Entradas", fontEncabezado, XBrushes.Black,
                new XRect(xPos, yPos, page.Width, page.Height),
                XStringFormats.TopLeft);

            // Dibujar la tabla de datos
            double tableWidth = 400;
            yPos += 30; // Ajuste de la posición Y
            xPos = (page.Width - tableWidth) / 2;
            double cellHeight = 30;

            // Encabezados de columna
            string[] headers = { "ID", "Fecha", "Nombre de la Persona" };
            XFont fontHeader = new XFont("Arial", 12);
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawRectangle(XBrushes.LightGray, xPos, yPos, tableWidth / 3, cellHeight);
                gfx.DrawString(headers[i], fontHeader, XBrushes.Black,
                    new XRect(xPos, yPos, tableWidth / 3, cellHeight),
                    XStringFormats.Center);
                xPos += tableWidth / 3;
            }
            yPos += cellHeight;

            // Datos de las entradas
            XFont fontData = new XFont("Arial", 12);
            foreach (var entrada in entradas)
            {
                xPos = (page.Width - tableWidth) / 2;

                gfx.DrawRectangle(XBrushes.White, xPos, yPos, tableWidth / 3, cellHeight);
                gfx.DrawString(entrada.id_entrada.ToString(), fontData, XBrushes.Black,
                    new XRect(xPos, yPos, tableWidth / 3, cellHeight),
                    XStringFormats.Center);
                xPos += tableWidth / 3;

                gfx.DrawRectangle(XBrushes.White, xPos, yPos, tableWidth / 3, cellHeight);
                gfx.DrawString(entrada.fecha.ToString("dd/MM/yyyy HH:mm"), fontData, XBrushes.Black,
                    new XRect(xPos, yPos, tableWidth / 3, cellHeight),
                    XStringFormats.Center);
                xPos += tableWidth / 3;

                gfx.DrawRectangle(XBrushes.White, xPos, yPos, tableWidth / 3, cellHeight);
                gfx.DrawString(entrada.nombre, fontData, XBrushes.Black,
                    new XRect(xPos, yPos, tableWidth / 3, cellHeight),
                    XStringFormats.Center);

                yPos += cellHeight;
            }

            // Guardar el documento en un MemoryStream
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;

            // Devolver el PDF como un archivo para descargar
            return File(stream, "application/pdf", $"reporte_entradasPorPersona{nombre_persona}.pdf");
        }

        [HttpGet]
        [Route("Reporte_EntradasPorFecha")]
        public IActionResult GenerarReporteEntradasPorFecha(string fecha_inicio, string fecha_final)
        {
            Entradas objEntradas = new Entradas();

            List<Entradas> entradas = objEntradas.Consultar_EntradasPorFecha(fecha_inicio, fecha_final);

            // Crear un nuevo documento PDF
            PdfDocument document = new PdfDocument();

            // Añadir una página al documento
            PdfPage page = document.AddPage();

            // Obtener el objeto XGraphics para dibujar en la página
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Dibujar el encabezado
            XFont fontEncabezado = new XFont("Arial", 16);
            XSize textSize = gfx.MeasureString("Reporte de Entradas", fontEncabezado);
            double xPos = (page.Width - textSize.Width) / 2; // Centrar horizontalmente
            double yPos = 40; // Posición fija en la parte superior de la página
            gfx.DrawString("Reporte de Entradas", fontEncabezado, XBrushes.Black,
                new XRect(xPos, yPos, page.Width, page.Height),
                XStringFormats.TopLeft);

            // Dibujar la tabla de datos
            double tableWidth = 400;
            yPos += 30; // Ajuste de la posición Y
            xPos = (page.Width - tableWidth) / 2;
            double cellHeight = 30;

            // Encabezados de columna
            string[] headers = { "ID", "Fecha", "Nombre de la Persona" };
            XFont fontHeader = new XFont("Arial", 12);
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawRectangle(XBrushes.LightGray, xPos, yPos, tableWidth / 3, cellHeight);
                gfx.DrawString(headers[i], fontHeader, XBrushes.Black,
                    new XRect(xPos, yPos, tableWidth / 3, cellHeight),
                    XStringFormats.Center);
                xPos += tableWidth / 3;
            }
            yPos += cellHeight;

            // Datos de las entradas
            XFont fontData = new XFont("Arial", 12);
            foreach (var entrada in entradas)
            {
                xPos = (page.Width - tableWidth) / 2;

                gfx.DrawRectangle(XBrushes.White, xPos, yPos, tableWidth / 3, cellHeight);
                gfx.DrawString(entrada.id_entrada.ToString(), fontData, XBrushes.Black,
                    new XRect(xPos, yPos, tableWidth / 3, cellHeight),
                    XStringFormats.Center);
                xPos += tableWidth / 3;

                gfx.DrawRectangle(XBrushes.White, xPos, yPos, tableWidth / 3, cellHeight);
                gfx.DrawString(entrada.fecha.ToString("dd/MM/yyyy HH:mm"), fontData, XBrushes.Black,
                    new XRect(xPos, yPos, tableWidth / 3, cellHeight),
                    XStringFormats.Center);
                xPos += tableWidth / 3;

                gfx.DrawRectangle(XBrushes.White, xPos, yPos, tableWidth / 3, cellHeight);
                gfx.DrawString(entrada.nombre, fontData, XBrushes.Black,
                    new XRect(xPos, yPos, tableWidth / 3, cellHeight),
                    XStringFormats.Center);

                yPos += cellHeight;
            }

            // Guardar el documento en un MemoryStream
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;

            // Devolver el PDF como un archivo para descargar
            return File(stream, "application/pdf", $"reporte_entradasPorFecha{fecha_inicio}-{fecha_final}.pdf");
        }

        [HttpGet]
        [Route("Reporte_Deudas")]
        public IActionResult GenerarReporteDeudas(int id_tesorero, string tipo_deuda)
        {
            DeudasController objDeudas = new DeudasController();
            List<Deudas> deudas = new List<Deudas>();

            if (tipo_deuda == "Ordinaria" || tipo_deuda == "ordinaria")
            {
                deudas = objDeudas.Consultar_DeudasOrdinarias(id_tesorero);
            }
            else if (tipo_deuda == "Extraordinaria" || tipo_deuda == "extraordinaria")
            {
                deudas = objDeudas.Consultar_DeudaExtra(id_tesorero);
            }
            else
            {
                return BadRequest("Tipo de deuda no válido.");
            }

            // Crear un nuevo documento PDF
            PdfDocument document = new PdfDocument();

            // Añadir una página al documento
            PdfPage page = document.AddPage();

            // Obtener el objeto XGraphics para dibujar en la página
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Dibujar el encabezado
            XFont fontEncabezado = new XFont("Arial", 16);
            XSize textSize = gfx.MeasureString("Reporte de Deudas", fontEncabezado);
            double xPos = (page.Width - textSize.Width) / 2; // Centrar horizontalmente
            double yPos = 40; // Posición fija en la parte superior de la página
            gfx.DrawString("Reporte de Deudas", fontEncabezado, XBrushes.Black,
                new XRect(xPos, yPos, page.Width, page.Height),
                XStringFormats.TopLeft);

            // Dibujar la tabla de datos
            double tableWidth = page.Width - 40; // Ajustar para dejar márgenes
            yPos += 30; // Ajuste de la posición Y
            xPos = (page.Width - tableWidth) / 2; // Centrar la tabla horizontalmente

            // Ancho de cada columna (ajustado)
            double[] columnWidths = { 30, 50, 100, 130, 60, 60, 50, 90 }; // Ancho de cada columna en orden

            // Encabezados de columna
            string[] headers = { "ID", "Monto", "Nombre", "Descripción", "Días", "Periodicidad", "Recargo", "Próximo Pago" };
            XFont fontHeader = new XFont("Arial", 12);
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawRectangle(XBrushes.LightGray, xPos, yPos, columnWidths[i], 30); // Altura fija de celda
                gfx.DrawString(headers[i], fontHeader, XBrushes.Black,
                    new XRect(xPos, yPos, columnWidths[i], 30),
                    XStringFormats.Center);
                xPos += columnWidths[i];
            }
            yPos += 30; // Ajuste de la posición Y

            // Datos de las deudas
            XFont fontData = new XFont("Arial", 12);
            foreach (var deuda in deudas)
            {
                xPos = (page.Width - tableWidth) / 2; // Centrar horizontalmente

                // Dibujar cada celda de datos
                for (int i = 0; i < headers.Length; i++)
                {
                    gfx.DrawRectangle(XBrushes.White, xPos, yPos, columnWidths[i], 20); // Altura fija de celda
                    string data = "";
                    switch (i)
                    {
                        case 0: data = deuda.id_deudas.ToString(); break;
                        case 1: data = deuda.monto.ToString("C"); break; // Formato de moneda
                        case 2: data = deuda.nombre; break;
                        case 3: data = deuda.descripcion; break;
                        case 4: data = deuda.dias_gracia.ToString(); break;
                        case 5: data = deuda.periodicidad.ToString(); break;
                        case 6: data = deuda.recargo.ToString(); break;
                        case 7: data = deuda.proximo_pago.ToString("dd/MM/yyyy"); break;
                    }
                    gfx.DrawString(data, fontData, XBrushes.Black,
                        new XRect(xPos, yPos, columnWidths[i], 20),
                        XStringFormats.Center);
                    xPos += columnWidths[i];
                }
                yPos += 20; // Ajuste de la posición Y
            }

            // Guardar el documento en un MemoryStream
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;

            // Devolver el PDF como un archivo para descargar
            return File(stream, "application/pdf", $"reporte_Deudas_{tipo_deuda}.pdf");
        }


        [HttpGet]
        [Route("Reporte_Deudores")]
        public IActionResult GenerarReporteDeudores(int id_fraccionamiento)
        {
            DeudasController objDeudas = new DeudasController();
            List<Deudoress> deudores = objDeudas.Consultar_DeudoresOrdinarios(id_fraccionamiento);

            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Encabezado
            XFont fontEncabezado = new XFont("Arial", 20, XFontStyleEx.Bold);
            gfx.DrawString("Reporte de deudores", fontEncabezado, XBrushes.DarkBlue,
                new XRect(0, 40, page.Width, 40), XStringFormats.TopCenter);

            // Configuración de márgenes y anchos
            double leftMargin = 20;  // Margen izquierdo
            double rightMargin = 20; // Margen derecho
            double tableWidth = page.Width - leftMargin - rightMargin; // Ancho total de la tabla
            double yPos = 90;
            double xPos = leftMargin; // Comenzar desde el margen izquierdo

            // Ancho de cada columna
            double[] columnWidths = { 150, 150, 120, 80, 80 }; // Ajustado para eliminar ID y Tipo de Deuda
            string[] headers = { "Persona", "Nombre de deuda", "Monto", "Fecha" };
            XFont fontHeader = new XFont("Arial", 12, XFontStyleEx.Bold);
            XFont fontData = new XFont("Arial", 10);

            // Dibujar encabezados
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawRectangle(XBrushes.LightSkyBlue, xPos, yPos, columnWidths[i], 35);
                gfx.DrawString(headers[i], fontHeader, XBrushes.Black,
                    new XRect(xPos, yPos, columnWidths[i], 35), XStringFormats.Center);
                xPos += columnWidths[i];
            }
            yPos += 35;

            // Dibujar datos de deudores
            for (int j = 0; j < deudores.Count; j++)
            {
                var deudor = deudores[j];
                xPos = leftMargin; // Reiniciar xPos al margen izquierdo
                for (int i = 0; i < headers.Length; i++)
                {
                    gfx.DrawRectangle(XBrushes.White, xPos, yPos, columnWidths[i], 35);
                    string data = "";
                    switch (i)
                    {
                        case 0:
                            data = deudor.nombre_persona;
                            if (gfx.MeasureString(data, fontData).Width > columnWidths[i])
                            {
                                // Acortar el nombre si es demasiado largo
                                data = data.Substring(0, Math.Min(data.Length, 20)) + "...";
                            }
                            break;
                        case 1: data = deudor.nombre_deuda; break;
                        case 2: data = deudor.monto.ToString("C"); break;
                        case 3: data = DateTime.Parse(deudor.proximo_pago).ToString("yyyy-MM-dd"); break;
                    }
                    gfx.DrawString(data, fontData, XBrushes.Black,
                        new XRect(xPos, yPos, columnWidths[i], 25), XStringFormats.Center);
                    xPos += columnWidths[i];
                }
                yPos += 25;

                // Dibujar línea inferior solo si no es el último registro
                if (j < deudores.Count - 1)
                {
                    // Dibujar línea inferior
                    gfx.DrawLine(XPens.Gray, leftMargin, yPos, leftMargin + 500, yPos); // Ajusta "500" para cambiar el largo de la línea

                }
            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;

            return File(stream, "application/pdf", $"reporte_deudores_fraccionamiento_{id_fraccionamiento}.pdf");
        }





        [HttpGet]
        [Route("GenerarRecibo")]
        public IActionResult GenerarRecibo(string nombrePersona, string idDeuda, int id_fraccionamiento)
        {
            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                string consulta = "SELECT * FROM deudores WHERE (nombre_persona = @nombrePersona AND id_deuda = @idDeuda) AND id_fraccionamiento=@id_fraccionamiento";
                MySqlCommand comando = new MySqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@nombrePersona", nombrePersona);
                comando.Parameters.AddWithValue("@idDeuda", idDeuda);
                comando.Parameters.AddWithValue("@id_fraccionamiento", id_fraccionamiento);
                conexion.Open();

                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        // Crear documento PDF
                        PdfDocument document = new PdfDocument();
                        PdfPage page = document.AddPage();
                        XGraphics gfx = XGraphics.FromPdfPage(page);
                        double yPos = 40; // Posición inicial

                        // Logo de la empresa
                        //XImage logo = XImage.FromFile("assets/disenio.png");


                        string path = Path.Combine(Directory.GetCurrentDirectory(), "assets/disenio.png");
                        if (!System.IO.File.Exists(path))
                        {
                            return NotFound("El archivo de imagen no se encontró.");
                        }
                        XImage logo = XImage.FromFile(path);




                        gfx.DrawImage(logo, 20, yPos, 100, 50);

                        // Título y nombre de la empresa
                        XFont fontTitulo = new XFont("Arial", 18);
                        XFont fontEmpresa = new XFont("Arial", 12);
                        XFont fontContacto = new XFont("Arial", 10);

                        gfx.DrawString("Recibo de Pago", fontTitulo, XBrushes.Black,
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
                        gfx.DrawString($"Nombre del Deudor:", fontEmpresa, XBrushes.Black, 20, yPos);
                        gfx.DrawString(nombrePersona, fontEmpresa, XBrushes.Black, 150, yPos);

                        yPos += 20;
                        gfx.DrawString($"Deuda ID:", fontEmpresa, XBrushes.Black, 20, yPos);
                        gfx.DrawString(idDeuda, fontEmpresa, XBrushes.Black, 150, yPos);

                        yPos += 20;
                        gfx.DrawString($"Fecha de Pago:", fontEmpresa, XBrushes.Black, 20, yPos);
                        gfx.DrawString(Convert.ToDateTime(lector["proximo_pago"]).ToString("dd/MM/yyyy"), fontEmpresa, XBrushes.Black, 150, yPos);

                        yPos += 20;
                        gfx.DrawString($"Estado de la Deuda:", fontEmpresa, XBrushes.Black, 20, yPos);
                        gfx.DrawString(lector["estado"].ToString(), fontEmpresa, XBrushes.Black, 150, yPos);

                        yPos += 40;
                        gfx.DrawLine(XPens.Gray, 20, yPos, page.Width - 20, yPos);
                        yPos += 10;

                        XFont fontResumen = new XFont("Arial", 14);
                        gfx.DrawString("Resumen del Pago", fontResumen, XBrushes.Black,
                            new XRect(0, yPos, page.Width, 20), XStringFormats.TopCenter);

                        yPos += 30;
                        gfx.DrawString("Monto Total:", fontEmpresa, XBrushes.Black, 20, yPos);
                        gfx.DrawString($"${Convert.ToDouble(lector["monto"]).ToString("F2")}", fontEmpresa, XBrushes.Black, page.Width - 150, yPos);

                        yPos += 20;
                        gfx.DrawString("Recargos:", fontEmpresa, XBrushes.Black, 20, yPos);
                        gfx.DrawString($"${Convert.ToDouble(lector["recargo"]).ToString("F2")}", fontEmpresa, XBrushes.Black, page.Width - 150, yPos);

                        yPos += 20;
                        gfx.DrawString("Días de Gracia:", fontEmpresa, XBrushes.Black, 20, yPos);
                        gfx.DrawString(lector["dias_gracia"].ToString(), fontEmpresa, XBrushes.Black, page.Width - 150, yPos);

                        yPos += 40;
                        gfx.DrawString("________________________", fontEmpresa, XBrushes.Black, 20, yPos);
                        yPos += 20;
                        gfx.DrawString("Firma del Emisor", fontContacto, XBrushes.Black, 20, yPos);

                        yPos += 30;
                        gfx.DrawString("Gracias por su pago. Si tiene alguna pregunta, comuníquese con nosotros.", fontContacto, XBrushes.Black, 20, yPos);

                        MemoryStream stream = new MemoryStream();
                        document.Save(stream, false);
                        stream.Position = 0;

                        // Devolver el PDF con el nombre idDeuda_Nombrepersona
                        string nombreArchivo = $"{idDeuda}_{nombrePersona}.pdf";
                        return File(stream, "application/pdf", nombreArchivo);
                    }
                    else
                    {
                        return NotFound("No se encontraron registros para la persona y deuda especificada.");
                    }
                }
            }
        }


        [HttpGet]
        [Route("Reporte_Proveedores")]
        public IActionResult GenerarReporteProveedores(int id_fraccionamiento)
        {
            // Crear una instancia del controlador o directamente llamar al método
            var objProveedores = new ProveedoresController();
            List<Proveedores> proveedores = objProveedores.Consultar_Proveedores(id_fraccionamiento);

            // Crear un documento PDF
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Encabezado
            XFont fontEncabezado = new XFont("Arial", 20, XFontStyleEx.Bold);
            gfx.DrawString("Reporte de Proveedores", fontEncabezado, XBrushes.DarkBlue,
                new XRect(0, 40, page.Width, 40), XStringFormats.TopCenter);

            // Configuración de márgenes y anchos
            double leftMargin = 20;  // Margen izquierdo
            double rightMargin = 20; // Margen derecho
            double tableWidth = page.Width - leftMargin - rightMargin; // Ancho total de la tabla
            double yPos = 90;
            double xPos = leftMargin + 10; // Ajuste hacia la izquierda (ligero)

            // Ancho de cada columna (ajustado para un mejor balance)
            double[] columnWidths = { 40, 85, 70, 70, 70, 70, 85, 70 }; // Anchos de columnas equilibrados
            string[] headers = { "ID", "Nombre", "A. Paterno", "A. Materno", "Teléfono", "Tipo", "Dirección", "Función" };
            XFont fontHeader = new XFont("Arial", 12, XFontStyleEx.Bold);
            XFont fontData = new XFont("Arial", 12);

            // Dibujar encabezados
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawRectangle(XBrushes.LightSkyBlue, xPos, yPos, columnWidths[i], 35);
                gfx.DrawString(headers[i], fontHeader, XBrushes.Black,
                    new XRect(xPos, yPos, columnWidths[i], 35), XStringFormats.Center);
                xPos += columnWidths[i];
            }
            yPos += 35;

            // Dibujar datos de proveedores
            foreach (var proveedor in proveedores)
            {
                xPos = leftMargin + 10; // Reiniciar xPos al margen izquierdo con ligero ajuste
                for (int i = 0; i < headers.Length; i++)
                {
                    gfx.DrawRectangle(XBrushes.White, xPos, yPos, columnWidths[i], 25);
                    string data = "";
                    switch (i)
                    {
                        case 0: data = proveedor.id_proveedor.ToString(); break;
                        case 1: data = proveedor.nombre; break;
                        case 2: data = proveedor.apellido_paterno; break;
                        case 3: data = proveedor.apellido_materno; break;
                        case 4: data = proveedor.telefono; break;
                        case 5: data = proveedor.tipo; break;
                        case 6: data = proveedor.direccion; break;
                        case 7: data = proveedor.funcion; break;
                    }
                    gfx.DrawString(data, fontData, XBrushes.Black,
                        new XRect(xPos, yPos, columnWidths[i], 25), XStringFormats.Center);
                    xPos += columnWidths[i];
                }
                yPos += 25;

                // Dibujar línea inferior
                gfx.DrawLine(XPens.Gray, leftMargin, yPos, page.Width - rightMargin, yPos);
            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;

            return File(stream, "application/pdf", $"reporte_ProveedoresFraccionamiento_{id_fraccionamiento}.pdf");
        }

        [HttpGet]
        [Route("Reporte_IngresosDeudas")]
        public IActionResult GenerarReporteHistorialDeudas(int id_fraccionamiento)
        {
            // Crear una instancia del controlador de deudas
            DeudasController objDeudas = new DeudasController();
            List<h_deuda> historial_deudas = objDeudas.Consultar_HistorialDeudas(id_fraccionamiento);

            // Verificar si se obtuvo algún historial
            if (historial_deudas == null || historial_deudas.Count == 0)
            {
                return NotFound("No se encontraron deudas en el historial.");
            }

            // Crear un nuevo documento PDF
            PdfDocument document = new PdfDocument();

            // Añadir una página al documento
            PdfPage page = document.AddPage();

            // Obtener el objeto XGraphics para dibujar en la página
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Dibujar el encabezado
            XFont fontEncabezado = new XFont("Arial", 14);
            XSize textSize = gfx.MeasureString("Reporte de Ingresos", fontEncabezado);
            double xPos = (page.Width - textSize.Width) / 2;
            double yPos = 30;
            gfx.DrawString("Reporte de Ingresos", fontEncabezado, XBrushes.Black,
                new XRect(xPos, yPos, page.Width, page.Height),
                XStringFormats.TopLeft);

            // Ajustes para la tabla de datos
            double tableWidth = page.Width - 40; // Ajustar para dejar márgenes
            yPos += 40; // Ajuste de la posición Y
            double xPosBase = 10; // Margen izquierdo ajustado más a la izquierda

            // Ancho de cada columna ajustado
            double[] columnWidths = { 30, 110, 80, 90, 60, 60, 70, 80 }; // Manteniendo el ancho de las columnas
            double totalWidth = columnWidths.Sum(); // Sumar los anchos de las columnas

            // Encabezados de columna
            string[] headers = { "#", "Deudor", "Tipo de deuda", "Descripción", "Monto", "Recargo", "Estado", "Fecha" };
            XFont fontHeader = new XFont("Arial", 10); // Tamaño de letra reducido

            // Asegurarse que los encabezados y los anchos de columna tienen la misma longitud
            int numColumns = Math.Min(headers.Length, columnWidths.Length);

            // Dibujar encabezados de columna
            for (int i = 0; i < numColumns; i++)
            {
                gfx.DrawRectangle(XBrushes.LightGray, xPosBase, yPos, columnWidths[i], 25); // Altura fija de celda
                gfx.DrawString(headers[i], fontHeader, XBrushes.Black,
                    new XRect(xPosBase, yPos, columnWidths[i], 25),
                    XStringFormats.Center);
                xPosBase += columnWidths[i];
            }
            yPos += 25; // Ajuste de la posición Y

            // Datos del historial de deudas
            XFont fontData = new XFont("Arial", 9); // Tamaño de letra reducido
            foreach (var deuda in historial_deudas)
            {
                xPosBase = 10; // Reiniciar posición X para cada fila (ajustado más a la izquierda)

                // Dibujar cada celda de datos
                for (int i = 0; i < numColumns; i++)
                {
                    string dataValue = "";
                    switch (i)
                    {
                        case 0: dataValue = deuda.id_deuda.ToString(); break; // Campo "id_deuda"
                        case 1: dataValue = deuda.nombre_persona; break; // Deudor
                        case 2: dataValue = deuda.tipo_deuda; break; // Tipo de deuda
                        case 3: dataValue = deuda.nombre_deuda; break; // Descripción
                        case 4: dataValue = deuda.monto.ToString("C"); break; // Monto
                        case 5: dataValue = deuda.recargo.ToString("C"); break; // Recargo
                        case 6: dataValue = deuda.estado; break; // Estado
                        case 7: dataValue = deuda.dia_registro; break; // Fecha
                        default: break;
                    }

                    gfx.DrawRectangle(XBrushes.White, xPosBase, yPos, columnWidths[i], 20);
                    gfx.DrawString(dataValue, fontData, XBrushes.Black,
                        new XRect(xPosBase, yPos, columnWidths[i], 20),
                        XStringFormats.Center);
                    xPosBase += columnWidths[i];
                }

                yPos += 20; // Ajuste de la posición Y
            }

            // Guardar el documento en un MemoryStream
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;

            // Devolver el PDF como un archivo para descargar
            return File(stream, "application/pdf", $"reporte_HistorialDeudas_{id_fraccionamiento}.pdf");
        }

        [HttpGet]
        [Route("Reporte_Egresos")]
        public IActionResult GenerarReporteEgresos(int id_fraccionamiento)
        {
            EgresosController objEgresos = new EgresosController();
            List<Egresos> egresos = objEgresos.Consultar_Egresos(id_fraccionamiento);

            // Crear un nuevo documento PDF
            PdfDocument document = new PdfDocument();

            // Añadir una página al documento
            PdfPage page = document.AddPage();

            // Obtener el objeto XGraphics para dibujar en la página
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Dibujar el encabezado
            XFont fontEncabezado = new XFont("Arial", 16);
            XSize textSize = gfx.MeasureString("Reporte de Egresos", fontEncabezado);
            double xPos = (page.Width - textSize.Width) / 2; // Centrar horizontalmente
            double yPos = 40; // Posición fija en la parte superior de la página
            gfx.DrawString("Reporte de Egresos", fontEncabezado, XBrushes.Black,
                new XRect(xPos, yPos, page.Width, page.Height),
                XStringFormats.TopLeft);

            // Dibujar la tabla de datos
            double tableWidth = 500; // Ancho total de la tabla
            yPos += 30; // Ajuste de la posición Y
            xPos = (page.Width - tableWidth) / 2; // Centrar la tabla horizontalmente
            xPos -= 40; // Ajuste hacia la izquierda para centrar más

            // Ancho de cada columna (ajustado)
            double[] columnWidths = { 30, 100, 160, 140, 80, 60 }; // Ancho de cada columna en orden

            // Encabezados de columna
            string[] headers = { "#", "Concepto", "Descripción", "Proveedor", "Monto", "Fecha" };
            XFont fontHeader = new XFont("Arial", 12);
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawRectangle(XBrushes.LightGray, xPos, yPos, columnWidths[i], 30); // Altura fija de celda
                gfx.DrawString(headers[i], fontHeader, XBrushes.Black,
                    new XRect(xPos, yPos, columnWidths[i], 30),
                    XStringFormats.Center);
                xPos += columnWidths[i];
            }
            yPos += 30; // Ajuste de la posición Y

            // Datos de los egresos
            XFont fontData = new XFont("Arial", 12);
            foreach (var egreso in egresos)
            {
                xPos = (page.Width - tableWidth) / 2; // Centrar horizontalmente
                xPos -= 40; // Ajuste hacia la izquierda para centrar más

                // Dibujar cada celda de datos
                gfx.DrawRectangle(XBrushes.White, xPos, yPos, columnWidths[0], 20); // ID Egreso
                gfx.DrawString(egreso.id_egreso.ToString(), fontData, XBrushes.Black,
                    new XRect(xPos, yPos, columnWidths[0], 20),
                    XStringFormats.Center);
                xPos += columnWidths[0];

                gfx.DrawRectangle(XBrushes.White, xPos, yPos, columnWidths[1], 20); // Concepto
                gfx.DrawString(egreso.concepto, fontData, XBrushes.Black,
                    new XRect(xPos, yPos, columnWidths[1], 20),
                    XStringFormats.Center);
                xPos += columnWidths[1];

                gfx.DrawRectangle(XBrushes.White, xPos, yPos, columnWidths[2], 20); // Descripción
                gfx.DrawString(egreso.descripcion, fontData, XBrushes.Black,
                    new XRect(xPos, yPos, columnWidths[2], 20),
                    XStringFormats.Center);
                xPos += columnWidths[2];

                gfx.DrawRectangle(XBrushes.White, xPos, yPos, columnWidths[3], 20); // Proveedor
                gfx.DrawString(egreso.proveedor, fontData, XBrushes.Black,
                    new XRect(xPos, yPos, columnWidths[3], 20),
                    XStringFormats.Center);
                xPos += columnWidths[3];

                gfx.DrawRectangle(XBrushes.White, xPos, yPos, columnWidths[4], 20); // Monto
                gfx.DrawString(egreso.monto.ToString("C2", System.Globalization.CultureInfo.CurrentCulture), fontData, XBrushes.Black,
                    new XRect(xPos, yPos, columnWidths[4], 20),
                    XStringFormats.Center);
                xPos += columnWidths[4];

                gfx.DrawRectangle(XBrushes.White, xPos, yPos, columnWidths[5], 20); // Fecha
                if (DateTime.TryParse(egreso.fecha, out DateTime fecha))
                {
                    gfx.DrawString(fecha.ToString("dd/MM/yyyy"), fontData, XBrushes.Black,
                        new XRect(xPos, yPos, columnWidths[5], 20),
                        XStringFormats.Center);
                }
                else
                {
                    gfx.DrawString("N/A", fontData, XBrushes.Black,
                        new XRect(xPos, yPos, columnWidths[5], 20),
                        XStringFormats.Center);
                }

                yPos += 20; // Ajuste de la posición Y
            }

            // Guardar el documento en un MemoryStream
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;

            // Devolver el PDF como un archivo para descargar
            return File(stream, "application/pdf", $"reporte_Egresos.pdf");
        }





















        // Clase para representar las deudas
        public class Deuda
        {
            public int Id { get; set; }
            public string NombrePersona { get; set; }
            public string NombreDeuda { get; set; }
            public decimal Monto { get; set; }
            public decimal Recargo { get; set; }
            public string Estado { get; set; }
            public DateTime ProximoPago { get; set; }
        }
    }
}
