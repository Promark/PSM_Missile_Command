//This was written intial by Joshua L Skelton as part of research with Dr jon Preston
//It is a client for Sony devices as part of a Missile Command game to be developed
//within the Playstaion Mobile Suite and initially used on Vitas and Sony Tablets
//Date: 1/16/2013
//jskelto2@spsu.edu

//test to show Github
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
using System.Xml;
using System.Xml.Linq;
using Sce.PlayStation.HighLevel.UI;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Research_Game
{
	public class GameScene : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		#region variables
		/// <summary>
		/// Joshua's codes
		/// </summary>
		static string text, ID, textTemp = "temp"; //console_0 = "Client_0", 
		static string[] positionArray = {""};
		static SimpleSprite[] spriteArray = new SimpleSprite[2];
		//static protected GraphicsContext graphics;
		static Texture2D texture2,
		 				 texture2_left,
						 texture2_right,
						 texture3, 
						 Paris_1, 
						 Paris_2, 
						 Paris_3, 
						 Paris_4, 
						 Atlanta, 
						 Atlanta_01,
						 Atlanta_02,
						 Atlanta_03,
						 Atlanta_04,
						 LosAngeles, 
						 LA_01,
						 LA_02,
						 LA_03,
						 LA_04,
						 NewYork,
						 NYC_01,
						 NYC_02,
						 NYC_03,
						 NYC_04,
						 particle_Tex, 
						 skyline_1_fire, 
						 skyline_2_fire, 
						 skyline_3_fire, 
						 skyline_4_fire, 
						 explosionPic;
		static int count = 0, count_2 = 0, lower = 0, upper = 960; //count_3 = 0, 
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
		static SimpleSprite yellow_Dot, red_Dot, ATL1, ATL, paris_1, LA, NYC, play_Sky_Line_1, play_Sky_Line_2,
				play_Sky_Line_3, play_Sky_Line_4, particle, Fire_skyline_1, Fire_skyline_2, Fire_skyline_3, Fire_skyline_4; //explosion
		static Rectangle skyline_1;
		static bool skyline_1_isHit, skyline_2_isHit, skyline_3_isHit, skyline_4_isHit, firstTime_1 = true, firstTime_2 = true, firstTime_3 = true, firstTime_4 = true;
		static List<SimpleSprite> dots;
		//static List<SimpleSprite> dots_2;
		static List<Hits> hits;
		static Random generator;
		
		static bool loop;
		static string x, y;
		static int X, Y;
		//static float starting_XPOS;
		static TcpClient tcpclnt;
		static NetworkStream ns;
		static StreamWriter sw;
		static StreamReader sr;
		static Thread listen;
		
		//create label for time and score
		private Sce.PlayStation.HighLevel.UI.Label timeCount, score;
		
		//scene
		private Sce.PlayStation.HighLevel.UI.Scene _uiScene;
		
		//timer
		private System.Diagnostics.Stopwatch timer;
		
		//score label
		private Sce.PlayStation.HighLevel.UI.Label labelOfScore;
		
		//health bar
		private ImageBox healthbar_01, healthbar_02, healthbar_03, healthbar_04;
		
		//explosion
		public Explosion ex = null;
		
		//health judge
		public bool healthOneDelete = false, healthTwoDelete = false, healthThreeDelete = false, healthFourDelete = false;
		
		//record the score
		public static int totalScore = 0;
		
		//state change
		enum gameStates
 		{
			gamePlay,
			gameOver,
			gameWin,
			explosion,
		}
	    gameStates currentState = gameStates.gamePlay;
		#endregion
		
		public GameScene()
		{
			try
			{
				//licence from server -- position x and y -- and then send back
				//the damage 
				tcpclnt = new TcpClient();
				tcpclnt.Connect ("192.168.1.103", 8001);
				ns = tcpclnt.GetStream ();
				sw = new StreamWriter (ns);
				sr = new StreamReader (ns);
				
				//set up your own id and send to server
//				if(!isSendIdToServer)
//				{
//					ID = "0";
//					sw.WriteLine(ID);	
//					sw.Flush();
//					isSendIdToServer = true;
//				}
				
				text = sr.ReadLine (); // id + x + y;
				positionArray = text.Split (',');
				ID = positionArray [0]; //
				
				//Console.WriteLine("first time read =" + text);
				//incoming();
				listen = new Thread (new ThreadStart (incoming));
				listen.Start ();
			}
			catch
			{
				//ID = "1";
			}
			
			Initialize ();
			loop = true;
			currentState = gameStates.gamePlay;
			
			while (loop) {
				SystemEvents.CheckEvents ();
				Update ();
				Render ();
			}
		}

		public void Initialize ()
		{
			//graphics = new GraphicsContext();
			//UISystem.Initialize(graphics);
			Console.WriteLine("---- game scene -----");
			
			ImageRect rectScreen = AppMain.graphics.Screen.Rectangle;
			generator = new Random ();
			
			dots = new List<SimpleSprite> ();
			
			//dots_2 = new List<SimpleSprite> ();
			hits = new List<Hits> ();
			//creating texture2D in prep for the Simple Sprites
			//texture = new Texture2D ("/Application/resources/world.png", false);
			texture2 = new Texture2D ("/Application/resources/Bomb_mini.png", false);
			texture2_left = new Texture2D("/Application/resources/Bomb_mini_left.png", false);
			texture2_right = new Texture2D("/Application/resources/Bomb_mini_right.png", false);
			texture3 = new Texture2D ("/Application/resources/dot_1.png", false);
			Atlanta = new Texture2D ("/Application/resources/Atlanta.png", false);
			Paris_1 = new Texture2D ("/Application/resources/Paris_01.png", false);
			Paris_2 = new Texture2D ("/Application/resources/Paris_02.png", false);
			Paris_3 = new Texture2D ("/Application/resources/Paris_03.png", false);
			Paris_4 = new Texture2D ("/Application/resources/Paris_04.png", false);
			Atlanta_01 = new Texture2D("/Application/resources/Atlanta_01.png", false);
			Atlanta_02 = new Texture2D("/Application/resources/Atlanta_02.png", false);
			Atlanta_03 = new Texture2D("/Application/resources/Atlanta_03.png", false);
			Atlanta_04 = new Texture2D("/Application/resources/Atlanta_04.png", false);
			LA_01 = new Texture2D("/Application/resources/LA_01.png", false); 
			LA_02 = new Texture2D("/Application/resources/LA_02.png", false);
			LA_03 = new Texture2D("/Application/resources/LA_03.png", false);
			LA_04 = new Texture2D("/Application/resources/LA_04.png", false);
			NYC_01= new Texture2D("/Application/resources/NYC_01.png", false);
			NYC_02= new Texture2D("/Application/resources/NYC_02.png", false);
			NYC_03= new Texture2D("/Application/resources/NYC_03.png", false);
			NYC_04= new Texture2D("/Application/resources/NYC_04.png", false);
			LosAngeles = new Texture2D ("/Application/resources/LA.png", false);
			NewYork = new Texture2D ("/Application/resources/NYC.png", false);
			particle_Tex = new Texture2D ("/Application/resources/fire_particle.png", false);
			skyline_1_fire = new Texture2D ("/Application/resources/Paris_01_hit.png", false);
			skyline_2_fire = new Texture2D ("/Application/resources/Paris_02_hit.png", false);
			skyline_3_fire = new Texture2D ("/Application/resources/Paris_03_hit.png", false);
			skyline_4_fire = new Texture2D ("/Application/resources/Paris_04_hit.png", false);
			explosionPic = new Texture2D("/Application/resources/GraySmoke.png",false);
			
			//create array to put different player maps in.
			SimpleSprite[] skylines = new SimpleSprite[]
			{
				ATL1 = new SimpleSprite (AppMain.graphics, Atlanta_01),//0
				paris_1 = new SimpleSprite (AppMain.graphics, Paris_1),//1
				LA = new SimpleSprite (AppMain.graphics, LA_01),
				NYC = new SimpleSprite (AppMain.graphics, NYC_01),
				ATL = new SimpleSprite (AppMain.graphics, Atlanta_01),
			};
			
			for (int s = 0; s <= skylines.Length; ++s) {
				if (s == Convert.ToInt32 (ID)) {
					
					play_Sky_Line_1 = skylines [s];
					play_Sky_Line_1.Position.X = 0;
					play_Sky_Line_1.Position.Y = 0;
					
				}
				if (play_Sky_Line_1 == skylines [1]) {
					play_Sky_Line_2 = new SimpleSprite (AppMain.graphics, Paris_2);
					play_Sky_Line_3 = new SimpleSprite (AppMain.graphics, Paris_3);
					play_Sky_Line_4 = new SimpleSprite (AppMain.graphics, Paris_4);
				}
				if (play_Sky_Line_1 == skylines [2]) {
					play_Sky_Line_2 = new SimpleSprite (AppMain.graphics, LA_02);
					play_Sky_Line_3 = new SimpleSprite (AppMain.graphics, LA_03);
					play_Sky_Line_4 = new SimpleSprite (AppMain.graphics, LA_04);
				}
				if (play_Sky_Line_1 == skylines [3]) {
					play_Sky_Line_2 = new SimpleSprite (AppMain.graphics, NYC_02);
					play_Sky_Line_3 = new SimpleSprite (AppMain.graphics, NYC_03);
					play_Sky_Line_4 = new SimpleSprite (AppMain.graphics, NYC_04);
				}
				if (play_Sky_Line_1 == skylines [4]) {
					play_Sky_Line_2 = new SimpleSprite (AppMain.graphics, Atlanta_02);
					play_Sky_Line_3 = new SimpleSprite (AppMain.graphics, Atlanta_03);
					play_Sky_Line_4 = new SimpleSprite (AppMain.graphics, Atlanta_04);
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
			
			//World = new SimpleSprite (AppMain.graphics, texture);
			Fire_skyline_1 = new SimpleSprite (AppMain.graphics, skyline_1_fire);
			Fire_skyline_2 = new SimpleSprite (AppMain.graphics, skyline_2_fire);
			Fire_skyline_3 = new SimpleSprite (AppMain.graphics, skyline_3_fire);
			Fire_skyline_4 = new SimpleSprite (AppMain.graphics, skyline_4_fire);
			yellow_Dot = new SimpleSprite (AppMain.graphics, texture3);
			red_Dot = new SimpleSprite (AppMain.graphics, texture2);
			particle = new SimpleSprite (AppMain.graphics, particle_Tex);
			spriteArray [0] = red_Dot;
			spriteArray [1] = yellow_Dot;
			
			timeCount = new Sce.PlayStation.HighLevel.UI.Label();
			score = new Sce.PlayStation.HighLevel.UI.Label();
			labelOfScore = new Sce.PlayStation.HighLevel.UI.Label();
			labelSetting(timeCount,                    //time count
			             "30",   					
			             960/2 - 20,				
			             0,							
			             150,							
			             100,							 
			             32,							 
			             FontStyle.Regular,				
			             new UIColor(0, 0, 0, 255));
			timer = new System.Diagnostics.Stopwatch();//timer
			
			labelSetting(score,                       //score
			             "0",   					
			             960 - 100,				
			             0,							
			             150,							
			             100,							 
			             20,							 
			             FontStyle.Regular,				
			             new UIColor(0, 0, 0, 255));
			
			labelSetting(labelOfScore,                //score board
			             "Score: ",   					
			             960 - 200,				
			             0,							
			             150,							
			             100,							 
			             20,							 
			             FontStyle.Regular,				
			             new UIColor(0, 0, 0, 255));
			
			//heath bar -- saparate to 5 parts
			_uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
			ImageBox healthIcon = new ImageBox();
			healthIcon.Image = new ImageAsset("/Application/resources/shield-grey.png");
			healthIcon.PivotType = PivotType.MiddleCenter;
			healthIcon.Width = healthIcon.Image.Width/4.5f;
			healthIcon.Height = healthIcon.Image.Height/4.5f;
			healthIcon.SetPosition(35,40);
			
			healthbar_01 = new ImageBox();
			healthbar_01.Image = new ImageAsset("/Application/resources/red-rectangle.jpg");
			healthbar_01.PivotType = PivotType.MiddleCenter;
			healthbar_01.Width = healthbar_01.Image.Width/4.5f;
			healthbar_01.Height = healthbar_01.Image.Height/4.5f;
			healthbar_01.SetPosition(100,40);
			
			healthbar_02 = new ImageBox();
			healthbar_02.Image = new ImageAsset("/Application/resources/red-rectangle.jpg");
			healthbar_02.PivotType = PivotType.MiddleCenter;
			healthbar_02.Width = healthbar_02.Image.Width/4.5f;
			healthbar_02.Height = healthbar_02.Image.Height/4.5f;
			healthbar_02.SetPosition(155,40);
			
			healthbar_03 = new ImageBox();
			healthbar_03.Image = new ImageAsset("/Application/resources/red-rectangle.jpg");
			healthbar_03.PivotType = PivotType.MiddleCenter;
			healthbar_03.Width = healthbar_03.Image.Width/4.5f;
			healthbar_03.Height = healthbar_03.Image.Height/4.5f;
			healthbar_03.SetPosition(210,40);
			
			healthbar_04 = new ImageBox();
			healthbar_04.Image = new ImageAsset("/Application/resources/red-rectangle.jpg");
			healthbar_04.PivotType = PivotType.MiddleCenter;
			healthbar_04.Width = healthbar_04.Image.Width/4.5f;
			healthbar_04.Height = healthbar_04.Image.Height/4.5f;
			healthbar_04.SetPosition(265,40);

			_uiScene.RootWidget.AddChildLast(timeCount);
			_uiScene.RootWidget.AddChildLast(score);
			_uiScene.RootWidget.AddChildLast(healthIcon);
			_uiScene.RootWidget.AddChildLast(healthbar_01);
			_uiScene.RootWidget.AddChildLast(healthbar_02);
			_uiScene.RootWidget.AddChildLast(healthbar_03);
			_uiScene.RootWidget.AddChildLast(healthbar_04);
			_uiScene.RootWidget.AddChildLast(labelOfScore);
            UISystem.SetScene(_uiScene); // create menu scene
		}
		
		void labelSetting(Sce.PlayStation.HighLevel.UI.Label label, string content, float xPosition, float yPosition, float xSize,
		                  float ySize, int fontSize, FontStyle fontStyle, UIColor colorOfFont)
		{
			label.Text = content;
			label.SetPosition(xPosition,yPosition);
			label.SetSize(xSize,ySize);
			label.TextTrimming = TextTrimming.EllipsisCharacter;
			label.Font = new UIFont(FontAlias.System, fontSize, fontStyle);
			label.LineBreak = LineBreak.Word;
			label.TextColor = colorOfFont;
		}
		
		public void Term ()
		{
			//SampleDraw.Term ();
			//UISystem.Terminate();
			AppMain.graphics.Dispose ();
			//DOT.Dispose();
		}
		
		public void Update ()
        {
			#region game state play
			if(currentState == gameStates.gamePlay)
			{	
				//start time count down, when reach to 0, then jump to winning
				timer.Start();
				if(timer.ElapsedMilliseconds > 1000f)
				{
					//if time count to 0, win the game
					if(Int32.Parse(timeCount.Text) > 0)
					{
						timeCount.Text = (Int32.Parse(timeCount.Text) - 1).ToString();
						timer.Reset();	
					}
					else
					{
						currentState = gameStates.gameWin;	
						timer.Stop();
					}
				}	
				
				//if the health count to 0, lose the game
				if(healthFourDelete && healthThreeDelete && healthTwoDelete && healthOneDelete)
				{
					timer.Stop();
					currentState = gameStates.gameOver;
				}
			
				foreach (var touchData in Touch.GetData(0)) {
					if (touchData.Status == TouchStatus.Down) {			
						X = (int)((touchData.X + 0.5f) * 960.0);
						Y = (int)((touchData.Y + 0.5f) * 544.0);					
						x = X.ToString ();
						y = Y.ToString ();		
//						xy = x + "," + y;		
//						if (xy != z) {
//							sw.WriteLine (xy);
//							sw.Flush ();				
//						}
						//check whether the touch in the dots areas
						foreach(var eachDot in dots)
						{
							if((System.Math.Abs(eachDot.Position.X - X)<40) && (System.Math.Abs(eachDot.Position.Y - Y)<40))
							{
								//remove the code, add score, explosion happen
								eachDot.isAlive = false;
								var scoreInt = Int32.Parse(score.Text);
								scoreInt += 1;
								totalScore = scoreInt;
								score.Text = scoreInt.ToString();
								ex = new Explosion(explosionPic,new Vector3(eachDot.Position.X, eachDot.Position.Y, eachDot.Position.Z));
								Support.SoundSystem.Instance.Play("explosionMissile.wav");
								currentState = gameStates.explosion;
							}
						}
					}
				}
				
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
									var randomGenerateX = generator.Next (lower, upper);
								    // Console.WriteLine("dotsX =" + randomGenerateX);
									if(randomGenerateX < 490)
									{
										dots.Add (new SimpleSprite (AppMain.graphics, texture2_left));
									}
									
									if(randomGenerateX > 490)
									{
										dots.Add (new SimpleSprite (AppMain.graphics, texture2_right));
									}
									
									if(randomGenerateX == 490)
										dots.Add (new SimpleSprite (AppMain.graphics, texture2));
								
									dots [count_2].Position.X = randomGenerateX;
									dots [count_2].starting_x_pos = randomGenerateX;
									dots [count_2].rec = new Rectangle (dots [count_2].Position.X, dots [count_2].Position.Y, texture2.Width, texture2.Height);
									dots [count_2].Position.Y = 1;
									//Console.WriteLine("dotsX =" + dots[count_2].Position.X + " Y =" + dots[count_2].Position.Y);
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
				
				
				foreach (SimpleSprite s in dots) {
					if (dots.Count > 0) {
						//if((skyline_1.X + skyline_1.Width) < s.Position.X || (s.rec.X + s.rec.Width) < skyline_1.X || (skyline_1.Y + skyline_1.Height) < s.rec.Y || (s.rec.Y + s.rec.Height) < skyline_1.Y)
						if (s.Position.Y > 410 && s.Position.X < 240) {						
							s.isAlive = false;
							if(firstTime_1 == true)
							{
								skyline_1_isHit = true;	
								//Support.SoundSystem.Instance.Play("missileBroken.wav");
								if(!healthFourDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_04);
								   healthFourDelete = true;
								    totalScore -= 10;
								}
								else if(!healthThreeDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_03);
								   healthThreeDelete = true;
									 totalScore -= 10;
								}
								else if(!healthTwoDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_02);
								   healthTwoDelete = true;
									 totalScore -= 10;
								}
								else if(!healthOneDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_01);
								   healthOneDelete = true;	
									 totalScore -= 10;
								}
								
								sw.WriteLine(ID);//if the image change, send to server 
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
								//Support.SoundSystem.Instance.Play("missileBroken.wav");
								if(!healthFourDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_04);
								   healthFourDelete = true;
								    totalScore -= 10;
								}
								else if(!healthThreeDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_03);
								   healthThreeDelete = true;
									 totalScore -= 10;
								}
								else if(!healthTwoDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_02);
								   healthTwoDelete = true;
									 totalScore -= 10;
								}
								else if(!healthOneDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_01);
								   healthOneDelete = true;	
									 totalScore -= 10;
								}
								
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
								//Support.SoundSystem.Instance.Play("missileBroken.wav");
								if(!healthFourDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_04);
								   healthFourDelete = true;
									 totalScore -= 10;
								}
								else if(!healthThreeDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_03);
								   healthThreeDelete = true;
									 totalScore -= 10;
								}
								else if(!healthTwoDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_02);
								   healthTwoDelete = true;
									 totalScore -= 10;
								}
								else if(!healthOneDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_01);
								   healthOneDelete = true;	
									 totalScore -= 10;
								}
								
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

								if(!healthFourDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_04);
								   healthFourDelete = true;
									 totalScore -= 10;
								}
								else if(!healthThreeDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_03);
								   healthThreeDelete = true;
									 totalScore -= 10;
								}
								else if(!healthTwoDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_02);
								   healthTwoDelete = true;
									 totalScore -= 10;
								}
								else if(!healthOneDelete)
								{
								   _uiScene.RootWidget.RemoveChild(healthbar_01);
								   healthOneDelete = true;
									 totalScore -= 10;
								}
								
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
				
				//REMOVE THE DOTS WHEN OFF THE SCREEN
