using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Himesyo.Check;
using Himesyo.ComponentModel.Design;
using Himesyo.Logger;

namespace IAService
{
    internal static class AppMain
    {
        public const byte SymbolStart = 0x68;
        public const byte SymbolEnd = 0x16;

        private static readonly string configName = "IAService.jsonconfig";
        public static AppConfig Config { get; set; } = default!;
        public static TcpListener Listener { get; set; } = default!;


        public static ConcurrentDictionary<string, TcpClient> Clients { get; } = new ConcurrentDictionary<string, TcpClient>();

        private static uint nextSocketNumber = 10000001;
        private static int taskNumber = 0;

        public static void Run(string[] args)
        {
            InitLogger();
            LoadConfig();
            try
            {
                Listener = new TcpListener(IPAddress.Any, Config.ListenPort);
                Listener.Start();
            }
            catch (Exception ex)
            {
                LoggerSimple.WriteError($"无法启动监听。", ex);
                Console.WriteLine($"* Error - 无法启动监听：{ex.Message}");
                return;
            }

            Task taskListen = ListenConnAsync().ContinueWith(task => taskNumber--);

            Process curr = Process.GetCurrentProcess();
            FileLengthFormatter fileLengthFormatter = new FileLengthFormatter();
            Random random = new Random();
            while (true)
            {
                Thread.Sleep(3 * 1000);

                curr.Refresh();
                int threadCount = curr.Threads.Count;
                int taskCount = taskNumber;
                string memorySize = fileLengthFormatter.Format("L", curr.PrivateMemorySize64, null);

                Console.WriteLine($"{LoggerSimple.Time} Info  - 线程数:{threadCount,4} 任务数:{taskCount,5} 专用内存:{memorySize}");

            }
        }

        public static async Task ListenConnAsync()
        {
            taskNumber++;
            while (true)
            {
                TcpClient client = await Listener.AcceptTcpClientAsync();
                uint socketNumber = nextSocketNumber++;
                LoggerSimple.WriteInfo($"【{socketNumber}】接受新连接：{client.Client.RemoteEndPoint}");
                Console.WriteLine($"【{socketNumber}】接受新连接：{client.Client.RemoteEndPoint}");

                Task taskHandle = HandleConn(socketNumber, client).ContinueWith(task => taskNumber--);
            }
        }

        public static async Task HandleConn(uint socketNumber, TcpClient client)
        {
            taskNumber++;
            LoggerSimple.WriteInfo($"【{socketNumber}】开始处理连接...");
            Console.WriteLine($"【{socketNumber}】开始处理连接...");
            try
            {
                while (true)
                {
                    NetworkStream stream = client.GetStream();
                    IAMessage message = new IAMessage();
                    byte[] buffer = new byte[512];
                    int post = 0;
                    int readLength = await stream.ReadAsync(buffer.AsMemory(0, 3));
                    post += readLength;
                    if (readLength == 3)
                    {
                        message.UserDataLength = buffer[1];
                        if (buffer[0] != SymbolStart || buffer[2] != SymbolStart)
                            LoggerSimple.WriteWarning($"接收到请求，但报文头有误：0:0x{buffer[0]:X} 1:0x{buffer[1]:X} 2:0x{buffer[2]:X}");
                    }
                    readLength = await stream.ReadAsync(buffer.AsMemory(post, message.UserDataLength));
                    post += readLength;
                    if (readLength == message.UserDataLength)
                    {
                        Console.WriteLine($"【{socketNumber}】{buffer[0..post].ToShow()}...");
                    }
                    readLength = await stream.ReadAsync(buffer.AsMemory(post, buffer.Length - post));
                    post += readLength;
                    if (!client.Connected)
                    {
                        break;
                    }
                    if (post == 0)
                    {
                        stream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"连接异常断开：{ex.Message}");
            }
        }

        public static void LoadConfig()
        {
            try
            {
                Config = AppConfig.Load<AppConfig>(configName) ?? new AppConfig();
            }
            catch (Exception ex)
            {
                LoggerSimple.WriteError($"已初始化配置。加载配置时出错。", ex);
                Console.WriteLine($"* Error - 已初始化配置。加载配置时出错：{ex.Message}");
                Config = new AppConfig();
            }
        }

        public static void SaveConfig()
        {
            try
            {
                Config.Save(configName);
            }
            catch (Exception ex)
            {
                LoggerSimple.WriteError($"保存配置失败。", ex);
                Console.WriteLine($"* Error - 保存配置失败：{ex.Message}");
            }
        }

        public static void InitLogger()
        {
            try
            {
                LoggerSimple.Init("logs", "IAService");
                LoggerSimple.WriteCallLocation = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"* Error - 无法初始化日志记录器：{ex.Message}");
            }
        }
    }

    public class IAMessage
    {
        public byte UserDataLength { get; set; }

        public byte ControlDomain { get; set; }

        public byte CRC { get; set; }
    }
}
