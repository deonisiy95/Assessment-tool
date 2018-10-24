using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUP
{
    class User
    {
        public string login;
        public string hash;

        public User(string _login, string _hash)
        {
            login = _login;
            hash = _hash;
        }
    }
}
