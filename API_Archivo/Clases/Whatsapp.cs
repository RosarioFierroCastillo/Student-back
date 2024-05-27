using System.Net.Http.Headers;

namespace API_Archivo.Clases
{
    public class Whatsapp
    {

        public async Task Enviar_MensajeAsync()
        {
            //Token
            string token = "EAAacHBJYtX0BO9ZACZC9aTAtlBRiKVsZC4RU3gpAhz87jON5Bc6x6ZAQarJ4ORYzBYcYBMlWpYORlZAj1S4q3EolK4vQpEnvZAn0Tr1Xegu8we38qxBXHfRmhJZArSKmKZBP8G4WlFGfKyxprtPW6DiUkaZAPybVIs8yJkCv8jDDIx5QZA8Y19hw9ZCpPelUmiWxM1yZCCz4T0DaqQJUBdIrkP4ZD";
            //Identificador de número de teléfono
            string idTelefono = "180074601858172";
            //Nuestro telefono
            string telefono = "526681908522";
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://graph.facebook.com/v15.0/" + idTelefono + "/messages");
            request.Headers.Add("Authorization", "Bearer " + token);
            request.Content = new StringContent("{ \"messaging_product\": \"whatsapp\", \"to\": \"" + telefono + "\", \"type\": \"template\", \"template\": { \"name\": \"codigo_qr\", \"language\": { \"code\": \"es_MX\" } } }"); request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            //response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
        }
    }
}
