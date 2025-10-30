using Serilog;
using System.Diagnostics;

class Program
{
    static async Task Main(string[] args)
    {
        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../logs/system_metrics.txt");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, shared: true)
            .CreateLogger();


        Log.Information("=== System Health Logger Started ===");

        PerformanceCounter? cpuCounter = null;
        PerformanceCounter? ramCounter = null;

        if (OperatingSystem.IsWindows())
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        while (true)
        {
            try
            {
                float cpuUsage;
                float availableMemory;

                if (OperatingSystem.IsWindows())
                {
                    cpuUsage = cpuCounter!.NextValue();
                    availableMemory = ramCounter!.NextValue();
                }
                else
                {
                    cpuUsage = GetLinuxCpuUsage();
                    availableMemory = GetLinuxAvailableMemory();
                }

                var timestamp = DateTime.Now;
                Log.Information("Time: {Time} | CPU Usage: {CpuUsage}% | Available Memory: {AvailableMemory} MB",
                    timestamp, cpuUsage, availableMemory);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error capturing system metrics");
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }

    // For Linux/macOS CPU Usage
    private static float GetLinuxCpuUsage()
    {
        try
        {
            var cpuLine = File.ReadLines("/proc/stat").FirstOrDefault(l => l.StartsWith("cpu "));
            if (cpuLine == null) return 0;

            var parts = cpuLine.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Skip(1).Select(float.Parse).ToArray();

            float idle = parts[3];
            float total = parts.Sum();

            float usage = (1 - idle / total) * 100;
            return (float)Math.Round(usage, 2);
        }
        catch
        {
            return 0;
        }
    }

    // For Linux/macOS Memory Usage
    private static float GetLinuxAvailableMemory()
    {
        try
        {
            var memInfo = File.ReadAllLines("/proc/meminfo");
            float memTotal = float.Parse(memInfo.First(l => l.StartsWith("MemTotal:")).Split()[1]) / 1024;
            float memFree = float.Parse(memInfo.First(l => l.StartsWith("MemAvailable:")).Split()[1]) / 1024;
            return (float)Math.Round(memFree, 2);
        }
        catch
        {
            return 0;
        }
    }
}
