using MySql.Data.MySqlClient;

namespace API_Archivo.Clases
{
    public class Proveedores
    {

        public int id_proveedor {  get; set; }
        public int id_fraccionamiento{  get; set; }
        public string nombre { get; set; }
        public string apellido_paterno { get; set; }
        public string apellido_materno { get; set; }
        public string telefono { get; set; }
        public string  tipo { get; set; }
        public string direccion { get; set; }
        public string funcion { get; set; }





    }
}
