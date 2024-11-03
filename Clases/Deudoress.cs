﻿namespace API_Archivo.Clases
{
    public class Deudoress
    {
        public int id_deuda { get; set; }
        public int id_deudor { get; set; }
        public int id_fraccionamiento { get; set; }
        public string? nombre_persona { get; set; }
        public int lote { get; set; }
        public string? tipo_deuda { get; set; }
        public string? nombre_deuda { get; set; }
        public string? persona { get; set; }
        public float monto { get; set; }
        public float recargo { get; set; }
        public int dias_gracia { get; set; }
        public string? proximo_pago { get; set; }
        public string? estado { get; set; }

        public string? concepto { get; set; }
        public int periodicidad { get; set; }

        public float monto_restante { get; set; }
    }
}



/*
 * 
 * public int id_deuda { get; set; }
        public string concepto { get; set; }
        public string persona { get; set; }
        public float monto { get; set; }
        public DateTime proximo_pago { get; set; }
 * 
 * */