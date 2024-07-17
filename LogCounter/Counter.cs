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

        public Counter(CommandReader.Settings settings) 
        {
            _settings = settings;
            _subnetMask = ParseMask((int)_settings.SubnetMaskDecimal);
        }

        public List<Log> ReadLogs()
        {
            List<Log> logs = new List<Log>();
            //try
            //{
                if (File.Exists(_settings.Path))
                {
                    List<string> lines = File.ReadLines(_settings.Path).ToList();

                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            int firstCommaIndex = line.IndexOf(':');
                            IPAddress firstPart = null;
                            IPAddress.TryParse(line.Substring(0, firstCommaIndex), out firstPart); //255.255.255.0
                            DateTime secondPart = DateTime.Parse(line.Substring(firstCommaIndex + 1)); //yyyy-MM-dd HH:mm:ss
                            logs.Add(new Log(firstPart, secondPart));
                        }
                    }

                    return logs;
                }
            /*}
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }*/
            return null;
        }

        public void GetLogs()
        {
            List<Log> logs = ReadLogs();
            List<Log> result = new List<Log>();
            IPAddress endIpAddr = CalculateBroadcastAddress( _settings.IpAddress, _subnetMask);

            if (logs.Count > 0)
            {
                foreach (Log log in logs)
                {
                    if (CompareIP(_settings.IpAddress, endIpAddr, log.IP) && CompareDate((DateTime)_settings.DateStart, (DateTime)_settings.DateEnd, log.Date))
                    {
                        result.Add(log);
                    }
                }
            }

            WriteLogs(result, _settings.Output);
        }

        private bool CompareDate(DateTime dateStart, DateTime dateEnd, DateTime date)
        {
            if (dateStart <= date && date <= dateEnd)
            {
                return true;
            }

            return false;
        }

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

        private bool CompareIP(IPAddress startIpAddr, IPAddress endIpAddr, IPAddress address)
        {
            long ipStart = BitConverter.ToInt32(startIpAddr.GetAddressBytes().Reverse().ToArray(), 0);
            long ipEnd = BitConverter.ToInt32(endIpAddr.GetAddressBytes().Reverse().ToArray(), 0);
            long ip = BitConverter.ToInt32(address.GetAddressBytes().Reverse().ToArray(), 0);
            return ip >= ipStart && ip <= ipEnd;
        }

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
