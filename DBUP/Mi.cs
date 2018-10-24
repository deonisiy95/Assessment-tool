using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUP
{
    public class Mi
    {
        public int indexGroup;
        public double sum;
        public double value;
        public bool overlook;
        public int amountQuestions;
        public int amountOverlookQuestions;
        public int countZero;

        public Mi[] secondaryMi;
        //public Mi BPTP;
        //public Mi BITP;
        //public Mi BTPPDn;
        public Mi(int _indexGroup)
        {
            indexGroup = _indexGroup;
            sum = 0;
            value = 0;
            secondaryMi = new Mi[] {null, null, null };
            amountQuestions = 0;
            amountOverlookQuestions = 0;
            countZero = 0;
        }

        public bool isOverlook()
        {
            return amountQuestions == amountOverlookQuestions;
        }

        public void CalculateValue()
        {
            value = sum / (amountQuestions - amountOverlookQuestions);
        }
    }
}
