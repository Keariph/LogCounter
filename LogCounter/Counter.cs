using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LogCounter
{
    public class Counter
    {
        private readonly CommandReader.Settings _settings;
        private IPAddress _subnetMask;

        /// <summary>
        /// Initialize an entity of the log counter.
        /// </summary>
        /// <param name="settings">The got from console parameters.</param>
        public Counter(CommandReader.Settings settings) 
        {
            _settings = settings;
        }

        /// <summary>
        /// Read the log file and convert data to log entity.
        /// </summary>
        /// <returns>The list of logs entity.</returns>
        public List<Log> ReadLogs()
        {
            List<Log> logs = new List<Log>();
            if (File.Exists(_settings.Path))
            {
                List<string> lines = File.ReadLines(_settings.Path).ToList();

                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        int firstCommandIndex = line.IndexOf(':');
                        IPAddress firstPart = null;
                        IPAddress.TryParse(line.Substring(0, firstCommandIndex), out firstPart); //255.255.255.0
                        DateTime secondPart = DateTime.Parse(line.Substring(firstCommandIndex + 1)); //yyyy-MM-dd HH:mm:ss
                        logs.Add(new Log(firstPart, secondPart));
                    }
                }

                return logs;
            }
            else
            {
                Console.Error.WriteLine("Файла не существует или указан неверный путь.");
                return null;
            }
        }

        /// <summary>
        /// Read the logs file, find IP addresses by filters, calculate the number of visits to every IP address, and write the result in the input file.
        /// </summary>
        public void GetLogs()
        {
            List<Log> logs = ReadLogs();
            List<Log> result = new List<Log>();
            if (logs.Count > 0)
            {
                if (_settings.IpAddress != null)
                {
                    if (_settings.SubnetMaskDecimal != null)
                    {
                        _subnetMask = ParseMask((int)_settings.SubnetMaskDecimal);
                        IPAddress endIpAddr = CalculateBroadcastAddress(_settings.IpAddress, _subnetMask);

                        if (_settings.DateStart != null)
                        {
                            if (_settings.DateEnd != null)
                            {
                                if (_settings.DateStart > _settings.DateEnd)
                                {
                                    foreach (Log log in logs)
                                    {
                                        if (CompareIP(_settings.IpAddress, endIpAddr, log.IP) && CompareDate((DateTime)_settings.DateStart, (DateTime)_settings.DateEnd, log.Date))
                                        {
                                            result.Add(log);
                                        }
                                    }
                                }
                                Console.Error.WriteLine("Неверные значения данных. Конец диапозона не может быть меньше начала.");
                            }
                            foreach (Log log in logs)
                            {
                                if (CompareIP(_settings.IpAddress, endIpAddr, log.IP) && CompareDate((DateTime)_settings.DateStart, (DateTime)_settings.DateEnd, log.Date))
                                {
                                    result.Add(log);
                                }
                            }
                        }
                        else
                        {
                            foreach (Log log in logs)
                            {

                                if (CompareIP(_settings.IpAddress, endIpAddr, log.IP))
                                {
                                    result.Add(log);
                                }
                            }
                        }
                    }

                    if (_settings.DateStart != null)
                    {
                        if (_settings.DateEnd != null)
                        {
                            if (_settings.DateStart > _settings.DateEnd)
                            {
                                foreach (Log log in logs)
                                {
                                    if (CompareIP(_settings.IpAddress, log.IP) && CompareDate((DateTime)_settings.DateStart, (DateTime)_settings.DateEnd, log.Date))
                                    {
                                        result.Add(log);
                                    }
                                }
                            }
                            Console.Error.WriteLine("Неверные значения данных. Конец диапозона не может быть меньше начала.");
                        }
                        foreach (Log log in logs)
                        {
                            if (CompareIP(_settings.IpAddress, log.IP) && CompareDate((DateTime)_settings.DateStart, (DateTime)_settings.DateEnd, log.Date))
                            {
                                result.Add(log);
                            }
                        }
                    }
                    else
                    {
                        foreach (Log log in logs)
                        {

                            if (CompareIP(_settings.IpAddress, log.IP))
                            {
                                result.Add(log);
                            }
                        }
                    }
                }
                else
                {
                    if (_settings.DateStart != null)
                    {
                        if (_settings.DateEnd != null)
                        {
                            if (_settings.DateStart > _settings.DateEnd)
                            {
                                foreach (Log log in logs)
                                {

                                    if (CompareDate((DateTime)_settings.DateStart, (DateTime)_settings.DateEnd, log.Date))
                                    {
                                        result.Add(log);
                                    }
                                }
                            }
                            Console.Error.WriteLine("Неверные значения данных. Конец диапозона не может быть меньше начала.");
                        }
                        foreach (Log log in logs)
                        {

                            if (CompareDate((DateTime)_settings.DateStart, log.Date))
                            {
                                result.Add(log);
                            }
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine("Нет данных для начала отбора");
                    }
                }
                WriteLogs(result, _settings.Output);
            }
        }

        /// <summary>
        /// Compare date with the start point and endpoint.
        /// </summary>
        /// <param name="dateStart">The date of the start search diapason.</param>
        /// <param name="dateEnd">The date of the end search diapason.</param>
        /// <param name="date">The checked date.</param>
        /// <returns></returns>
        private bool CompareDate(DateTime dateStart, DateTime dateEnd, DateTime date)
        {
            if (dateStart <= date && date <= dateEnd)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Compare date with the start point without endpoint.
        /// </summary>
        /// <param name="dateStart">The date of the start search diapason.</param>
        /// <param name="date">The checked date.</param>
        /// <returns></returns>
        private bool CompareDate(DateTime dateStart, DateTime date)
        {
            if (dateStart <= date)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Convert the decimal subnet mask to the IP address form.
        /// </summary>
        /// <param name="subnetMaskDecimal">The decimal subnet mask.</param>
        /// <returns>The subnet mask in the IP address form.</returns>
        private static IPAddress ParseMask(int subnetMaskDecimal)
        {
            string[] str = new string[4];
            byte[] byteSubnetMask = new byte[4];

            if (subnetMaskDecimal > 32)
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (8 * i + j < subnetMaskDecimal)
                        {
                            str[i] += "1";
                        }
                        else
                        {
                            str[i] += "0";
                        }
                    }
                    byteSubnetMask[i] = Byte.Parse(str[i]);
                }
            }

            return new IPAddress(byteSubnetMask);
        }

        /// <summary>
        /// Calculate network address.
        /// </summary>
        /// <param name="ipAddress">The IP address of the start search diapason.</param>
        /// <param name="subnetMask">The subnet mask.</param>
        /// <returns></returns>
        private IPAddress CalculateNetworkAddress(IPAddress ipAddress, IPAddress subnetMask)
        {
            byte[] ipBytes = ipAddress.GetAddressBytes();
            byte[] maskBytes = subnetMask.GetAddressBytes();
            byte[] networkBytes = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }

            return new IPAddress(networkBytes);
        }

        /// <summary>
        /// Calculate broadcast address.
        /// </summary>
        /// <param name="ipAddress">The IP address of the start search diapason.</param>
        /// <param name="subnetMask">The subnet mask.</param>
        /// <returns></returns>
        private IPAddress CalculateBroadcastAddress(IPAddress ipAddress, IPAddress subnetMask)
        {
            byte[] ipBytes = CalculateNetworkAddress(ipAddress, subnetMask).GetAddressBytes();
            byte[] maskBytes = subnetMask.GetAddressBytes();
            byte[] broadcastBytes = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                broadcastBytes[i] = (byte)(ipBytes[i] | (~maskBytes[i]));
            }

            return new IPAddress(broadcastBytes);
        }

        /// <summary>
        /// Compare IP addresses with the start point and endpoint.
        /// </summary>
        /// <param name="startIpAddr">The IP address of the start search diapason.</param>
        /// <param name="endIpAddr">The IP address of the end search diapason.</param>
        /// <param name="address">The checked IP address.</param>
        /// <returns></returns>
        private bool CompareIP(IPAddress startIpAddr, IPAddress endIpAddr, IPAddress address)
        {
            long ipStart = BitConverter.ToInt32(startIpAddr.GetAddressBytes().Reverse().ToArray(), 0);
            long ipEnd = BitConverter.ToInt32(endIpAddr.GetAddressBytes().Reverse().ToArray(), 0);
            long ip = BitConverter.ToInt32(address.GetAddressBytes().Reverse().ToArray(), 0);
            return ip >= ipStart && ip <= ipEnd;
        }

        /// <summary>
        /// Compare IP addresses with the start point without the endpoint.
        /// </summary>
        /// <param name="startIpAddr">The IP address of the start search diapason.</param>
        /// <param name="address">The checked IP address.</param>
        /// <returns></returns>
        private bool CompareIP(IPAddress startIpAddr, IPAddress address)
        {
            long ipStart = BitConverter.ToInt32(startIpAddr.GetAddressBytes().Reverse().ToArray(), 0);
            long ip = BitConverter.ToInt32(address.GetAddressBytes().Reverse().ToArray(), 0);
            return ip >= ipStart;
        }

        /// <summary>
        /// Calculate and write logs to the result file.
        /// </summary>
        /// <param name="logs">The verified logs.</param>
        /// <param name="path">The path to the result file.</param>
        public void WriteLogs(List<Log> logs, string path)
        {
            Dictionary<IPAddress, int> result = new Dictionary<IPAddress, int>();

            foreach (Log log in logs)
            {
                if (result.ContainsKey(log.IP))
                {
                    result[log.IP] += 1;
                }
                else
                {
                    result.Add(log.IP, 1);
                }
            }
            using (StreamWriter sw = (File.Exists(path)) ? File.AppendText(path) : File.CreateText(path))
            {
                foreach (KeyValuePair<IPAddress, int> kvp in result)
                {
                    sw.WriteLine("{0}:{1}", kvp.Key, kvp.Value);
                }
            }
        }
    }
}
