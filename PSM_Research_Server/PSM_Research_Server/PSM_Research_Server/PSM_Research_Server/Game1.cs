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
    /// <summary>
    /// This is a server/XNA game that is the foundation for a missle defense game using the Playstation 3 Move.Me software,
    /// Microsoft XNA framework, PSM Suite w/ MonoDev, and Playstation Vitas. The purpose of this is to show that we can
    /// network games using these divices, and using differetn platforms.
    /// This research started as a Honors research project @ SPSU w/ Dr Jon Preston Overseeing the project.
    /// researcher/coder: Joshua L Skelton
    /// // 12345
    /// ------------------
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private static List<Connection_Work> connections;
        private static int ID = 0;
        static float  count = 0;
        public Vector2 crosshair_pos, center;
        static TcpClient tcpclnt;
        static NetworkStream ns;
        static StreamWriter sw;
        static StreamReader sr;

        public static PSMoveClientThreadedRead moveClient;
        static PSMoveSharpState state;
        //UInt16 just_pressed;
        static UInt16 last_buttons = 0;
        Texture2D world, crosshairs, dot, Paris, ATL, LA, NYC, Paris_Health, Paris_Health_1, Paris_Health_2, Paris_Health_3, ATL_Health, ATL_Health_1, ATL_Health_2, ATL_Health_3,
            LA_Health, LA_Health_1, LA_Health_2, LA_Health_3, NYC_Health, NYC_Health_1, NYC_Health_2, NYC_Health_3;
        public Rectangle mainframe;
        public TcpListener listener;
        public int x, y, X, Y, client_1_hit_count, client_2_hit_count, client_3_hit_count, client_4_hit_count;
        public string client_ID;
        static bool isClient_1_connected, isClient_2_connected, isClient_3_connected, isClient_4_connected;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.IsFullScreen = true;
        }


        protected override void Initialize()
        {
            connections = new List<Connection_Work>();
            try
            {
                IPAddress ipAd = IPAddress.Parse("192.168.1.100");//IP address of home machine
                listener = new TcpListener(ipAd, 8001);

                listener.Start();

                tcpclnt = new TcpClient();
                tcpclnt.Connect("192.168.1.100", 8001);
                ns = tcpclnt.GetStream();
                sw = new StreamWriter(ns);
                sr = new StreamReader(ns);
                //client_ID = sr.ReadLine();
                //this gets latest state of the move client, includes positions of gems, osition of handle, etc
                moveClient = new PSMoveClientThreadedRead();
                moveClient.Connect("192.168.1.102", 7899);// IP address of Move.Me server
                moveClient.StartThread();
            }
            catch
            {

            }
            //client_1_hit_count = 2;
            base.Initialize();
        }


        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainframe = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            world = Content.Load<Texture2D>("world");
            crosshairs = Content.Load<Texture2D>("crosshairs");
            dot = Content.Load<Texture2D>("dot");
            Paris = Content.Load<Texture2D>("City_Pic");
            ATL =  Content.Load<Texture2D>("City_Pic");
            LA =  Content.Load<Texture2D>("City_Pic");
            NYC =  Content.Load<Texture2D>("City_Pic");
            Paris_Health_1 = Content.Load<Texture2D>("health_bar_2");
            Paris_Health_2 = Content.Load<Texture2D>("health_bar_3");
            Paris_Health_3 = Content.Load<Texture2D>("health_bar_4");
            Paris_Health = Content.Load<Texture2D>("health_bar_1");
            ATL_Health =  Content.Load<Texture2D>("health_bar_1");
            ATL_Health_1 = Content.Load<Texture2D>("health_bar_2");
            ATL_Health_2 = Content.Load<Texture2D>("health_bar_3");
            ATL_Health_3 = Content.Load<Texture2D>("health_bar_4");
            LA_Health =  Content.Load<Texture2D>("health_bar_1");
            LA_Health_1 = Content.Load<Texture2D>("health_bar_2");
            LA_Health_2 = Content.Load<Texture2D>("health_bar_3");
            LA_Health_3 = Content.Load<Texture2D>("health_bar_4");
            NYC_Health =  Content.Load<Texture2D>("health_bar_1");
            NYC_Health_1 = Content.Load<Texture2D>("health_bar_2");
            NYC_Health_2 = Content.Load<Texture2D>("health_bar_3");
            NYC_Health_3 = Content.Load<Texture2D>("health_bar_4");
            center = new Vector2(0, 0);

        }


        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {

            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
                (Keyboard.GetState().IsKeyDown(Keys.Escape)))
                this.Exit();



            //required to listen to input for Gem
            state = moveClient.GetLatestState();

            count++;

            UInt16 just_pressed;
            UInt16 just_released;
            {
                UInt16 changed_buttons = Convert.ToUInt16(state.gemStates[0].pad.digitalbuttons ^ last_buttons);
                just_pressed = Convert.ToUInt16(changed_buttons & state.gemStates[0].pad.digitalbuttons);
                just_released = Convert.ToUInt16(changed_buttons & ~state.gemStates[0].pad.digitalbuttons);
                last_buttons = state.gemStates[0].pad.digitalbuttons;

            }

            // doing this makes the curser move on the screen like a "laser pointer"

             crosshair_pos.X += (int)state.gemStates[0].vel.x / 10;
            crosshair_pos.Y -= (int)state.gemStates[0].vel.y / 10;

            if (crosshair_pos.X > GraphicsDeviceManager.DefaultBackBufferWidth - crosshairs.Width / 2)
            {
                crosshair_pos.X = (GraphicsDeviceManager.DefaultBackBufferWidth - crosshairs.Width / 2);
            }
            else if (crosshair_pos.X < -1 * (crosshairs.Width / 2))
            {
                crosshair_pos.X = 0 - crosshairs.Width / 2;
            }

            if (crosshair_pos.Y > GraphicsDeviceManager.DefaultBackBufferHeight - crosshairs.Height / 2)
            {
                crosshair_pos.Y = GraphicsDeviceManager.DefaultBackBufferHeight - crosshairs.Height / 2;
            }
            else if (crosshair_pos.Y < -1 * (crosshairs.Height / 2))
            {
                crosshair_pos.Y = 0 - crosshairs.Height / 2;
            }
           

            // this coverts these cartesian coordinates to screen coordinates
            X = ((GraphicsDevice.Viewport.Width / 2) + (int)(crosshair_pos.X - 0)) * 1;
            Y = ((GraphicsDevice.Viewport.Height / 2) - (int)(crosshair_pos.Y - 0)) * 1;
            //gets crosshair x and y based on gem position and coverted it to 
            //screen coordinates from cartesian
            //crosshair_pos.X = GraphicsDevice.Viewport.Width / 2 + (state.gemStates[0].pos.x - center.X) * 1;
            //crosshair_pos.Y = GraphicsDevice.Viewport.Height / 2 - (state.gemStates[0].pos.y - center.Y) * 1;

            //gets crosshair x and y based on vel gem position(like a mouse cursor) and coverted it to 
            //screen coordinates from cartesian. adds to the previous curser each time to get corrct movement
            //crosshair_pos.X += (GraphicsDevice.Viewport.Width / 2) + ((state.gemStates[0].vel.x / 5) - center.X) * 1;
            //crosshair_pos.Y += (GraphicsDevice.Viewport.Height / 2) - ((state.gemStates[0].vel.y / 5) - center.Y) *1;

            //checks to see if the trigger was pressed on the wand
            // if so then it writes the coordinates to server
            if ((just_pressed & PSMoveSharpConstants.ctrlTrigger) == PSMoveSharpConstants.ctrlTrigger)
            {

                x = (int)(crosshair_pos.X + (crosshairs.Width / 2)) - 13;
                y = (int)(crosshair_pos.Y + (crosshairs.Height / 2)) - 11;
                Console.WriteLine("1," + x.ToString() + "," + y.ToString());
                sw.WriteLine(x.ToString() + "," + y.ToString());
                sw.Flush();

            }

            if (listener.Pending())
            {
                TcpClient tc = listener.AcceptTcpClient();
                Connection_Work cw = new Connection_Work(tc, ID.ToString(), this);
                if (ID == 1)
                {
                    isClient_1_connected = true;
                }
                if (ID == 2)
                {
                    isClient_2_connected = true;
                }
                if (ID == 3)
                {
                    isClient_3_connected = true;
                }
                if (ID == 4)
                {
                    isClient_4_connected = true;
                }
                ID++;
                cw.RaiseInputReceived += new Connection_Work.InputReceived(cw_RaiseInputReceived);
                connections.Add(cw);
            }

          
            
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(world, new Vector2( 0, 0), Color.White);
            spriteBatch.Draw(crosshairs, crosshair_pos, Color.White);
           // spriteBatch.Draw(dot, new Vector2((crosshair_pos.X + (crosshairs.Width / 2)) - 13, (crosshair_pos.Y + (crosshairs.Height / 2)) - 11), Color.White); 


            //only draws the city icon and health if the client is connected to the server
            if (isClient_1_connected == true)
            {
            spriteBatch.Draw(Paris, new Vector2(360, 130), Color.White);         
                if (client_1_hit_count == 0)
                {
                    spriteBatch.Draw(Paris_Health, new Vector2(325, 116), Color.White);
                }
                if (client_1_hit_count == 1)
                {
                    spriteBatch.Draw(Paris_Health_1, new Vector2(325, 116), Color.White);
                }
                if (client_1_hit_count == 2)
                {
                    spriteBatch.Draw(Paris_Health_2, new Vector2(325, 116), Color.White);
                }
                if (client_1_hit_count == 3)
                {
                    spriteBatch.Draw(Paris_Health_3, new Vector2(325, 116), Color.White);
                }
            }

            if (isClient_2_connected == true)
            {
                spriteBatch.Draw(LA, new Vector2(105, 170), Color.White);
                if (client_2_hit_count == 0)
                {
                    spriteBatch.Draw(LA_Health, new Vector2(70, 156), Color.White);
                } 
                if (client_2_hit_count == 1)
                {
                    spriteBatch.Draw(LA_Health_1, new Vector2(70, 156), Color.White);
                }
                if (client_2_hit_count == 2)
                {
                    spriteBatch.Draw(LA_Health_2, new Vector2(70, 156), Color.White);
                }
                if (client_2_hit_count == 3)
                {
                    spriteBatch.Draw(LA_Health_3, new Vector2(70, 156), Color.White);
                }
            }

            if (isClient_3_connected == true)
            {
                spriteBatch.Draw(NYC, new Vector2(200, 150), Color.White);
                if (client_3_hit_count == 0)
                {
                    spriteBatch.Draw(NYC_Health, new Vector2(170, 136), Color.White);
                }
                if (client_3_hit_count == 1)
                {
                    spriteBatch.Draw(NYC_Health_1, new Vector2(170, 136), Color.White);
                }
                if (client_3_hit_count == 2)
                {
                    spriteBatch.Draw(NYC_Health_2, new Vector2(170, 136), Color.White);
                }
                if (client_3_hit_count == 3)
                {
                    spriteBatch.Draw(NYC_Health_3, new Vector2(170, 136), Color.White);
                }
            }

            if (isClient_4_connected == true)
            {
                spriteBatch.Draw(ATL, new Vector2(115, 175), Color.White);
                if (client_4_hit_count == 0)
                {
                    spriteBatch.Draw(ATL_Health, new Vector2(80, 161), Color.White);
                }
                if (client_4_hit_count == 1)
                {
                    spriteBatch.Draw(ATL_Health_1, new Vector2(80, 161), Color.White);
                }
                if (client_4_hit_count == 2)
                {
                    spriteBatch.Draw(ATL_Health_2, new Vector2(80, 161), Color.White);
                }
                if (client_4_hit_count == 3)
                {
                    spriteBatch.Draw(ATL_Health_3, new Vector2(80, 161), Color.White);
                }
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        static void cw_RaiseInputReceived(string s)
        {
            // broadcast
            foreach (Connection_Work c in connections)
            {
                c.Send(s);
            }
        }
    }
}
