using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.Core.Environment;

using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.UI;

namespace Research_Game
{
	public class CreditScene : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		//credit scene we could we one image. :)
		
		private Sce.PlayStation.HighLevel.UI.Scene _uiScene;
		
		public CreditScene ()
		{
			//create a panel
			Sce.PlayStation.HighLevel.UI.Panel panel = new Sce.PlayStation.HighLevel.UI.Panel();
			panel.Width = Director.Instance.GL.Context.GetViewport().Width;
            panel.Height = Director.Instance.GL.Context.GetViewport().Height;			
			
			Button buttonUI1 = new Button(); //buttons
            buttonUI1.Name = "go back";
            buttonUI1.Text = "go back";
            buttonUI1.Width = 250;
            buttonUI1.Height = 50;
            buttonUI1.Alpha = 0.8f;
            buttonUI1.SetPosition(panel.Width/2.5f,panel.Height - 100);
            buttonUI1.TouchEventReceived += (sender, e) => {
				Support.SoundSystem.Instance.Play("ButtonClick.wav");
                Director.Instance.ReplaceScene(new MenuScene());
            };
			
			ImageBox ib = new ImageBox(); //set background images
            ib.Width = panel.Width;
            ib.Image = new ImageAsset("/Application/resources/Credits.png",false);
            ib.Height = panel.Height;
            ib.SetPosition(0.0f,0.0f);
			
			//add panel to rootwidget
			panel.AddChildLast(buttonUI1);
			_uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
			_uiScene.RootWidget.AddChildLast(ib);
			_uiScene.RootWidget.AddChildLast(panel);
			UISystem.SetScene(_uiScene);
			Scheduler.Instance.ScheduleUpdateForTarget(this,0,false); //run the loop
		}
		
		public override void Update (float dt)
        {
            base.Update (dt);
            UISystem.Update(Touch.GetData(0));         
        }
        
        public override void Draw ()
        {
            base.Draw();
            UISystem.Render ();
        }    
		
		~CreditScene()
		{
			Support.SoundSystem.Instance.Stop("ButtonClick.wav");
		}
	}
}

