using Microsoft.AspNetCore.Mvc;
using System.Net;
using MySql.Data;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Cors;
using API_Archivo.Clases;

namespace API_Archivo.Controllers
{

    [ApiController]
    [Route("[controller]")]
    
    public class FileController : ControllerBase
    {
        
        private string _pathString = @"C:\prueba_archivos\";
        public static string cadena_conexion = "server=localhost;port=3306;user id=root;password=Bosschapo300;database=condominios2;";

       

        [HttpPost]
        [Route("Guardar")]
        
        public string Guardar_archivo(string nombre_archivo,IFormFile file)
        {
           int id_usuario = 1;
            string tipo_deuda = "Ordinaria", nombre_deuda = "Mantenimiento", ruta_comprobante="";
            double monto = 540.215;

            bool archivo_guardado = false;

            if(file.Length > 0)
            {
                string filePath = Path.Combine(_pathString, file.FileName);
                using(Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);

                    ruta_comprobante = _pathString + file.FileName;

                    if (Guardar_bd(id_usuario, tipo_deuda, nombre_deuda, monto, ruta_comprobante))
                    {
                        archivo_guardado = true;
                    }

                }
            }
            return "hola";
        }

        private bool Guardar_bd(int id_usuario, string tipo_deuda, string nombre_deuda, double monto, string ruta_comprobante)
        {
            bool archivo_guardado = false;
            using (MySqlConnection conexion = new MySqlConnection(cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("insert into pagos (id_usuario, Tipo_deuda, Nombre_deuda, Monto, Ruta_comprobante, Estado) VALUES ( @id_usuario, @Tipo_deuda,@Nombre_deuda, @Monto, @Ruta_comprobante, @Estado)", conexion);

                //@id_usuario, @Tipo_deuda,@Nombre_deuda, @Monto, @Ruta_comprobante, @Estado

                comando.Parameters.Add("@id_usuario", MySqlDbType.Int32).Value = id_usuario;
                comando.Parameters.Add("@Tipo_deuda", MySqlDbType.VarChar).Value = tipo_deuda;
                comando.Parameters.Add("@Nombre_deuda", MySqlDbType.VarChar).Value = nombre_deuda;
                comando.Parameters.Add("@Monto", MySqlDbType.Double).Value = monto;
                comando.Parameters.Add("@Ruta_comprobante", MySqlDbType.VarChar).Value = ruta_comprobante;
                comando.Parameters.Add("@Estado", MySqlDbType.VarChar).Value = "Por Aprobar";



                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        archivo_guardado = true;
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
                return archivo_guardado;

            }
        }
        

    }
}
