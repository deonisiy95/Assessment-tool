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

    class Profile
    {
        public string full_name { get; set; }
        public int role { get; set; }
        public string organisation_name { get; set; }
        public string address { get; set; }
    }

    public class Assessments
    {
        public int assessment_id { get; set; }
        public string audit_object { get; set; }
        public string address { get; set; }
        public int auditor_id { get; set; }
        public string assessment_link { get; set; }
        public int created_at { get; set; }
        public string document_poll { get; set; }
    }
}
