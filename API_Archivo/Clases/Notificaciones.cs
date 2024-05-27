using MySql.Data.MySqlClient;

namespace API_Archivo.Clases
{
    public class Notificaciones
    {
        public int id_notificacion { get; set; }
        public int id_fraccionamiento { get; set; }
        public string tipo { get; set; }
        public int id_destinatario { get; set; }
        public string asunto { get; set; }
        public string mensaje { get; set; }


    }
}
