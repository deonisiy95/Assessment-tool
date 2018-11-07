using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUP
{
    class Define
    {

        // статусы ответа сервера
        public const string REQUEST_STATUS_OK = "OK";
        public const string REQUEST_STATUS_ERROR = "ERROR";

        // типы ролей
        public static string[] TYPE_ROLE = new string[] {"Администратор", "Аудитор", "Сотрудник", "Документавод"};
        public const int TYPE_ROLE_ADMIN = 0;
        public const int TYPE_ROLE_AUDITOR = 1;
        public const int TYPE_ROLE_USER = 2;
        public const int TYPE_ROLE_DOC = 3;
    }
}
