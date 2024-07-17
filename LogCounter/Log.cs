using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LogCounter
{
    public class Log
    {
        public Log(IPAddress iP, DateTime dateTime)
        {
            IP = iP;
            Date = dateTime;
        }
        
        public IPAddress IP { get;}
        public DateTime Date { get;}
    }
}
