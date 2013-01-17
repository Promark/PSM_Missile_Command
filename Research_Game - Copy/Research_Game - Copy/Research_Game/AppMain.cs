//This was written intial by Joshua L Skelton as part of research with Dr jon Preston
//It is a client for Sony devices as part of a Missile Command game to be developed
//within the Playstaion Mobile Suite and initially used on Vitas and Sony Tablets
//Date: 1/16/2013
//jskelto2@spsu.edu



using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using System.Threading;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;

namespace Research_Game
{
	public class AppMain
	{
		static string text, ID, textTemp = "temp", tempConsole1; //console_0 = "Client_0", 
		static string[] positionArray;
		static SimpleSprite[] spriteArray = new SimpleSprite[2];
		static protected GraphicsContext graphics;
		static Texture2D texture, texture2, texture3, Paris_1, Paris_2, Paris_3, Paris_4, Atlanta, LosAngeles, NewYork, particle_Tex, skyline_1_fire, skyline_2_fire, skyline_3_fire, skyline_4_fire;
		static int count = 0, count_2 = 0, indexSize = 4, generator_counter, lower = 0, upper = 960; //count_3 = 0, 
		static float[] vertices = new float[12];
		static float[] texcoords = {
			0.0f, 0.0f,	// 0 top left.
			0.0f, 1.0f,	// 1 bottom left.
			1.0f, 0.0f,	// 2 top right.
			1.0f, 1.0f,	// 3 bottom right.
		};
		static float[] colors = {
			1.0f,	1.0f,	1.0f,	1.0f,	// 0 top left.
			1.0f,	1.0f,	1.0f,	1.0f,	// 1 bottom left.
			1.0f,	1.0f,	1.0f,	1.0f,	// 2 top right.
			1.0f,	1.0f,	1.0f,	1.0f,	// 3 bottom right.
		};
		static SimpleSprite World, yellow_Dot, red_Dot, ATL1, ATL, paris_1, paris_2, paris_3, paris_4, LA, NYC, play_Sky_Line_1, play_Sky_Line_2,
				play_Sky_Line_3, play_Sky_Line_4, particle, Fire_skyline_1, Fire_skyline_2, Fire_skyline_3, Fire_skyline_4;
		static Rectangle skyline_1, skyline_2, skyline_3, skyline_4;
		static bool skyline_1_isHit, skyline_2_isHit, skyline_3_isHit, skyline_4_isHit, firstTime_1 = true, firstTime_2 = true, firstTime_3 = true, firstTime_4 = true;
		static List<SimpleSprite> dots;
		static List<SimpleSprite> dots_2;
		static List<Hits> hits;
		static Random generator;
		
		//networking stuff
		static bool loop = true;
		static string x, y, xy, z = " ";
		static int X, Y;
		static float starting_XPOS;
		static TcpClient tcpclnt;
		static NetworkStream ns;
		static StreamWriter sw;
		static StreamReader sr;
		static Thread listen;
		
		public static void Main (string[] args)
		{
			try
			{
			tcpclnt = new TcpClient ();
			
			//tcpclnt.Connect ("192.168.1.115", 8001);
			tcpclnt.Connect ("192.168.1.115", 8001);
			ns = tcpclnt.GetStream ();
			sw = new StreamWriter (ns);
			sr = new StreamReader (ns);
			
			text = sr.ReadLine ();
			positionArray = text.Split (',');
			ID = positionArray [0];
			
			listen = new Thread (new ThreadStart (incoming));
			listen.Start ();
			}
			catch
			{
				ID = "1";
			}
			Initialize ();
						
			while (loop) {
				SystemEvents.CheckEvents ();
				Update ();
				Render ();
			}
			
			Term ();
			tcpclnt.Close ();
		}

