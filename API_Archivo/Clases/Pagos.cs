using MySql.Data.MySqlClient;

namespace API_Archivo.Clases
{
    public class Pagos
    {

        public int id_usuario { get; set; }

        public string nombre_persona { get; set; }
        public int id_fraccionamiento { get; set; }
        public double monto { get; set; }
        public string nombre_deuda { get; set; }
        public string fecha { get; set; }
        public string descripcion { get; set; }
        public int periodicidad { get; set; }

        public int dias_atraso { get; set; }
        public double Monto_total { get; set; }


        /*metodo "Registrari_Pago"

        lo que hace es actualizar la tabla "Pagos" para actualizar el campo "Fecha" y tener registrado en que fecha fue registrado
        el ultimo pago referente a la deuda y posteriormente sea mas facil sacar la cantidad de dias que tiene de adeudo el usuario

        El metodo va a ser llamado cuando el Tesorero reciba el pago de una deuda ordinaria por parte de un inquilino o propietario
        */
        public bool Registrar_pago(int id_usuario, string nombre_deuda, double cantidad)
        {
            bool resultado = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("UPDATE pagos " +
                    "SET Fecha=@fecha  " +
                    "WHERE id_usuario=@id_usuario && Nombre_deuda=@Nombre_deuda && Monto = @Monto", conexion);

                comando.Parameters.Add("@fecha", MySqlDbType.VarChar).Value = DateTime.Now.ToString("yyyy-MM-dd");
                comando.Parameters.Add("@Nombre_deuda", MySqlDbType.VarChar).Value = nombre_deuda;
                comando.Parameters.Add("@Monto", MySqlDbType.VarChar).Value = cantidad;
                comando.Parameters.Add("@id_usuario", MySqlDbType.VarChar).Value = id_usuario;


                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        resultado = true;
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
                return resultado;
            }

        }


        /*
         Metodo "Obtener_Atrasos" CAMBIALE EL NOMBRE SI QUIERES, SE LOS PUSE ASI MEDIO A LO LOCO 
         
        lo que hace este metodo de retornar una lista del tipo de clase Pagos que contiene a los usuarios registrados
        como deudores en la tabla Pagos, esta lista contiene los datos generales de la deuda, el nombre del deudor y 
        tambien la cantidad de dias que tiene de atraso en el pago.
        por ultimo tambien devuelve el monto_total de la deuda calculado tomando en cuenta la cantidad de dias de atraso que tiene 
        el usuario

        formula:

        monto_total= Monto_deuda * (Dias_atraso / periodicidad);
          
         * */
        public List<Pagos> Obtener_atrasos(int id_fraccionamiento)
        {

            int dias_atraso = 0;
            double monto_total = 0;
            List<Pagos> Lista_atrasos = new List<Pagos>();


            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM pagos WHERE id_fraccionamiento=@id_fraccionamiento", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        DateTime fechaPago = Convert.ToDateTime(reader["Fecha"]);
                        dias_atraso = (DateTime.Now - fechaPago).Days;
                        monto_total = reader.GetDouble(3) * (dias_atraso / reader.GetInt32(7));

                        Lista_atrasos.Add(new Pagos() { id_usuario = reader.GetInt32(1), nombre_persona = reader.GetString(2), dias_atraso = dias_atraso, periodicidad = reader.GetInt32(7), Monto_total = monto_total });
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

                return Lista_atrasos;
            }


        }

        public bool Cargar_Comprobante_pago(byte[] imagen_recibida)
        {
            bool resultado = false;
            string connectionString = Global.cadena_conexion;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO pagos(Comprobante) VALUES (@imagen)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@imagen", imagen_recibida);
                        int rowsaffected = command.ExecuteNonQuery();
                        if (rowsaffected > 0)
                        {
                            resultado = true;
                        }
                        else
                        {
                            resultado = false;
                        }
                    }
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar la imagen en la base de datos: " + ex.Message);
            }
            finally
            {

            }
            return resultado;

        }

        public byte[] ConsultarComprobantePago(int idPago)
        {
            byte[] imagen = null;
            string connectionString = Global.cadena_conexion;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Comprobante FROM pagos WHERE id_Pago = @id_Pago";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_Pago", idPago);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("Comprobante")))
                                {
                                    long size = reader.GetBytes(reader.GetOrdinal("Comprobante"), 0, null, 0, 0);
                                    imagen = new byte[size];
                                    reader.GetBytes(reader.GetOrdinal("Comprobante"), 0, imagen, 0, (int)size);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al consultar el comprobante de pago en la base de datos: " + ex.Message);
            }

            return imagen;
        }
    }
}
