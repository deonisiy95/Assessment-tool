using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DBUP
{
     class API {

        public static CookieContainer cookie = new CookieContainer();
        public static string call(string method, Dictionary<string, string> post_data = null) {

            try
            {
                // параметры запроса
                string param_list = "";

                // 
                if (post_data != null)
                {
                    // соберем параметры для запроса
                    foreach (KeyValuePair<string, string> keyValue in post_data)
                    {
                        param_list += String.Format("{0}={1}&", keyValue.Key, keyValue.Value);
                    }
                }


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Config.HOST + "/api/?api_method=" + method);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.AllowAutoRedirect = false;
                
                // укажем контейнер для куки
                request.CookieContainer = cookie;

                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                
                byte[] postByteArray = encoding.GetBytes(param_list);
                request.ContentLength = postByteArray.Length;

                System.IO.Stream postStream = request.GetRequestStream();
                postStream.Write(postByteArray, 0, postByteArray.Length);
                postStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataSteam = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataSteam);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataSteam.Close();
                response.Close();

                // обновим данные в файле cookie
                saveCookie();

                return responseFromServer;
            }
            catch (Exception ex)
            {
                //Если что-то пошло не так, выводим ошибочку о том, что же пошло не так.
                Logs.write("Response: " + ex);

                return "{\"status\":\"error\",\"response\":{\"message\":400}}";
            }
        }

        public static void saveCookie()
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream("cookies.bin", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, cookie);
                stream.Close();
            }
        }

        public static void initCookie()
        {
            try 
            {
                using (Stream stream = new FileStream("cookies.bin", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    try 
                    {
                        IFormatter formatter = new BinaryFormatter();
                        cookie = (CookieContainer)formatter.Deserialize(stream);
                    }
                    catch (Exception e)
                    {
                    }
                    stream.Close();
                }
            }
            catch (Exception e)
            {
            }
        }
       
    }
}
