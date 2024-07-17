using Spectre.Console.Cli;
using System.Net;


namespace LogCounter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandApp<CommandReader>();
            app.Run(args);           
        }
    }
}
