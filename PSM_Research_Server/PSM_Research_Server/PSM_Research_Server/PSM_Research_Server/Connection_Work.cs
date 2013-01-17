using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using PSMoveSharp;

namespace PSM_Research_Server
{
    public class Connection_Work
    {
        private TcpClient tc;
        NetworkStream ns;
        StreamWriter sw;
        StreamReader sr;
        private string ID;
        Game1 parent;

        public delegate void InputReceived(string s);
        public event InputReceived RaiseInputReceived;

        public Connection_Work(TcpClient _connection, string id, Game1 g)
        {
            tc = _connection;
            ns = tc.GetStream();
            sw = new StreamWriter(ns);
            sr = new StreamReader(ns);
            ID = id;
            parent = g;
            Thread t = new Thread(new ThreadStart(DoWork));
            t.Start();
        }

        private void DoWork()
        {
            sw.WriteLine(ID.ToString());
            sw.Flush();

            string s;
            while (true)
            {

                try
                {
                    s = sr.ReadLine();


                    if (String.IsNullOrEmpty(s))
                    {
                        return;
                    }
                    else
                    {
                        if (s.Equals("1"))
                        {
                            parent.client_1_hit_count++;
                        }
                        if (s.Equals("2"))
                        {
                            parent.client_2_hit_count++;
                        }
                        if (s.Equals("3"))
                        {
                            parent.client_3_hit_count++;
                        }
                        if (s.Equals("4"))
                        {
                            parent.client_4_hit_count++;
                        }
                    }

                    Console.WriteLine(ID + "," + s);
                    RaiseInputReceived(ID + "," + s);

                }

                catch
                {
                }
            }
        }

        public void Send(string s)
        {
            Console.WriteLine("Sending to client");
            sw.WriteLine(s);
            sw.Flush();
        }
    }
}