using CardManagement;
using MySql.Data.MySqlClient;

namespace API_Archivo.Clases
{
    public class Hikvision
    {
        public int id_controlador { get; set; }
        public int id_administrador { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public string port { get; set; }
        public string ip { get; set; }


    }
}
