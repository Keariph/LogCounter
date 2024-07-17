# LogCounter

## Project Description
A console project for counting a visit by the IP addresses using special filters. You can filter IP addresses by a date and range IP addresses. The LogCounter project is a command line tool accepting arguments such as:
Name | Description
--- | ---
--file-log | Path to the the log file
--file-output | Path to the output file
--time-start [otional] | Start of the time range in `dd.mm.yyyy` format
--time-end [optional] | End of the time range in `dd.mm.yyyy` format
--address-start [optional] | Ip address of the Start of the range of IP addresses 
--address-mask [optional] | Address mask in decimal `[0-32]` for finding the end of the range of IP addresses

And processing them with the `Spectre.Console` library.

The project reads the IP addresses from a text file, counts a visit by the IP addresses and writes the result in the output file. It takes the path for reading logs from the argument `--file-log` and the path for writing the result from the `--file-output`.

For finding the start and end of the range of IP addresses, LogCounter calculates a network address and broadcast address. The broadcast address will be the end of the range while the received `--address-start` will be the start of the range. 

### Used technologies:
- Visual Studio 2022
- .Net 8.0
- Spectre.Console library


## How to use 
1) Import the project
2) Go to the `C:\{path}\LogCounter` directory and open console
3) Write a comand line like
```
dotnet run --file-log={path} --file-outpt={path} --time-start={dd.mm.yyyy} [optional] --time-end={dd.mm.yyyy} [optional] --address-start={ip address} [optional] --address-mask={decimal mask} [optional]
```