		public static void Initialize ()
		{
			graphics = new GraphicsContext ();
			ImageRect rectScreen = graphics.Screen.Rectangle;
			generator = new Random ();
			
			dots = new List<SimpleSprite> ();
			//dots_2 = new List<SimpleSprite> ();
			hits = new List<Hits> ();
			//creating texture2D in prep for the Simple Sprites
			texture = new Texture2D ("/Application/resources/world.png", false);
			texture2 = new Texture2D ("/Application/resources/dot.png", false);
			texture3 = new Texture2D ("/Application/resources/dot_1.png", false);
			Atlanta = new Texture2D ("/Application/resources/Atlanta.png", false);
			Paris_1 = new Texture2D ("/Application/resources/Paris_01.png", false);
			Paris_2 = new Texture2D ("/Application/resources/Paris_02.png", false);
			Paris_3 = new Texture2D ("/Application/resources/Paris_03.png", false);
			Paris_4 = new Texture2D ("/Application/resources/Paris_04.png", false);
			LosAngeles = new Texture2D ("/Application/resources/LA.png", false);
			NewYork = new Texture2D ("/Application/resources/NYC.png", false);
			particle_Tex = new Texture2D ("/Application/resources/fire_particle.png", false);
			skyline_1_fire = new Texture2D ("/Application/resources/Paris_01_hit.png", false);
			skyline_2_fire = new Texture2D ("/Application/resources/Paris_02_hit.png", false);
			skyline_3_fire = new Texture2D ("/Application/resources/Paris_03_hit.png", false);
			skyline_4_fire = new Texture2D ("/Application/resources/Paris_04_hit.png", false);
			//create array to put different player maps in.
			SimpleSprite[] skylines = new SimpleSprite[]
			{
				ATL1 = new SimpleSprite (graphics, Atlanta),
				paris_1 = new SimpleSprite (graphics, Paris_1),
				LA = new SimpleSprite (graphics, LosAngeles),
				NYC = new SimpleSprite (graphics, NewYork),
				ATL = new SimpleSprite (graphics, Atlanta),
			};
			
			for (int s = 0; s <= skylines.Length; ++s) {
				if (s == Convert.ToInt32 (ID)) {
					
					play_Sky_Line_1 = skylines [s];
					play_Sky_Line_1.Position.X = 0;
					play_Sky_Line_1.Position.Y = 0;
					
				}
				if (play_Sky_Line_1 == skylines [1]) {
					play_Sky_Line_2 = new SimpleSprite (graphics, Paris_2);
					play_Sky_Line_3 = new SimpleSprite (graphics, Paris_3);
					play_Sky_Line_4 = new SimpleSprite (graphics, Paris_4);
				}
				if (play_Sky_Line_1 == skylines [2]) {
					play_Sky_Line_2 = new SimpleSprite (graphics, Paris_2);
					play_Sky_Line_3 = new SimpleSprite (graphics, Paris_3);
					play_Sky_Line_4 = new SimpleSprite (graphics, Paris_4);
				}
				if (play_Sky_Line_1 == skylines [3]) {
					play_Sky_Line_2 = new SimpleSprite (graphics, Paris_2);
					play_Sky_Line_3 = new SimpleSprite (graphics, Paris_3);
					play_Sky_Line_4 = new SimpleSprite (graphics, Paris_4);
				}
				if (play_Sky_Line_1 == skylines [4]) {
					play_Sky_Line_2 = new SimpleSprite (graphics, Paris_2);
					play_Sky_Line_3 = new SimpleSprite (graphics, Paris_3);
					play_Sky_Line_4 = new SimpleSprite (graphics, Paris_4);
				}
				//if(skylines[s] == LA_1)
				//{//
				//play_Sky_Line_2 = skylines [LA_2];
				//play_Sky_Line_3 = skylines [LA_3];
				//play_Sky_Line_4 = skylines [LA_4];
				//}
				
			}
			play_Sky_Line_2.Position.X = 240;
			play_Sky_Line_2.Position.Y = 0;
			play_Sky_Line_3.Position.X = 480;
			play_Sky_Line_3.Position.Y = 0;
			play_Sky_Line_4.Position.X = 720;
			play_Sky_Line_4.Position.Y = 0;
			skyline_1 = new Rectangle (0, 410, Paris_1.Width, 134); 
			//skyline_2 = new Rectangle(240, 410, Paris_2.Width, 134); 
			//skyline_3 = new Rectangle(480, 410, Paris_3.Width, 134); 
			//skyline_4 = new Rectangle(720, 410, Paris_4.Width, 134); 
			
			World = new SimpleSprite (graphics, texture);
			Fire_skyline_1 = new SimpleSprite (graphics, skyline_1_fire);
			Fire_skyline_2 = new SimpleSprite (graphics, skyline_2_fire);
			Fire_skyline_3 = new SimpleSprite (graphics, skyline_3_fire);
			Fire_skyline_4 = new SimpleSprite (graphics, skyline_4_fire);
			yellow_Dot = new SimpleSprite (graphics, texture3);
			red_Dot = new SimpleSprite (graphics, texture2);
			particle = new SimpleSprite (graphics, particle_Tex);
			spriteArray [0] = red_Dot;
			spriteArray [1] = yellow_Dot;
			

		}
		
