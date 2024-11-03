namespace API_Archivo.Clases
{
    public class Grupos
    {
        public int id_fraccionamiento { get; set; }
        public int id_grupo { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int usuarios { get; set; }

    }
    public class Persona
    {
        public int id { get; set; }
        public int id_persona { get; set; }
        public int id_grupo { get; set; }
        public string nombre { get; set; }
        public string apellido_pat { get; set; }
        public string apellido_mat { get; set; }


    }


}
