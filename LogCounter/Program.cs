using Spectre.Console.Cli;
using System.Net;


namespace LogCounter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // TODO: 1ый шаг - проверить что обязательные параметры существуют
            // TODO: 2ой шаг - проверить что нет лишних/неизвестных параметров
            // TODO: 3ий шаг - смотрим внутрь каждого параметра, проверяем что там всё правильно
            // TODO: 4ый шаг - проверяем зависимые параметры, например что time-start меньше time-end
            // И только если всёс збс - продолжаем программу
            /*if (!AreKnown(args))
            {
                WriteHelp();
            }*/

            var app = new CommandApp<CommandReader>();
            app.Run(args);
            //Counter counter = new Counter();
            //counter.GetLogs(path, output, dateStart, dateEnd, ipAddress, subnetMask);
        }

        /*private static bool AreKnown(string[] args)
        {
            return args.All(arg => knownParams.Contains(arg.Split('=').First()));

            foreach (string arg in args)
            {
                string[] part = arg.Split('=');

                switch (part[0])
                {
                    case FileLog:
                        {
                            path = part[1];
                            break;
                        }
                    case FileOutput:
                        {
                            output = part[1];
                            break;
                        }
                    case TimeStart:
                        {
                            dateStart = DateTime.Parse(part[1]);
                            break;
                        }
                    case TimeEnd:
                        {
                            dateEnd = DateTime.Parse(part[1]);
                            break;
                        }
                    case AddressStart:
                        {
                            ipAddress = IPAddress.Parse(part[1]);
                            break;
                        }
                    case AddressMask:
                        {
                            subnetMaskDecimal = Int32.Parse(part[1]);
                            IPAddress subnetMask = ParseMask(subnetMaskDecimal);
                            break;
                        }
                    default:
                        {
                            Console.WriteLine();
                            break;
                        }
                }
            }
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
        }*/

        static void WriteHelp()
        {
            Console.WriteLine();
        }
    }
}
