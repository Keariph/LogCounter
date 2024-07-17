using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Spectre.Console;
using Spectre.Console.Cli;

namespace LogCounter
{
    public class CommandReader : Command<CommandReader.Settings>
    {
        public class Settings : CommandSettings
        {
            [CommandOption("--file-log")]
            public string? Path {get; set;}

            [CommandOption("--file-output")]
            public string? Output { get; set;}

            [CommandOption("--address-start")]
            public IPAddress? IpAddress { get; set;}

            [CommandOption("--address-mask")]
            public int? SubnetMaskDecimal { get; set;}

            [CommandOption("--time-start")]
            public DateTime? DateStart { get; set;}

            [CommandOption("--time-end")]
            public DateTime? DateEnd { get; set;}
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            IPAddress mask;
            var fileProcessor = new Counter(settings);   
            fileProcessor.GetLogs();

            return 0;
        }    
    }
}
