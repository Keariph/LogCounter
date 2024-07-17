using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Spectre.Console;
using Spectre.Console.Cli;

namespace LogCounter
{
    public class CommandReader : Command<CommandReader.Settings>
    {
        /// <summary>
        /// 
        /// </summary>
        public class Settings : CommandSettings
        {
            /// <summary>
            /// 
            /// </summary>
            [CommandOption("--file-log")]
            public string? Path {get; set;}

            /// <summary>
            /// 
            /// </summary>
            [CommandOption("--file-output")]
            public string? Output { get; set;}

            /// <summary>
            /// 
            /// </summary>
            [CommandOption("--address-start")]
            public IPAddress? IpAddress { get; set;}

            /// <summary>
            /// 
            /// </summary>
            [CommandOption("--address-mask")]
            public int? SubnetMaskDecimal { get; set;}

            /// <summary>
            /// 
            /// </summary>
            [CommandOption("--time-start")]
            public DateTime? DateStart { get; set;}

            /// <summary>
            /// 
            /// </summary>
            [CommandOption("--time-end")]
            public DateTime? DateEnd { get; set;}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            IPAddress mask;
            var fileProcessor = new Counter(settings);   
            fileProcessor.GetLogs();

            return 0;
        }    
    }
}
