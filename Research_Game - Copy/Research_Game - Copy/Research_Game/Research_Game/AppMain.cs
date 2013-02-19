using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.UI; 
using Sce.PlayStation.HighLevel.GameEngine2D;

namespace Research_Game
{
	public class AppMain
	{		
		public static GraphicsContext graphics;
		public static void Main (string[] args)
		{
			//run the game
			graphics = new GraphicsContext();
			Director.Initialize(300,300,graphics);
			UISystem.Initialize(Director.Instance.GL.Context);
			Director.Instance.RunWithScene(new OpeningScene()); 
		}
	}
}
