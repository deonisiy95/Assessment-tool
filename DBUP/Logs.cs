using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUP
{
    // класс для работы с логированием
    class Logs
    {
        // записать логи
        public static void write(string message)
        {
            // откроем поток для записи
            using (StreamWriter stream = new StreamWriter(Config.LOGS_PATH))
            {
                try
                {
                    // запишем дату и время
                    stream.WriteLine(DateTime.Now);

                    // запишем сообщение
                    stream.WriteLine(message);
                }
                catch (Exception e)
                {
                }
                stream.Close();
            }
        }
        
    }
}
