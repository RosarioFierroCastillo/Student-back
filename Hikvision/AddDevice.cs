using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UserManagement;
using API_Archivo.Clases;
using MySql.Data.MySqlClient;
using System.Xml;

namespace CardManagement
{
    public partial class AddDevice
    {



        public static List<Hikvision> Consultar_Hikvision(int id_fraccionamiento)
        {

            List<Hikvision> Lista_acuerdos = new List<Hikvision>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM controlador WHERE id_fraccionamiento=@id_fraccionamiento AND estado = 'habilitado'", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Lista_acuerdos.Add(new Hikvision()
                        {
                            id_controlador = reader.GetInt32(0),
                            //nombre = reader.GetString(7),

                            user = reader.GetString(2),

                            password = reader.GetString(3),

                            port = reader.GetString(4),




                            ip = reader.GetString(5)

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

                return Lista_acuerdos;
            }


        }

        public static DeviceInfo struDeviceInfo;

        public static string ActionISAPI(string szUrl, string szRequest, string szMethod)
        {
            string szResponse = string.Empty;

            if (!szUrl.Substring(0, 4).Equals("http"))
            {

                if (AddDevice.struDeviceInfo.strHttpPort.Equals(""))
                {
                    szUrl = "http://" + AddDevice.struDeviceInfo.strDeviceIP + szUrl;
                    //szUrl = AddDevice.struDeviceInfo.strDeviceIP + szUrl;

                }
                else
                {
                    szUrl = "http://" + AddDevice.struDeviceInfo.strDeviceIP + AddDevice.struDeviceInfo.strHttpPort + szUrl;

                }
            }
            HttpClient clHttpClient = new HttpClient();
            byte[] byResponse = { 0 };
            int iRet = 0;
            string szContentType = string.Empty;

            switch (szMethod)
            {
                case "GET":
                    iRet = clHttpClient.HttpRequest(AddDevice.struDeviceInfo.strUsername, AddDevice.struDeviceInfo.strPassword, szUrl, szMethod, ref byResponse, ref szContentType);
                    break;
                case "PUT":
                    iRet = clHttpClient.HttpPut(AddDevice.struDeviceInfo.strUsername, AddDevice.struDeviceInfo.strPassword, szUrl, szMethod, szRequest, ref szResponse);
                    break;
                case "POST":
                    iRet = clHttpClient.HttpPut(AddDevice.struDeviceInfo.strUsername, AddDevice.struDeviceInfo.strPassword, szUrl, szMethod, szRequest, ref szResponse);
                    break;
                default:
                    break;
            }

            if (iRet == (int)CardManagement.HttpClient.HttpStatus.Http200)
            {
                if ((!szMethod.Equals("GET")) || (szContentType.IndexOf("application/xml") != -1))
                {
                    if (szResponse != string.Empty)
                    {
                        return szResponse;
                    }

                    if (szMethod.Equals("GET"))
                    {
                        szResponse = Encoding.Default.GetString(byResponse);
                        return szResponse;
                    }
                }
                else
                {
                    if (byResponse.Length != 0)
                    {
                        szResponse = Encoding.Default.GetString(byResponse);
                        return szResponse;
                    }
                }
            }
            else if (iRet == (int)HttpClient.HttpStatus.HttpOther)
            {
                string szCode = string.Empty;
                string szError = string.Empty;
                clHttpClient.ParserResponseStatus(szResponse, ref szCode, ref szError);
                Console.WriteLine("Pedido fallido, error:" + szCode + " Describe:" + szError + "\r\n");
                return string.Empty;
            }
            else if (iRet == (int)HttpClient.HttpStatus.HttpTimeOut)
            {
                Console.WriteLine(szMethod + " " + szUrl + "error! tiempo agotado");
                return string.Empty;
            }
            return szResponse;
        }

