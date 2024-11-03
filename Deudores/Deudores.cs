using API_Archivo.Clases;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace API_Archivo.Deudores
{
    public partial class Deudores
    {
        public static bool Agregar_Deudores(
             int id_deudas, int id_fraccionamiento, float monto, string nombre, DateTime proximo_pago)
            
        {

            bool fraccionamiento_agregado = false;
            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("INSERT INTO deudas_ordinarias(id_persona, " +
                    "id_deudas, id_fraccionamiento, monto, nombre, proximo_pago) " +
                    "SELECT id_renta, @id_deudas, @id_fraccionamiento, @monto, @nombre, @proximo_pago FROM lotes WHERE id_renta > 0;" +
                    "update deudas_ordinarias inner join personas on deudas_ordinarias.id_persona = personas.id_Persona set deudas_ordinarias.nombre_persona = CONCAT(personas.Nombre,' ', personas.Apellido_pat,' ', personas.Apellido_pat)", conexion);
       
                comando.Parameters.Add("@id_deudas", MySqlDbType.Int32).Value = id_deudas;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
                comando.Parameters.Add("@monto", MySqlDbType.Float).Value = monto;
                comando.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = nombre;
                comando.Parameters.Add("@proximo_pago", MySqlDbType.Date).Value = proximo_pago;

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

                }
                finally
                {

                }

                return fraccionamiento_agregado;
            }
        }


    }
}
