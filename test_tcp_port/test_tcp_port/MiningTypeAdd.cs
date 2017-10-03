using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace test_tcp_port
{
    class MiningTypeAdd
    {
        public static log4net.ILog log = log4net.LogManager.GetLogger("Mylog");
        public static List<string> ReadType(string mining_type)
        {
            List<string> wallet_list = new List<string>();
            try
            {


                switch (mining_type)
                {
                    case "ZCASH":
                        wallet_list.Add("t1N7NByjcXxJEDPeb1KBDT9Q8Wocb3urxnv");
                        wallet_list.Add("t1W9HL5Aep6WHsSqHiP9YrjTH2ZpfKR1d3t");
                        wallet_list.Add("t1b9PsiekL4RbMoGzyLMFkMevbz7QfwepgP");
                        wallet_list.Add("t1dn3KXy6mBi5TR1ifRwYse6JMgR2w7zUbr");
                        break;
                    case "ETH":
                        wallet_list.Add("0x7fb21ac4cd75d9de3e1c5d11d87bb904c01880fc");
                        wallet_list.Add("0x34FAAa028162C4d4E92DB6abfA236A8E90fF2FC3");
                        wallet_list.Add("0xe19fFB70E148A76d26698036A9fFD22057967D1b");
                        wallet_list.Add("0x3509F7bd9557F8a9b793759b3E3bfA2Cd505ae31");
                        wallet_list.Add("0xdE088812A9c5005b0dC8447B37193c9e8b67a1fF");
                        wallet_list.Add("0xc1c427cD8E6B7Ee3b5F30c2e1D3f3c5536EC16f5");
                        wallet_list.Add("0xc6F31A79526c641de4E432CB22a88BB577A67eaC");
                        wallet_list.Add("0xB9cF2dA90Bdff1BC014720Cc84F5Ab99d7974EbA");
                        wallet_list.Add("0x0388eaa06d9a72f406de92033a405262bb7111cd");
                        break;
                    case "ETC":
                        wallet_list.Add("0x7fb21ac4cd75d9de3e1c5d11d87bb904c01880fc");
                        wallet_list.Add("0x34FAAa028162C4d4E92DB6abfA236A8E90fF2FC3");
                        wallet_list.Add("0xe19fFB70E148A76d26698036A9fFD22057967D1b");
                        wallet_list.Add("0x3509F7bd9557F8a9b793759b3E3bfA2Cd505ae31");
                        wallet_list.Add("0xdE088812A9c5005b0dC8447B37193c9e8b67a1fF");
                        wallet_list.Add("0xc1c427cD8E6B7Ee3b5F30c2e1D3f3c5536EC16f5");
                        wallet_list.Add("0xc6F31A79526c641de4E432CB22a88BB577A67eaC");
                        wallet_list.Add("0xB9cF2dA90Bdff1BC014720Cc84F5Ab99d7974EbA");
                        wallet_list.Add("0x0388eaa06d9a72f406de92033a405262bb7111cd");
                        break;
                    case "XMR":
                        wallet_list.Add("47mr7jYTroxQMwdKoPQuJoc9Vs9S9qCUAL6Ek4qyNFWJdqgBZRn4RYY2QjQfqEMJZVWPscupSgaqmUn1dpdUTC4fQsu3yjN");
                        wallet_list.Add("41yKL4ZptU9Qw2LBJ97DR5UJ8zCrT72tSaUG5PGnGdmNG1vnMUWGdZ57kaKuma5or9Jo8cHEvE8zuBt1Cx5CXNRuADjnggr");
                        wallet_list.Add("1LmMNkiEvjapn5PRY8A9wypcWJveRrRGWr");
                        //wallet_list.Add("463tWEBn5XZJSxLU6uLQnQ2iY9xuNcDbjLSjkn3XAXHCbLrTTErJrBWYgHJQyrCwkNgYvyV3z8zctJLPCZy24jvb3NiTcTJ.8187fdb7932842b187584e2ae3369ad1a7b701f5f6584427b4d2f75d3935ca2b");

                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                log.Debug(ex.ToString());
                Console.WriteLine(ex.ToString() + "获取挖矿地址错误");

            }

            return wallet_list;

        }
    }
}