//				for (int i = dots.Count - 1; i > 0; i--)
//	            {
//	                SimpleSprite o = dots[i];
//	                if (!o.isAlive)
//	                {
//	                    dots.Remove(o);
//	                }
//	            }
			}
			#endregion
			
			#region game state game over
			if(currentState == gameStates.gameOver)
			{	
				if(Director.Instance.CurrentScene.IsRunning)
				{
     			   Director.Instance.ReplaceScene(new LoseScene());
				}
			}
			#endregion
			
			#region game state winning
			if(currentState == gameStates.gameWin)
			{
				if(Director.Instance.CurrentScene.IsRunning)
				{
					Director.Instance.ReplaceScene(new WinningScene());	
				}
			}
			#endregion
			
			#region game state explosion
			if(currentState == gameStates.explosion)
			{
				var explosionAliveDetection = ex.getSpriteAlive();
				if(explosionAliveDetection == true)
					ex.Update();
				else
					currentState = gameStates.gamePlay;
			}
			#endregion
			
			//Director.Instance.Update();
			//Console.WriteLine("hits ==" + hits.Count);
        }
        
        public void Render ()
        {
			AppMain.graphics.SetClearColor (0.0f, 0.0f, 0.0f, 0.0f);	
			AppMain.graphics.Clear ();
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
					if ((t.Position.X != 0) && (t.Position.Y != 0) && (t.isAlive)) {
						t.Render ();
						if (t.starting_x_pos < 490) {
							t.Position.X++;
						}
						if (t.starting_x_pos > 490) {
							t.Position.X--;
							
						}
						
						t.Position.Y++;
					}
					
					
				}
			}
