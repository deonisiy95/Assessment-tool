using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUP
{
    class Response<Type>
    {
        public string status { get; set; }
        public Type response { get; set; }
    }

    class doStart
    {
        public int is_logged {get; set; }
    }

    class Message
    {
        public string message { get; set; }
    }
}