        //Add Hikvision device
        public static bool Login(int id_fraccionamiento)
        {
            //    string textBoxUserName = "admin", textBoxPassword = "Repara123",
            //    textBoxDeviceAddress = "187.216.118.73";
            //    string textBoxPort = "5551";

            List<Hikvision> controlador = new List<Hikvision>();

            controlador = AddDevice.Consultar_Hikvision(id_fraccionamiento);

            struDeviceInfo = new DeviceInfo();

            foreach (var dispositivo in controlador)
            {
                if (dispositivo.port.Equals("0")) { dispositivo.port = ""; } else { dispositivo.port = ":" + dispositivo.port; }
                struDeviceInfo.strUsername = dispositivo.user;
                struDeviceInfo.strPassword = dispositivo.password;
                struDeviceInfo.strDeviceIP = dispositivo.ip;
                struDeviceInfo.strHttpPort = dispositivo.port;

            }

            bool num = false;

            if (controlador.Count > 0)
            {
                if (Security.Login(struDeviceInfo))
                {
                    // user check success
                    num = true;
                    struDeviceInfo.bIsLogin = true;
                    Console.WriteLine("Hecho");
                }
            }


            return num;

        }

        public static bool InsertUser(string id_usuario, string nombre, string fechaActual, string fechaProximoPago)
        {
            string szUrl = "/ISAPI/AccessControl/UserInfo/SetUp?format=json";
            string szRequest = "{\"UserInfo\":{\"employeeNo\":\"" + id_usuario +
            "\",\"name\":\"" + nombre +
            "\",\"userType\":\"normal\",\"Valid\":{\"enable\":true,\"beginTime\":\"" + fechaActual + "\",\"endTime\":\"" + fechaProximoPago + "\"},\"doorRight\": \"1\",\"RightPlan\":[{\"doorNo\":1,\"planTemplateNo\":\"1\"}]}}";
            string szMethod = "PUT";

            string szResponse = ActionISAPI(szUrl, szRequest, szMethod);
            bool res = false;
            if (szResponse != string.Empty)
            {
                ResponseStatus rs = JsonConvert.DeserializeObject<ResponseStatus>(szResponse);
                if (1 == rs.statusCode)
                {
                    Console.WriteLine("Set UserInfo Succ!");
                    res = true;
                }
                else
                {
                    Console.WriteLine(rs.errorMsg);
                    res = false;
                }
            }
            return res;
        }

        public static bool InsertCardUser(string id_usuario, string codigo_acceso)
        {
            // var token = Guid.NewGuid().ToString();
            //    token.Substring(0, 3);
            // var randomNumber = new Random().Next(0, 1000);
            //    string token = randomNumber.ToString();

            string szUrl = "/ISAPI/AccessControl/CardInfo/SetUp?format=json";
            string szResponse = string.Empty;
            string szRequest = "{\"CardInfo\":{\"employeeNo\":\"" + id_usuario +
                "\",\"cardNo\":\"" + codigo_acceso +
                "\",\"cardType\":\"normalCard\"}}";
            string szMethod = "PUT";

            bool res = false;
            szResponse = ActionISAPI(szUrl, szRequest, szMethod);
            if (szResponse != string.Empty)
            {
                ResponseStatus rs = JsonConvert.DeserializeObject<ResponseStatus>(szResponse);
                if (1 == rs.statusCode)
                {
                    Console.WriteLine("Add Card Succ!");
                    res = true;
                }
                else
                {
                    Console.WriteLine(rs.errorMsg);
                    res = false;
                }
            }

            return res;
        }

        public static bool DeleteCardUser(string id_usuario)
        {
            string szUrl = "/ISAPI/AccessControl/CardInfo/Delete?format=json";
            string szResponse = string.Empty;
            string szRequest = "{\"CardInfoDelCond\":{\"CardNoList\":[{\"cardNo\":\"" + id_usuario + "\"}]}}";
            string szMethod = "PUT";

            szResponse = ActionISAPI(szUrl, szRequest, szMethod);

            bool res = false;

            if (szResponse != string.Empty)
            {
                ResponseStatus rs = JsonConvert.DeserializeObject<ResponseStatus>(szResponse);
                if (rs.statusString.Equals("OK"))
                {
                    Console.WriteLine("Delete Card Succ!");
                    res = true;
                }
                else
                {
                    Console.WriteLine(rs.statusString);
                }
            }
            return res;
        }


