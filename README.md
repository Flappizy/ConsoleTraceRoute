# ConsoleTraceRoute
ConsoleTraceRoute is a simple console application for tracing the route to a destination host using ICMP Echo Requests. It sends ICMP Echo Requests with increasing Time to Live (TTL) values to observe the path a packet takes through the network.

## Features
- Traces the route to a destination host.
- Uses ICMP Echo Requests with increasing TTL values.
- Displays information about each hop, including address and status.
- Terminates when the destination is reached or the maximum number of hops is reached.

## Usage
- Clone the repository to your local machine.
- Navigate to the project directory
- Build the solution using dotnet build.
- Run the application with dotnet run.
- Follow the on-screen instructions to input the destination domain name you want to trace.

## Dependencies
.NET 8

## Notes
This application is a simple implementation of traceroute for educational purposes.
It helps in understanding the route a packet takes through the network.
