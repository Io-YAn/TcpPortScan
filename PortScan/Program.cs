using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace TcpPortScan
{
    // Token: 0x02000002 RID: 2
    internal class TcpPortScan
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
        private static void Main(string[] args)
        {
            List<IPAddress> iprange;
            List<int> portrange;
            if (TcpPortScan.GetTargets(args, out iprange, out portrange))
            {
                TcpPortScan.Scan(iprange, portrange);
            }
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
        private static bool GetTargets(string[] args, out List<IPAddress> scanip, out List<int> scanport)
        {
            if (args.Length != 0 && args[0] == "-h")
            {
                Console.WriteLine("Ex: PortScanner.exe 127.0.0.1 (Default port: 1-65535)");
                Console.WriteLine("Ex: PortScanner.exe 127.0.0.1-3");
                Console.WriteLine("Ex: PortScanner.exe 127.0.0.1-3 1 65535");
                Console.WriteLine("Ex: PortScanner.exe 127.0.0.1-3 1 80,443");
                scanip = null;
                scanport = null;
                return false;
            }
            if (args.Length >= 1)
            {
                scanip = new List<IPAddress>();
                scanport = new List<int>();
                string text = args[0];
                if (text.Contains("-"))
                {
                    string str = string.Concat(new string[]
                    {
                        text.Split(new char[]
                        {
                            '.'
                        })[0],
                        ".",
                        text.Split(new char[]
                        {
                            '.'
                        })[1],
                        ".",
                        text.Split(new char[]
                        {
                            '.'
                        })[2],
                        "."
                    });
                    int num = int.Parse(text.Split(new char[]
                    {
                        '.'
                    })[3].Split(new char[]
                    {
                        '-'
                    })[0]);
                    int num2 = int.Parse(text.Split(new char[]
                    {
                        '-'
                    })[1]);
                    for (int i = num; i <= num2; i++)
                    {
                        IPAddress item;
                        IPAddress.TryParse(str + i.ToString(), out item);
                        scanip.Add(item);
                    }
                }
                else
                {
                    IPAddress item2;
                    IPAddress.TryParse(text, out item2);
                    scanip.Add(item2);
                }
                if (args.Length == 1)
                {
                    for (int j = 0; j <= 65535; j++)
                    {
                        scanport.Add(j);
                    }
                }
                else if (args.Length == 2)
                {
                    foreach (string s in args[1].Split(new char[]
                    {
                        ','
                    }))
                    {
                        scanport.Add(int.Parse(s));
                    }
                }
                else
                {
                    if (args.Length != 3)
                    {
                        return false;
                    }
                    for (int l = int.Parse(args[1]); l <= int.Parse(args[2]); l++)
                    {
                        scanport.Add(l);
                    }
                }
                Console.WriteLine(scanip.Count.ToString() + " targets with " + scanport.Count.ToString() + " ports to scan...");
                return true;
            }
            Console.WriteLine("Args number wrong! Use -h to show help.");
            scanip = null;
            scanport = null;
            return false;
        }

        // Token: 0x06000003 RID: 3 RVA: 0x000022B4 File Offset: 0x000004B4
        private static void Scan(List<IPAddress> iprange, List<int> portrange)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            Console.WriteLine("Scan start . . . ");
            foreach (IPAddress ipaddress in iprange)
            {
                Console.WriteLine("Scanning " + ipaddress.ToString());
                foreach (int num in portrange)
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    for (; ; )
                    {
                        try
                        {
                            socket.Bind(new IPEndPoint(IPAddress.Any, random.Next(65535)));
                        }
                        catch
                        {
                            continue;
                        }
                        break;
                    }
                    try
                    {
                        socket.BeginConnect(new IPEndPoint(ipaddress, num), new AsyncCallback(TcpPortScan.ScanCallBack), new ArrayList
                        {
                            socket,
                            num
                        });
                    }
                    catch
                    {
                    }
                }
                Console.WriteLine(ipaddress.ToString() + " finished");
            }
            Console.WriteLine("Scan end.\n");
        }

        // Token: 0x06000004 RID: 4 RVA: 0x00002410 File Offset: 0x00000610
        private static void ScanCallBack(IAsyncResult result)
        {
            ArrayList arrayList = (ArrayList)result.AsyncState;
            Socket socket = (Socket)arrayList[0];
            int num = (int)arrayList[1];
            if (result.IsCompleted && socket.Connected)
            {
                Console.WriteLine("Port Open: {0,5}", num);
            }
            socket.Close();
        }
    }
}
