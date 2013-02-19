using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.Core.Environment;

using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.UI;

namespace Research_Game
{
	public class Explosion
	{
		float speed=3;
		int cnt=0;
		SimpleSprite sprite;
		
		public Explosion(Texture2D textrue, Vector3 position) 
		{
			sprite = new SimpleSprite( AppMain.graphics, textrue);
			sprite.Center.X = 0.5f;
			sprite.Center.Y = 0.5f;
			sprite.Position = position;
		}

		public void Update()
		{
			sprite.Position.Y += speed;

			if(++cnt >=30)
			{
				sprite.isAlive = false;
			}
			
			sprite.SetColor(new Vector4(1.0f, 1.0f, 1.0f, (30-cnt)/30.0f));
			sprite.Scale.X += 0.02f;
			sprite.Scale.Y += 0.02f;
		}
		
		public void Render()
		{
			sprite.Render();	
		}
		
		public bool getSpriteAlive()
		{
			return sprite.isAlive;	
		}
	}
}

