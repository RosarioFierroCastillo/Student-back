using API_Archivo.Clases;
using CardManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace API_Archivo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {


        [HttpPost]
        [Route("Agregar_Usuario")]
        public bool Agregar_Usuario([FromBody] Personas request)
        {
            bool Persona_agregada = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                // Ruta de la imagen por defecto
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "assets", "default.jpg");

                // Verificar si el archivo de imagen existe
                byte[] imageBytes;
                if (System.IO.File.Exists(imagePath))
                {
                    imageBytes = System.IO.File.ReadAllBytes(imagePath); // Leer la imagen como arreglo de bytes
                }
                else
                {
                    // Manejar el caso en que la imagen no exista
                    return false;
                }

                // Comando SQL actualizado para incluir el campo de imagen
                MySqlCommand comando = new MySqlCommand(
                    "INSERT INTO personas (nombre, apellido_pat, apellido_mat, telefono, fecha_nacimiento, id_fraccionamiento, id_lote,codigo_acceso, correo, contrasenia, id_administrador, tipo_usuario, hikvision, imagen) " +
                    "VALUES (@Nombre, @Apellido_pat, @Apellido_mat, @Telefono, @Fecha_nacimiento, @id_fraccionamiento, @id_lote,@codigo_acceso, @correo, @contrasenia, @id_administrador, @tipo_usuario, @hikvision, @Imagen)",
                    conexion
                );

                // Agregar parámetros
                comando.Parameters.Add("@Nombre", MySqlDbType.VarChar).Value = request.nombre;
                comando.Parameters.Add("@Apellido_pat", MySqlDbType.VarChar).Value = request.apellido_pat;
                comando.Parameters.Add("@Apellido_mat", MySqlDbType.VarChar).Value = request.apellido_mat;
                comando.Parameters.Add("@Telefono", MySqlDbType.VarChar).Value = request.telefono;
                comando.Parameters.Add("@Fecha_nacimiento", MySqlDbType.DateTime).Value = request.fecha_nacimiento1;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = request.id_fraccionamiento;
                comando.Parameters.Add("@id_lote", MySqlDbType.Int32).Value = request.id_lote;
                comando.Parameters.Add("@codigo_acceso", MySqlDbType.VarChar).Value = request.codigo_acceso;
                comando.Parameters.Add("@correo", MySqlDbType.VarChar).Value = request.correo;
                comando.Parameters.Add("@contrasenia", MySqlDbType.VarChar).Value = request.contrasenia;
                comando.Parameters.Add("@id_administrador", MySqlDbType.Int32).Value = request.id_administrador;
                comando.Parameters.Add("@tipo_usuario", MySqlDbType.VarChar).Value = request.tipo_usuario;
                comando.Parameters.Add("@hikvision", MySqlDbType.VarChar).Value = request.hikvision;
                comando.Parameters.Add("@Imagen", MySqlDbType.LongBlob).Value = imageBytes; // Agregar la imagen como LONGBLOB

                int id_administrador = request.id_administrador.Value;
                int id_fraccionamiento = request.id_fraccionamiento.Value;

                DateTime now = DateTime.Now;
                DateTime Dateproximo_pago = now.AddDays(90);
                string fechaProximoPago = Dateproximo_pago.ToString("yyyy-MM-ddTHH:mm:ss");
                string fechaActual = now.ToString("yyyy-MM-ddTHH:mm:ss");

                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Persona_agregada = true;

                        AddDevice.Login(id_fraccionamiento);
                        string ultima = Consultar_Ultima_Persona(id_administrador).ToString();
                        AddDevice.InsertUser(ultima, request.nombre, fechaActual, fechaProximoPago);

                        DateTime fechaActual2 = DateTime.Now;
                        string fechaString = fechaActual2.ToString("yyyy-MM-dd HH:mm"); // Formato: "2024-11-04 14:30
                        NotificacionesController obj_notificaciones = new NotificacionesController();
                        obj_notificaciones.Agregar_Notificacion(request.id_fraccionamiento.Value, "General", 0, "!Nuevo usuario!", $"{request.nombre} {request.apellido_pat} ahora forma parte de tu comunidad", fechaString);

                    }


                }
                catch (MySqlException ex)
                {
                    // Manejo de errores (puedes agregar aquí la lógica de logging si lo necesitas)
                }
                finally
                {
                    conexion.Close();
                }

                return Persona_agregada;
            }
        }





        [HttpPut]
        [Route("Actualizar_Contrasenia")]
        public bool Actualizar_Contrasenia(string correo, string contrasenia)
        {

                bool Persona_actualizada = false;

                using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
                {
                    int rowsaffected = 0;
                    MySqlCommand comando = new MySqlCommand("UPDATE personas " +
                        "SET Contrasenia=@Contrasenia " +
                        "WHERE Correo=@Correo", conexion);

                    comando.Parameters.Add("@Correo", MySqlDbType.VarChar).Value = correo;
                    comando.Parameters.Add("@Contrasenia", MySqlDbType.VarChar).Value = contrasenia;


                    try
                    {
                        conexion.Open();
                        rowsaffected = comando.ExecuteNonQuery();

                        if (rowsaffected >= 1)
                        {
                            Persona_actualizada = true;
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
                    return Persona_actualizada;

                
            }
        }

        /*
        [HttpGet]
        [Route("Consultar_Personas_Por_Fraccionamiento")]
        public List<Personas> Consultar_Personas_Por_Fraccionamiento(int id_fraccionamiento)
        {
            Personas obj_persona = new Personas();
            List<Personas> list_Personas = obj_persona.Consultar_Personas_Por_Fraccionamiento(id_fraccionamiento);
            return list_Personas;
        }
        */
        [HttpGet]
        [Route("Consultar_Personas_Por_Fraccionamientos")]
        public List<Personas> Consultar_Personas_Por_Fraccionamiento(int id_fraccionamiento)
        {
            List<Personas> Lista_Personas = new List<Personas>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM personas WHERE id_fraccionamiento=@id_fraccionamiento", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;



                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Lista_Personas.Add(new Personas()
                        {
                            id_persona = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                            nombre = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            apellido_pat = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            apellido_mat = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            telefono = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            tipo_usuario = reader.IsDBNull(13) ? "" : reader.GetString(13),
                            id_fraccionamiento = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                            id_lote = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                            correo = reader.IsDBNull(10) ? "" : reader.GetString(10)
                        });
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

                return Lista_Personas;
            }
        }


        [HttpGet]
        [Route("Generar_Token")]
        public IActionResult GenerarToken()
        {
            var token = Guid.NewGuid().ToString(); // Generar un token aleatorio utilizando Guid
            return new ContentResult
            {
                Content = token,
                ContentType = "text/plain",
                StatusCode = 200 // Código de estado OK (200)
            };
        }

        [HttpPost]
        [Route("Generar_invitacion")]
        public bool Generar_invitacion(string token, string correo_invitado, int id_fraccionamiento, string nombre_fraccionamiento, string nombre_admin, string tipo_usuario)
        {
            


            bool invitacion_agregada = false;
            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("insert into invitaciones (token, correo_invitado, id_fraccionamiento, nombre_fraccionamiento,nombre_administrador,tipo_usuario) VALUES (@token, @correo_invitado, @id_fraccionamiento, @nombre_fraccionamiento,@nombre_administrador,@tipo_usuario)", conexion);



                comando.Parameters.Add("@token", MySqlDbType.VarChar).Value = token;
                comando.Parameters.Add("@correo_invitado", MySqlDbType.VarChar).Value = correo_invitado;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.VarChar).Value = id_fraccionamiento;
                comando.Parameters.Add("@nombre_fraccionamiento", MySqlDbType.VarChar).Value = nombre_fraccionamiento;
                comando.Parameters.Add("@nombre_administrador", MySqlDbType.VarChar).Value = nombre_admin;
                comando.Parameters.Add("@tipo_usuario", MySqlDbType.VarChar).Value = tipo_usuario;


                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        invitacion_agregada = true;
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
                return invitacion_agregada;
            }
        }//

        [HttpGet]
        [Route("Consultar_invitacion")]
        public List<Invitaciones> Cosultar_invitacion(string token)
        {
            List<Invitaciones> Lista_invitacion = new List<Invitaciones>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM invitaciones WHERE token=@token", conexion);

                comando.Parameters.Add("@token", MySqlDbType.VarChar).Value = token;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Lista_invitacion.Add(new Invitaciones() { id_invitacion = reader.GetInt32(0), token = reader.GetString(1), correo_electronico = reader.GetString(2), id_fraccionamiento = reader.GetInt32(3), nombre_fraccionamiento = reader.GetString(5), nombre_admin = reader.GetString(6), tipo_usuario = reader.GetString(7) });
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

                return Lista_invitacion;
            }
        }


        /*
        [HttpGet]
        [Route("Consultar_Persona")]
        public List<Personas> Consultar_Persona(string nombre, string apellido_pat, string apellido_mat)
        {

            List<Personas> Persona = new List<Personas>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM personas where id_administrador=@id_administrador", conexion);
                //y id_fraccionamiento
                //  comando.Parameters.Add("@Nombre", MySqlDbType.Int32).Value = nombre;
                //  comando.Parameters.Add("@Apellido_pat", MySqlDbType.Int32).Value = apellido_pat;
                comando.Parameters.Add("@id_administrador", MySqlDbType.Int32).Value = id_administrador;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        DateTime ffecha = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5);


                        Persona.Add(new Personas()
                        {
                            id_persona = reader.GetInt32(0),
                            nombre = reader.GetString(1),
                            apellido_pat = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            apellido_mat = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            id_lote = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                            telefono = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            fecha_nacimiento = ffecha.ToString("yyyy-MM-dd"),
                            correo = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            contrasenia = reader.GetString(11),
                            id_fraccionamiento = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                            tipo_usuario = reader.GetString(13),
                            intercomunicador = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                            codigo_acceso = reader.IsDBNull(9) ? "0" : reader.GetString(9),



                        });
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

                return Persona;
            }
        }

        [HttpGet]
        [Route("Consultar_Correo")]
        public string Consultar_Correo(string id_persona)
        {
            string correo = "";
            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT Correo FROM personas WHERE id_Persona=@id_Persona", conexion);

                comando.Parameters.Add("@id_Persona", MySqlDbType.Int32).Value = id_persona;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        correo = reader.GetString(0);
                    }
                    else
                    {
                        correo = "";
                    }


                }
                catch (MySqlException ex)
                {

                }
                finally
                {
                    conexion.Close();
                }

                return correo;
            }
        }

        [HttpGet]
        [Route("Generar_Token")]
        public IActionResult GenerarToken()
        {
            var token = Guid.NewGuid().ToString(); // Generar un token aleatorio utilizando Guid
            return new ContentResult
            {
                Content = token,
                ContentType = "text/plain",
                StatusCode = 200 // Código de estado OK (200)
            };
        }//

        [HttpPost]
        [Route("Generar_Invitacion")]
        public string Generar_invitacion(string token, string correo_electronico, int id_fraccionamiento, int id_lote, string nombre_fraccionamiento, string nombre_admin, string tipo_usuario)
        {
            Invitaciones obj_invitacion = new Invitaciones();
            if (obj_invitacion.Generar_invitacion(token, correo_electronico, id_fraccionamiento, id_lote, nombre_fraccionamiento, nombre_admin, tipo_usuario))
            {
                return "Invitacion generada correctamente";
            }
            else
            {
                return "Error al generar la invitacion";
            }

        }//

        [HttpGet]
        [Route("Consultar_Invitacion")]
        public List<Invitaciones> Consultar_Invitacion(string token)
        {
            Invitaciones obj_invitacion = new Invitaciones();
            List<Invitaciones> lista_invitacion = obj_invitacion.Cosultar_invitacion(token);

            return lista_invitacion;
        }
        */
        [HttpGet]
        [Route("Consultar_Imagen")]
        public IActionResult Consultar_Imagen(int id_Persona)
        {
            Usuarios obj_usuario = new Usuarios();

            byte[] imagenBytes = obj_usuario.Consultar_Imagen(id_Persona);

            if(obj_usuario.Consultar_Imagen(id_Persona) == null)
            {
                return NotFound("La imagen solicitada no está disponible.");

            }
            else
            {
                // Devolver los bytes como contenido binario
                return File(imagenBytes, "image/jpeg"); // Cambia el tipo de contenido según el formato de tu imagen
            }
            
         
        }
        

        [HttpPost]
        [Route("Actualizar_Imagen")]

        public string Actualizar_Imagen(IFormFile file, int id_persona)
        {

            if (file.Length > 0)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    byte[] archivoEnBytes = memoryStream.ToArray(); // Convertir a byte[]

                    // Aquí puedes usar 'archivoEnBytes' como necesites
                    Usuarios obj_usuario = new Usuarios();
                    if (obj_usuario.Cargar_Imagen(archivoEnBytes, id_persona))
                    {
                        return "si jala";
                    }
                    else
                    {
                        return "no jala";
                    }

                }
            }
            return "hola";
        }
        
        [HttpGet]
        [Route("Consultar_Ultima_Persona")]

        public string Consultar_Ultima_Persona(int id_fraccionamiento)
        {
            // List<Personas> Lista_Personas = new List<Personas>();
            //  int id_persona = 0;
            string id_persona="0";

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM personas ORDER BY id_persona DESC LIMIT 1;", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;



                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        //   Lista_Personas.Add(new Personas()
                        //   {
                        //  return reader.GetInt32(0);
                        id_persona = reader.GetInt32(0).ToString();
                      //  });
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

                return id_persona;
            }
        }

        


    }
}