using MySql.Data.MySqlClient;

namespace API_Archivo.Clases
{
    public class Acuerdos
    {

        public int id_acuerdo {  get; set; }
        public int id_fraccionamiento {  get; set; }
        public string asunto { get; set; }
        public string detalles {  get; set; }

        public string fecha {  get; set; }

    }
}
