using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUP
{
    class Logging
    {
        string path = "log.txt";
        DateTime date = DateTime.Now; //en-GB
        public void WriteLog(int code, string text = "")
        {
            StreamWriter writer = new StreamWriter(path, true, Encoding.Default);
            switch (code)
            {
                case 1:
                    writer.WriteLine(String.Format("{0}: Вход в систему. Пользователь: {1}", date.ToString(), text));
                    break;
                case 2:
                    writer.WriteLine(String.Format("{0}: Попытка входа", date.ToString()));
                    break;
                case 3:
                    writer.WriteLine(String.Format("{0}: Регистрация нового пользователя", date.ToString()));
                    break;
                case 4:
                    writer.WriteLine(String.Format("{0}: Генерация отчета", date.ToString()));
                    break;
                case 5:
                    writer.WriteLine(String.Format("{0}: Сохранение сессии", date.ToString()));
                    break;
                case 6:
                    writer.WriteLine(String.Format("{0}: Выход из системы", date.ToString()));
                    break;
                default:
                    break;
            }
            writer.Close();
        }
    }
}
