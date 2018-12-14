using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUP
{
    public class SecondaryOuestion
    {

        public int documentation;
        public int execution;
        public bool answered;
        public bool overlook;
        public SecondaryOuestion()
        {
            documentation = -1;
            execution = -1;
            answered = false;
            overlook = false;
        }
        public SecondaryOuestion(int _documentation, int _execution, bool _answered, bool _overlook)
        {
            documentation = _documentation;
            execution = _execution;
            answered = _answered;
            overlook = _overlook;
        }
    }
    public class Question
    {
        public int group;
        public int number;
        public string question;
        public bool mandatory;
        public int category;
        public bool overlook;
        public int value;
        public int documentation;
        public int execution;
        public bool answered;
        public SecondaryOuestion[] secondaryOuestions;
        public Question(int _group, int _number, string _question, bool _mandatory, int _category)
        {
            group = _group;
            number = _number;
            question = _question;
            mandatory = _mandatory;
            category = _category;
            overlook = false;
            documentation = -1;
            execution = -1;
            value = 0;
            answered = false;
            secondaryOuestions = new SecondaryOuestion[] { new SecondaryOuestion(), new SecondaryOuestion(), new SecondaryOuestion() };
        }
        public Question(int _group, int _number, string _question, bool _mandatory, int _category, bool _overlook, int _documentation, int _execution, bool _answered)
        {
            group = _group;
            number = _number;
            question = _question;
            mandatory = _mandatory;
            category = _category;
            overlook = _overlook;
            documentation = _documentation;
            execution = _execution;
            value = 0;
            answered = _answered;
            secondaryOuestions = new SecondaryOuestion[] { new SecondaryOuestion(), new SecondaryOuestion(), new SecondaryOuestion() };
        }
    }

    public class DocQuestion {

        public int id { get; set; }
        public string symbol { get; set; }
        public string value { get; set; }

        public DocQuestion(int _id, string _symbol, string _value)
        {
            id = _id;
            symbol = _symbol;
            value = _value;
        }
    }
}