        public static bool DeleteAllUser(string id_usuario)
        {
            string szUrl = "/ISAPI/AccessControl/UserInfoDetail/Delete?format=json";
            string szResponse = string.Empty;
            string szRequest = "{\"UserInfoDetail\":{\"mode\":\"byEmployeeNo\",\"EmployeeNoList\":[{\"employeeNo\":\"" + id_usuario + "\"}]}}";
            string szMethod = "PUT";

            szResponse = ActionISAPI(szUrl, szRequest, szMethod);

            bool res = false;

            if (szResponse != string.Empty)
            {
                ResponseStatus rs = JsonConvert.DeserializeObject<ResponseStatus>(szResponse);
                if (rs.statusString.Equals("OK"))
                {
                    Console.WriteLine("Delete Card Succ!");
                    res = true;
                }
                else
                {
                    Console.WriteLine(rs.statusString);
                }
            }
            return res;
        }


        public static bool RestrictedUser(string id_usuario)
        {
            string szUrl = "/ISAPI/AccessControl/UserInfo/Modify?format=json";
            string szResponse = string.Empty;
            string szRequest = "{\"UserInfo\":{\"employeeNo\":\"" + id_usuario + "\",\"Valid\":{\"enable\":false}}}";
            string szMethod = "PUT";

            szResponse = ActionISAPI(szUrl, szRequest, szMethod);

            bool res = false;

            if (szResponse != string.Empty)
            {
                ResponseStatus rs = JsonConvert.DeserializeObject<ResponseStatus>(szResponse);
                if (rs.statusString.Equals("OK"))
                {
                    Console.WriteLine("Delete Card Succ!");
                    res = true;
                }
                else
                {
                    Console.WriteLine(rs.statusString);
                }
            }
            return res;
        }


        public static bool EnableUser(string id_usuario)
        {
            string szUrl = "/ISAPI/AccessControl/UserInfo/Modify?format=json";
            string szResponse = string.Empty;
            string szRequest = "{\"UserInfo\":{\"employeeNo\":\"" + id_usuario + "\",\"Valid\":{\"enable\":true}}}";
            string szMethod = "PUT";

            szResponse = ActionISAPI(szUrl, szRequest, szMethod);

            bool res = false;

            if (szResponse != string.Empty)
            {
                ResponseStatus rs = JsonConvert.DeserializeObject<ResponseStatus>(szResponse);
                if (rs.statusString.Equals("OK"))
                {
                    Console.WriteLine("Delete Card Succ!");
                    res = true;
                }
                else
                {
                    Console.WriteLine(rs.statusString);
                }
            }
            return res;
        }



        public static bool btnOpen_Click()
        {
            string szUrl = "/ISAPI/AccessControl/RemoteControl/door/1";//全部门
            string szResponse = string.Empty;
            string szRequest = "<RemoteControlDoor version=\"2.0\" xmlns=\"http://www.isapi.org/ver20/XMLSchema\"><cmd>open</cmd></RemoteControlDoor>";
            string szMethod = "PUT";

            szResponse = ActionISAPI(szUrl, szRequest, szMethod);

            bool res = false;

            if (szResponse != string.Empty)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(szResponse);
                XmlNode rootNode = xmlDoc.DocumentElement;
                for (int i = 0; i < rootNode.ChildNodes.Count; i++)
                {
                    if (rootNode.ChildNodes[i].Name.Equals("statusString"))
                    {
                        // MessageBox.Show("STAYOPEN DOOR " + rootNode.ChildNodes[i].InnerText);
                        res = true;
                        break;
                    }
                }
            }
            return res;

        }

        public static bool btnClose_Click()
        {
            string szUrl = "/ISAPI/AccessControl/RemoteControl/door/1";//全部门
            string szResponse = string.Empty;
            string szRequest = "<RemoteControlDoor version=\"2.0\" xmlns=\"http://www.isapi.org/ver20/XMLSchema\"><cmd>close</cmd></RemoteControlDoor>";
            string szMethod = "PUT";

            szResponse = ActionISAPI(szUrl, szRequest, szMethod);

            bool res = false;

            if (szResponse != string.Empty)
            {
                ResponseStatus rs = JsonConvert.DeserializeObject<ResponseStatus>(szResponse);
                if (rs.statusString.Equals("OK"))
                {
                    Console.WriteLine("Delete Card Succ!");
                    res = true;
                }
                else
                {
                    Console.WriteLine(rs.statusString);
                }
            }
            return res;

        }
    }
}