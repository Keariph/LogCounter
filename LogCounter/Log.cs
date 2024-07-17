using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LogCounter
{
    /// <summary>
    /// 
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="dateTime"></param>
        public Log(IPAddress ip, DateTime dateTime)
        {
            IP = ip;
            Date = dateTime;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public IPAddress IP { get;}

        /// <summary>
        /// 
        /// </summary>
        public DateTime Date { get;}
    }
}
