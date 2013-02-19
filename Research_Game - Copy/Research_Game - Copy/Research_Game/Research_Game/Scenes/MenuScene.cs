using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

using Sce.PlayStation.HighLevel.UI;

namespace Research_Game
{
	public class MenuScene : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		private Sce.PlayStation.HighLevel.UI.Scene _uiScene;
        
        public MenuScene ()
		{		
			Console.WriteLine("--------- menu scene -------");
			
            this.Camera.SetViewFromViewport();//set camera view point
            Sce.PlayStation.HighLevel.UI.Panel dialog = new Panel();//create panel
            dialog.Width = Director.Instance.GL.Context.GetViewport().Width;
            dialog.Height = Director.Instance.GL.Context.GetViewport().Height;
            
            ImageBox ib = new ImageBox(); //set background images
            ib.Width = dialog.Width;
            ib.Image = new ImageAsset("/Application/resources/menuScene.png",false);
            ib.Height = dialog.Height;
            ib.SetPosition(0.0f,0.0f);
            
            Button buttonUI1 = new Button(); //set buttons positions
            buttonUI1.Name = "buttonPlay";
            buttonUI1.Text = "Play Game";
            buttonUI1.Width = 250;
            buttonUI1.Height = 50;
            buttonUI1.Alpha = 0.8f;
            buttonUI1.SetPosition(dialog.Width/15,dialog.Height - 100);
            buttonUI1.TouchEventReceived += (sender, e) => {    
				Support.SoundSystem.Instance.Play("ButtonClick.wav");
				Director.Instance.ReplaceScene(new GameScene());
            };
            
            Button buttonUI2 = new Button();
            buttonUI2.Name = "buttonOption";
            buttonUI2.Text = "Option";
            buttonUI2.Width = 250;
            buttonUI2.Height = 50;
            buttonUI2.Alpha = 0.8f;
            buttonUI2.SetPosition(dialog.Width/2.7f,dialog.Height - 100);
            buttonUI2.TouchEventReceived += (sender, e) => {
				Support.SoundSystem.Instance.Play("ButtonClick.wav");
            	Director.Instance.ReplaceScene(new OptionScene());
            }; 
			
			Button buttonUI3 = new Button();
            buttonUI3.Name = "buttonCredit";
            buttonUI3.Text = "Credit";
            buttonUI3.Width = 250;
            buttonUI3.Height = 50;
            buttonUI3.Alpha = 0.8f;
            buttonUI3.SetPosition(dialog.Width/1.5f,dialog.Height - 100);
            buttonUI3.TouchEventReceived += (sender, e) => {
				Support.SoundSystem.Instance.Play("ButtonClick.wav");
            	Director.Instance.ReplaceScene(new CreditScene());
            };
                
            dialog.AddChildLast(ib);
            dialog.AddChildLast(buttonUI1);
            dialog.AddChildLast(buttonUI2);
			dialog.AddChildLast(buttonUI3);
			
            _uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
            _uiScene.RootWidget.AddChildLast(dialog);
            UISystem.SetScene(_uiScene); // create menu scene
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
        
        ~MenuScene()
        {
		   Support.SoundSystem.Instance.Stop("ButtonClick.wav");
        }	
	}
}

