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

namespace Research_Game
{
	public class OpeningScene: Scene
	{		
		public OpeningScene ()
		{
			this.Camera.SetViewFromViewport();//change camera view
						
			Console.WriteLine("------ opening scene ------------");
			Support.ImagesInSystem.Instance.OpenTextureInFile("openningScene.jpg");
			Support.ImagesInSystem.Instance.setPosition(Director.Instance.GL.Context.GetViewport().Width/2,
			                                            Director.Instance.GL.Context.GetViewport().Height/2);
			Support.ImagesInSystem.Instance.setScale(1.0f,1.0f);
			Support.ImagesInSystem.Instance.setPivot(0.5f,0.5f);
			Support.ImagesInSystem.Instance.addToChild(this);
            Scheduler.Instance.ScheduleUpdateForTarget(this,0,false);
			
			Support.MusicSystem.Instance.Play("game_game_music.mp3");

            // Clear any queued clicks so we dont immediately exit if coming in from the menu
            Touch.GetData(0).Clear();
		}
		
		//control to play the song == still got trouble on loading the music
//		public override void OnEnter ()
//        {
//			Support.MusicSystem.Instance.PlayNoClobber("SLEEP.mp3");
//        }
//        public override void OnExit ()
//        {
//            base.OnExit ();
//			Support.MusicSystem.Instance.Stop("SLEEP.mp3");
//        }
		
		public override void Update (float dt)
        {
            base.Update (dt);
            var touches = Touch.GetData(0).ToArray();
            if((touches.Length >0 && touches[0].Status == TouchStatus.Down) || Input2.GamePad0.Cross.Press)
            {
                Director.Instance.ReplaceScene(new MenuScene());
            }
        }
    
        ~OpeningScene()
        {
			Support.ImagesInSystem.Instance.removeFromChild(this);
			//Support.MusicSystem.Instance.Stop("game_game_music.mp3");
        }
	}
}

