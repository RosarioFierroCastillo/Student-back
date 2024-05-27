using MySql.Data.MySqlClient;
using CardManagement;

namespace API_Archivo.Clases
{
    public class Renta
    {

        public int id_renta { get; set; }
        public int id_Persona { get; set; }
        public int id_fraccionamiento { get; set; }
        public int id_lote { get; set; }
        public float monto { get; set; }


    }
}