		public static void Term ()
		{
			//SampleDraw.Term ();
			graphics.Dispose ();
			//DOT.Dispose();
		}

		public static void Update ()
		{	
			//this gets this touch data from Vita screen and send coordinates to server	
			
			/*foreach (var touchData in Touch.GetData(0)) {
				if (touchData.Status == TouchStatus.Down || touchData.Status == TouchStatus.Move) {			
					X = (int)((touchData.X + 0.5f) * 960.0);
					Y = (int)((touchData.Y + 0.5f) * 544.0);					
					x = X.ToString ();
					y = Y.ToString ();		
					xy = x + "," + y;		
					if (xy != z) {
						sw.WriteLine (xy);
						sw.Flush ();				
					}
				}
			}
			*/
			
			//this reads from hits list. Checks to see what ID it has; based on this it will draw a differtn color dot.
			if (hits.Count != 0) {
				for (int i = hits.Count - 1; i >=0; --i) {
					if (hits.Count != count) {
						if (hits [i].Xpos > 340 && hits [i].Xpos < 390) {
							if (hits [i].Ypos > 120 && hits [i].Ypos < 170) {
								hits [i].ID = "1";
							}
							
						}
						
						if (hits [i].Xpos > 105 && hits [i].Xpos < 130) {
							if (hits [i].Ypos > 165 && hits [i].Ypos < 197) {
								hits [i].ID = "2";
							}
						}
						
						if (hits [i].Xpos > 190 && hits [i].Xpos < 215) {
							if (hits [i].Ypos > 155 && hits [i].Ypos < 180) {
								hits [i].ID = "3";
							}
						}
						
						if (hits [i].Xpos > 170 && hits [i].Xpos < 190) {
							if (hits [i].Ypos > 176 && hits [i].Ypos < 196) {
								hits [i].ID = "4";
							}
						}
							
							
						if (hits [i].ID == ID) {
							
							dots.Add (new SimpleSprite (graphics, texture2));
							dots [count_2].Position.X = generator.Next (lower, upper);
							dots [count_2].starting_x_pos = dots [count_2].Position.X;
							dots [count_2].rec = new Rectangle (dots [count_2].Position.X, dots [count_2].Position.Y, texture2.Width, texture2.Height);
							dots [count_2].Position.Y = 1;
							
							count_2++;							
						}
						/*if (hits [i].ID != ID) {
							
							dots_2.Add (new SimpleSprite (graphics, texture3));							
							dots_2 [count_3].Position.X = hits [i].Xpos - 10.0f;
							dots_2 [count_3].Position.Y = hits [i].Ypos - 10.0f;
							
							count_3++;						
						}*/
					
						count = hits.Count;
					}
				}
				
			}
			
		
			/*foreach(Hits h in hits)
			{
				if(h.ID == ID)
				{
					dots.Add (new SimpleSprite (graphics, texture2));
					dots [count_2].Position.X = h.Xpos - 10.0f;
					dots [count_2].Position.Y = h.Ypos - 10.0f;
					//count_2++;
				}
				
				else
				{
					dots_2.Add (new SimpleSprite (graphics, texture3));	
					
				
				}
				
			}
			*/
			
			z = xy;	
			dots.Add (new SimpleSprite (graphics, texture2));//
			//dots_2.Add (new SimpleSprite (graphics, texture3));	
			
			foreach (SimpleSprite s in dots) {
				if (dots.Count > 0) {
					//if((skyline_1.X + skyline_1.Width) < s.Position.X || (s.rec.X + s.rec.Width) < skyline_1.X || (skyline_1.Y + skyline_1.Height) < s.rec.Y || (s.rec.Y + s.rec.Height) < skyline_1.Y)
					if (s.Position.Y > 410 && s.Position.X < 240) {						
						s.isAlive = false;
						if(firstTime_1 == true)
						{
						skyline_1_isHit = true;	
							
						sw.WriteLine(ID);
							sw.Flush();
						if (skyline_1_isHit == true) {
							firstTime_1 = false;
							play_Sky_Line_1 = Fire_skyline_1;
							play_Sky_Line_1.Position.X = 0;
							play_Sky_Line_1.Position.Y = 0;	
						}
						}
					
					}
					if (s.Position.Y > 410 && s.Position.X > 240 && s.Position.X < 480) {
						s.isAlive = false;
						if(firstTime_2 == true)
						{
						skyline_2_isHit = true;
							
						sw.WriteLine(ID);
							sw.Flush();
						if (skyline_2_isHit == true) {
							firstTime_2 = false;
							play_Sky_Line_2 = Fire_skyline_2;
							play_Sky_Line_2.Position.X = 240;
							play_Sky_Line_2.Position.Y = 0;
						}
						}
				
					}	
					if (s.Position.Y > 410 && s.Position.X > 480 && s.Position.X < 720) {
						s.isAlive = false;
						if(firstTime_3 == true)
						{
						skyline_3_isHit = true;
							
						sw.WriteLine(ID);
							sw.Flush();
						if (skyline_3_isHit == true) {
							firstTime_3 = false;
							play_Sky_Line_3 = Fire_skyline_3;
							play_Sky_Line_3.Position.X = 480;
							play_Sky_Line_3.Position.Y = 0;
						}
						}
			
					}
					
					if (s.Position.Y > 410 && s.Position.X > 720) {
						s.isAlive = false;
						if(firstTime_4 == true)
						{
						skyline_4_isHit = true;
							
						sw.WriteLine(ID);
							sw.Flush();
						if (skyline_4_isHit == true) {
							firstTime_4 = false;
							play_Sky_Line_4 = Fire_skyline_4;
							play_Sky_Line_4.Position.X = 720;
							play_Sky_Line_4.Position.Y = 0;	
						}
					}	
					}
					
				}
			}
			
			/*for (int i = dots.Count - 1; i >= 0; i--)
            {
                SimpleSprite o = dots[i];
                if (!o.isAlive)
                {
                    dots.Remove(o);
                }
            }*/
			
		}

