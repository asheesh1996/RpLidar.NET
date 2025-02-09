using RpLidar.NET.Entities;

namespace RpLidar.NET.Example
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string PORT_NAME = "/dev/ttyUSB0";

            Console.WriteLine("Starting RpLidar Test");
            RPLidar lidar = new RPLidar(PORT_NAME);
            lidar.LidarPointScanEvent += Lidar_LidarPointScanEvent;
            Console.WriteLine("Lidar connected");
            bool isRunning = true;
            Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) =>
            {
                isRunning = false;
            };
            while (isRunning)
            {
                Thread.Sleep(10);
            }
            lidar.Dispose();
        }

        private static void Lidar_LidarPointScanEvent(List<LidarPoint> points)
        {
            foreach (LidarPoint point in points)
            {
                Console.WriteLine(point.ToString());
            }
        }
    }
}
  