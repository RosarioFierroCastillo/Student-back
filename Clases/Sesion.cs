using CardManagement;
using MySql.Data.MySqlClient;
using System.Threading;

namespace API_Archivo.Clases
{
    public class Sesion
    {

        public string correo { get; set; }
        public int id_usuario { get; set; }
        public string tipo_usuario { get; set; }

        public int id_fraccionamiento { get; set; }

        public string fraccionamiento { get; set; }
        public int id_lote { get; set; }

        public int id_tesorero { get; set; }


        public string nombre { get; set; }

        public string con_nombre { get; set; }

        public bool conexion { get; set; }

        public bool dark_mode { get; set; }


        public string ip {  get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public string port { get; set; }

        public string client_key { get; set; }
        public string secret_key { get; set; }


        public string hikvision { get; set; }

    }

}