//			if (dots_2 != null) {
//				foreach (SimpleSprite n in dots_2) {
//					if (n.Position.X != 0 && n.Position.Y != 0) {
//						n.Render ();
//					}
//				}
//			}
			
			if(currentState == gameStates.explosion)//render only when its in this state
			{
				ex.Render();
			}
			
			UISystem.Render ();
			AppMain.graphics.SwapBuffers ();	
        }
		
		public void incoming ()
		{
			while (true) {
				try{
					text = sr.ReadLine ();
					
					if (text != textTemp) {
						
						positionArray = text.Split (',');
						//Console.WriteLine("position array =" + positionArray);
						if(positionArray.Length > 2)
						{
							//Console.WriteLine("dots add to hits array = " + positionArray[0] +", "+ positionArray[1] +", "+ positionArray[2]);
							hits.Add (new Hits (positionArray [0], Convert.ToInt32 (positionArray [1]), Convert.ToInt32 (positionArray [2])));
						}
						
					}
	
					textTemp = text;
				}
				catch(Exception e)
				{
					Console.WriteLine( e.Message);
				}
			}
		}
		
		~GameScene()
		{
			Term ();
			listen.Abort();
			sw.Close();
			sr.Close();
			tcpclnt.Close ();
			dots.Clear();
			hits.Clear();
		}
	}
}