		public static void Render ()
		{
			graphics.SetClearColor (0.0f, 0.0f, 0.0f, 0.0f);	
			graphics.Clear ();
			//World.Render ();
			
			play_Sky_Line_1.Render ();
			
			
			play_Sky_Line_2.Render ();
			play_Sky_Line_3.Render ();
			play_Sky_Line_4.Render ();
			
			//if(skyline_1_isHit == true)
			//{
			//Fire_skyline_1.Render();
			//}
			
			
			if (dots != null) {
				foreach (SimpleSprite t in dots) {
					if (t.Position.X != 0 && t.Position.Y != 0) {
						t.Render ();
						if (t.starting_x_pos < 460) {
							t.Position.X++;
						}
						if (t.starting_x_pos > 460) {
							t.Position.X--;
						}
						t.Position.Y++;
						
					}
					
					
				}
			}
			if (dots_2 != null) {
				foreach (SimpleSprite n in dots_2) {
					if (n.Position.X != 0 && n.Position.Y != 0) {
						n.Render ();
					}
				}
			}
		
			
			graphics.SwapBuffers ();		
		}
		
		public static void incoming ()
		{
			while (true) {
				try{
				text = sr.ReadLine ();
				
				if (text != textTemp) {
					positionArray = text.Split (',');
					if(positionArray.Length > 2)
					{
						hits.Add (new Hits (positionArray [0], Convert.ToInt32 (positionArray [1]), Convert.ToInt32 (positionArray [2])));
					}
					
				}
				
				
				textTemp = text;
				}
				catch{
				}
			}
		}
		
		static string CharToString (char value)
		{
			/*
	 *
	 * Call ToString().
	 *
	 * */
			return value.ToString ();
		}
		
	
	}
}
