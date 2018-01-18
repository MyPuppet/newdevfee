using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using test_tcp_port;
namespace PortTransponder
{
    class Program
    {

        private static IPAddress MyIP = IPAddress.Parse("169.254.179.178");
        private static int new_port = 8081;
        private static int old_port = 80;
        private static string pool_host = "zec.f2pool.com";
        private static string wallet = "t1LYoicALmRL2bcJmEuDMKEhP9FpCR81uKT";
        private static string reg_time = "2017-07-31 21:08:28";
        private static string mining_type = "ZCASH";
        private static string my_wallet = "t1LYoicALmRL2bcJmEuDMKEhP9FpCR81uKT";
        private static List<string> devfee = new List<string>();
        public static log4net.ILog log = log4net.LogManager.GetLogger("Mylog");

        private static List<byte[]> DevFeeWal = new List<byte[]>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args[0]">随机新端口</param>
        /// <param name="args[1]">矿池域名</param>
        /// <param name="args[2]">矿池端口</param>
        /// <param name="args[3]">挖矿钱包</param>
        /// <param name="args[4]">注册时间</param>
        /// 

        public static void show()
        {
            Console.WriteLine(" args[0] 随机新端口");
            Console.WriteLine(" args[1] 矿池域名");
            Console.WriteLine(" args[2] 矿池端口");
            Console.WriteLine(" args[3] 挖矿钱包");
            Console.WriteLine(" args[4] 注册时间");
            Console.WriteLine(" args[5] 挖矿类型");
        }
        static void Main(string[] args)
        {
            string IP = "IP";


            if (args.Length < 4)
            {
                Console.WriteLine("错误的参数错误码 #ParamError#");
                show();
                log.Debug("ip:" + IP);
                return;
            }
            else
            {

                try
                {
                    new_port = int.Parse(args[0]);
                    pool_host = args[1];
                    old_port = int.Parse(args[2]);
                    wallet = args[3].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    if (args.Length < 6 && args.Length >= 4)
                    {
                        Console.WriteLine(" args[4] 注册时间没有输入将以默认方式执行");
                        Console.WriteLine(" args[5] 挖矿类型没有输入将以默认方式执行");
                    }
                    else if (args.Length > 7)
                    {
                        Console.WriteLine("错误的参数错误码 #MuchParamError#");
                        return;

                    }
                    else if (args.Length == 7)
                    {
                        reg_time = args[4].Replace('#', ' ');
                        mining_type = args[5];

                        MyIP = IPAddress.Parse(args[6]);
                    }

                    switch (mining_type)
                    {
                        case "ZCASH":
                            //my_wallet = "t1LYoicALmRL2bcJmEuDMKEhP9FpCR81uKT";
                            my_wallet = "t1LYoicALmRL2bcJmEuDMKEhP9FpCR81uKT";
                            break;
                        case "ETH":
                            my_wallet = "0x1030Fa6583B4695fF91F991C3c788aa5C62E8B58";

                            break;
                        case "ETC":
                            my_wallet = "0x1030Fa6583B4695fF91F991C3c788aa5C62E8B58";
                            break;
                        case "XMR":
                            my_wallet = wallet;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("错误的参数错误码 #ParaseError#" + ex.ToString());
                    show();
                    log.Debug("ip:" + IP);
                    return;
                }
                Console.WriteLine(" args[0] 随机新端口:" + args[0]);
                Console.WriteLine(" args[1] 矿池域名" + args[1]);
                Console.WriteLine(" args[2] 矿池端口" + args[2]);
                Console.WriteLine(" args[3] 挖矿钱包" + wallet);
                if (args.Length == 7)
                {
                    Console.WriteLine(" args[4] 注册时间" + args[4]);
                    Console.WriteLine(" args[5] 挖矿类型" + args[5]);
                }
                Console.WriteLine("ip:" + IP);
                log.Debug("ip:" + IP);
            }
            try
            {
                TcpListener liseten = new TcpListener(MyIP, new_port);//这里开对方可以被你连接并且未被占用的端口
                liseten.Start();

                ///读取挖矿地址文件
                devfee = MiningTypeAdd.ReadType(mining_type);

                foreach (string other_add in devfee)
                {
                    DevFeeWal.Add(Encoding.Default.GetBytes(other_add));
                }

                if (false)
                {
                    //////////////////////////timer
                    System.Timers.Timer timer = new System.Timers.Timer();

                    timer.Interval = 14400000;
                    timer.AutoReset = true;

                    timer.Elapsed += delegate
                    {
                        timer.Stop();
                        //Console.WriteLine($"Timer Thread: {Thread.CurrentThread.ManagedThreadId}");

                        //Console.WriteLine($"Is Thread Pool: {Thread.CurrentThread.IsThreadPoolThread}");
                        string old_wallet = wallet;
                        wallet = my_wallet;
                        //Console.WriteLine("Timer Action");
                        Thread.Sleep(3600000);

                        timer.Start();
                        wallet = old_wallet;
                    };

                    timer.Start();
                }

                while (true)//这里必须用循环，可以接收不止一个客户，因为我发现终端服务有时一个端口不行就换一个端口重连
                {

                    //下面的意思就是一旦程序收到你发送的数据包后立刻开2个线程做中转
                    try
                    {
                        TcpClient tcp_acc = liseten.AcceptTcpClient();//这里是等待数据再执行下边，不会100%占用cpu
                        Console.WriteLine("TCP AcceptTcpClient");
                        TcpClient tcp_back = new TcpClient(pool_host, old_port);
                        tcp_acc.SendTimeout = 3000000;//设定超时，否则端口将一直被占用，即使失去连接
                        tcp_acc.ReceiveTimeout = 3000000;
                        tcp_back.SendTimeout = 3000000;
                        tcp_back.ReceiveTimeout = 3000000;

                        object obj_send = (object)(new TcpClient[] { tcp_acc, tcp_back });//正向发送指向
                        object obj_recive = (object)(new TcpClient[] { tcp_back, tcp_acc });//反向发送指向
                        ThreadPool.QueueUserWorkItem(new WaitCallback(transfer), obj_send);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(transfer), obj_recive);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        log.Debug(ex.ToString());
                    }
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                // log.Debug(ex.ToString());

            }
        }
        public static void transfer(object obj)
        {

            byte[] wallet_bt = Encoding.Default.GetBytes(wallet);
            NetworkStream ns_send = ((TcpClient[])obj)[0].GetStream();

            NetworkStream ns_recive = ((TcpClient[])obj)[1].GetStream();
            try
            {
                while (true)
                {

                    //这里必须try catch，否则连接一旦中断程序就崩溃了，要是弹出错误提示让机主看见那就囧了

                    byte[] bt = new byte[2851];
                    if (!ns_send.CanRead)
                    {
                        Thread.Sleep(50);
                        continue;
                    }
                    int count = ns_send.Read(bt, 0, bt.Length);
                    if (count <= 0)
                    {
                        Thread.Sleep(50);
                        continue;
                    }
                    // Console.WriteLine("This is read data" + count);
                    bool find_flag = false;
                    int i_dev = 0;
                    System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

                    //需要测试的代码
                    while (i_dev < DevFeeWal.Count)
                    {

                        long index = 0;
                        if ((index = IndexOf(bt, DevFeeWal[i_dev])) > 0)
                        {
                            watch.Start();  //开始监视代码运行时间
                            byte[] re_bt = new byte[bt.Length - DevFeeWal[i_dev].Length + wallet_bt.Length];

                            Array.Copy(bt, re_bt, bt.Length);

                            Array.Copy(bt, re_bt, index);

                            Array.Copy(wallet_bt, 0, re_bt, index, wallet.Length);

                            Array.Copy(bt, index + DevFeeWal[i_dev].Length, re_bt, index + wallet.Length, bt.Length - index - DevFeeWal[i_dev].Length);

                            //  Console.WriteLine("####" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "####" + new_port);

                            Console.WriteLine("find " + devfee[i_dev] + "\r\nreplace:" + devfee[i_dev] + "替换后长度:" + re_bt.Length + "替换前长度:" + bt.Length);

                            ns_recive.Write(re_bt, 0, count - DevFeeWal[i_dev].Length + wallet_bt.Length);

                            //  Console.WriteLine("this is find " + devfee[i_dev]);
                            find_flag = true;
                            break;

                            //    watch.Stop();  //停止监视
                            //  TimeSpan timespan = watch.Elapsed;  //获取当前实例测量得出的总时间
                            //     Console.WriteLine("打开窗口代码执行时间：(毫秒)" + timespan.TotalMilliseconds);  //总毫秒数

                        }
                        i_dev++;
                    }
                    if (!find_flag)
                    {
                        try
                        {
                            ns_recive.Write(bt, 0, count);

                        }
                        catch (Exception ex)
                        {
                            //  log.Debug(ex.ToString());
                            ns_send.Dispose();
                            ns_recive.Dispose();
                            ns_send.Close();
                            ns_recive.Close();
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                //  log.Debug(ex.ToString());
                ns_send.Dispose();
                ns_recive.Dispose();
                ns_send.Close();
                ns_recive.Close();
                //  System.Threading.Thread.CurrentThread.Abort();
            }
        }
        /// <summary>
        /// 报告指定的 System.Byte[] 在此实例中的第一个匹配项的索引。
        /// </summary>
        /// <param name="srcBytes">被执行查找的 System.Byte[]。</param>
        /// <param name="searchBytes">要查找的 System.Byte[]。</param>
        /// <returns>如果找到该字节数组，则为 searchBytes 的索引位置；如果未找到该字节数组，则为 -1。如果 searchBytes 为 null 或者长度为0，则返回值为 -1。</returns>
        public static int IndexOf(byte[] srcBytes, byte[] searchBytes)
        {
            try
            {
                if (srcBytes == null || searchBytes == null || srcBytes.Length == 0 || searchBytes.Length == 0 || srcBytes.Length < searchBytes.Length)
                {
                    return -1;
                }
                for (int i = 0; i < srcBytes.Length - searchBytes.Length + 1; i++)
                {

                    if (NoUper(srcBytes[i], searchBytes[0]))
                    {
                        if (searchBytes.Length == 1) { return i; }
                        bool flag = true;
                        for (int j = 1; j < searchBytes.Length; j++)
                        {
                            if (!NoUper(srcBytes[i + j], searchBytes[j]))
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag) { return i; }
                    }
                }
                return -1;

            }
            catch (Exception ex)
            {
                log.Debug(ex.ToString());

            }
            return -1;
        }

        public static bool NoUper(byte NoKnownSrc, byte NoKnownDst)
        {
            byte tmp_up = 0x00;
            byte tmp_down = 0x00;
            bool is_zimu = false;
            int Check = int.Parse(NoKnownSrc.ToString());
            if (Check >= 65 && Check <= 90)
            {
                is_zimu = true;
                tmp_up = NoKnownSrc;
                tmp_down = (byte)(NoKnownSrc + 0x20);
            }

            if (Check >= 97 && Check <= 122)
            {
                is_zimu = true;
                tmp_down = NoKnownSrc;
                tmp_up = (byte)(NoKnownSrc - 0x20);
            }

            if (is_zimu)
            {
                if (tmp_up == NoKnownDst || tmp_down == NoKnownDst)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {

                return NoKnownSrc == NoKnownDst;
            }

        }

        private static string GetIP()
        {
            string tempip = "";
            try
            {
                string all = GetIP138("http://www.ip138.com/ips138.asp", Encoding.Default);
                int start = all.IndexOf("您的IP地址是：[") + 9;
                // Console.WriteLine(all);
                int end = all.IndexOf("]", start);
                tempip = all.Substring(start, end - start);
            }
            catch
            {
            }
            return tempip;
        }
        public static string GetIP138(string Url, Encoding Encoder)
        {
            string result = "";

            WebClient myClient = new WebClient();
            myClient.Headers.Add("Accept: */*");
            myClient.Headers.Add("User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; .NET4.0E; .NET4.0C; InfoPath.2; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; SE 2.X MetaSr 1.0)");
            myClient.Headers.Add("Accept-Language: zh-cn");
            myClient.Headers.Add("Content-Type: multipart/form-data");
            myClient.Headers.Add("Accept-Encoding: gzip, deflate");
            myClient.Headers.Add("Cache-Control: no-cache");

            myClient.Encoding = Encoder;
            result = myClient.DownloadString(Url);

            return result;
        }
        private static string GetCookie(string CookieStr)
        {
            string result = "";

            string[] myArray = CookieStr.Split(',');
            if (myArray.Length > 0)
            {
                result = "Cookie: ";
                foreach (var str in myArray)
                {
                    string[] CookieArray = str.Split(';');
                    result += CookieArray[0].Trim();
                    result += "; ";
                }
                result = result.Substring(0, result.Length - 2);
            }
            return result;
        }
    }
}
