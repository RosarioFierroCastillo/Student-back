namespace API_Archivo.Clases
{
    public class Usuario_lote
    {
        public int id_usuario_lote { get; set; }
        public int id_usuario { get; set; }
        public int id_lote { get; set; }
        public int id_renta { get; set; }
        public int id_fraccionamiento { get; set; }
        public string? codigo_acceso { get; set; }
        public string? intercomunicador { get; set; }
        public string? nombre { get; set; }

    }
}
