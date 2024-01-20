using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

class Traceroute
{
    //A buffer size of 32 bytes is commonly used for ICMP Echo Requests,
    //and it's usually sufficient for the purposes of basic ping operations.
    private const int BufferSize = 32;

    //This byte array holds the data sent in the ICMP Echo Request packet. This data is typically used for diagnostic purposes  
    //this payload data is like a helpful message that carries specific details about the journey of a packet
    //through the network.
    //These details assist in finding and fixing issues, such as figuring out how fast data travels
    //or identifying places where the network might be slowing down
    private static readonly byte[] Buffer = new byte[BufferSize];

    private const int Timeout = 1000;
    private const int MaxHops = 30;

    static async Task Main()
    {
        Console.WriteLine("Please input destination domain name you want to trace, examples cnn.com, goal.com, x.com");
        var destination = GetDestination();

        var tasks = new Task<PingReply>[MaxHops];

        //Time to Live" (TTL) refers to a field in the header of a packet that specifies the maximum amount of time the packet
        //is allowed to live
        //OR
        //ttl here means (Time to Live) the maximum number of hops (routers or network devices) the
        //packet can traverse before the program drops it, incase this packet does not reach its destination before the
        //default MaxHops(30), then the packet is dropped.
        for (int ttl = 1; ttl <= MaxHops; ttl++)
        {
            Ping pingSender = new();

            //Setting the "Don't Fragment" flag to true means that the packet should not be
            //fragmented as it travels through the network.
            //If the packet size exceeds the Maximum Transmission Unit (MTU) of a network link, and the "Don't Fragment"
            //flag is set to true,
            //the router will drop the packet and send an ICMP "Fragmentation Required" message back to the sender.
            //If you are wondring what  "Don't Fragment" (DF) flag means,
            //it is a setting in the IP header of a packet that instructs
            //routers not to fragment the packet into smaller chunks as it traverses the network.
            PingOptions pingOptions = new(ttl, true);

            tasks[ttl - 1] = pingSender.SendPingAsync(destination, Timeout, Buffer, pingOptions);//ICMP Echo Reques;
        }

        PingReply[] replies = null!;
        Task<PingReply[]> allTasks = Task.WhenAll(tasks);
        try
        {
            replies = await allTasks;
        }
        //I intentionally did not observe all the exceptions here. I felt it is
        //sufficient to respond to only the first error that was thrown, rather than all of them
        catch(PingException e)
        {
            Console.WriteLine($"An error occurred: {e.InnerException!.Message}");
            Environment.Exit(1);
        }
        
        for (int i = 0; i < replies.Length; i++)
        {
            PingReply reply = replies[i];
            if (reply.Status == IPStatus.TimedOut)
            {
                Console.WriteLine($"Hop {i + 1}: Status: {reply.Status} *");
            }
            else
            {
                Console.WriteLine($"Hop {i + 1}: Address: {reply.Address} Status: {reply.Status} RTT: {reply.RoundtripTime}");
            }

            if (reply.Status == IPStatus.Success || reply.Status == IPStatus.DestinationHostUnreachable)
            {
                Console.WriteLine("Destination reached!");
                break;
            }
        }
    }

    static string GetDestination()
    {
        var destination = Console.ReadLine();
        if (destination is null || IsValidDomain(destination) is false)
        {
            Console.WriteLine("Please input a valid destination URL");
            destination = GetDestination();
            Console.Clear();
        }

        return destination;
    }

    static bool IsValidDomain(string domain)
    {
        // Simple regex for checking if the string is a valid domain
        string pattern = @"^(?!:\/\/)([a-zA-Z0-9-]+\.){1,}[a-zA-Z]{2,}$";
        return Regex.IsMatch(domain, pattern);
    }
}
