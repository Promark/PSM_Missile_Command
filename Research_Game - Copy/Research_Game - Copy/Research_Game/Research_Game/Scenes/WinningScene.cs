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
	public class WinningScene: Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		private Sce.PlayStation.HighLevel.UI.Scene _uiScene;
		
		public WinningScene ()
		{
			Console.WriteLine("------- in winning scene ---------");
			
			Initialize();
			
			while(true)
			{
				SystemEvents.CheckEvents ();
				Update();
				Render();
			}
		}
		
		public void Initialize ()
		{
            Sce.PlayStation.HighLevel.UI.Panel dialog = new Panel();//create panel
            dialog.Width = 960;//only for vita
            dialog.Height = 544;
			
			ImageBox ib = new ImageBox(); //set background images
            ib.Width = dialog.Width;
            ib.Image = new ImageAsset("/Application/resources/win.png",false);
            ib.Height = dialog.Height;
            ib.SetPosition(0.0f,0.0f);
			
			Button buttonUI1 = new Button(); //set buttons positions
            buttonUI1.Name = "replay";
            buttonUI1.Text = "replay";
            buttonUI1.Width = 250;
            buttonUI1.Height = 50;
            buttonUI1.Alpha = 0.8f;
            buttonUI1.SetPosition(dialog.Width/15,dialog.Height - 100);
            buttonUI1.TouchEventReceived += (sender, e) => {    
				Support.SoundSystem.Instance.Play("ButtonClick.wav");
				Director.Instance.ReplaceScene(new GameScene());
            };
            
            Button buttonUI2 = new Button();
            buttonUI2.Name = "home";
            buttonUI2.Text = "home";
            buttonUI2.Width = 250;
            buttonUI2.Height = 50;
            buttonUI2.Alpha = 0.8f;
            buttonUI2.SetPosition(dialog.Width/2.7f,dialog.Height - 100);
            buttonUI2.TouchEventReceived += (sender, e) => {
				Support.SoundSystem.Instance.Play("ButtonClick.wav");
				Director.Instance.ReplaceScene(new MenuScene());
            }; 
			
			Sce.PlayStation.HighLevel.UI.Label scoreLabel = new Sce.PlayStation.HighLevel.UI.Label();
			labelSetting(scoreLabel,                    				//total score
			             "Total Score: " + GameScene.totalScore,   					
			             690,				
			             0,							
			             300,							
			             400,							 
			             30,							 
			             FontStyle.Regular,				
			             new UIColor(255, 0, 0, 255));
			
			dialog.AddChildLast(ib);
			dialog.AddChildLast(buttonUI1);
            dialog.AddChildLast(buttonUI2);
			dialog.AddChildLast(scoreLabel);
			
			_uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
            _uiScene.RootWidget.AddChildLast(dialog);
            UISystem.SetScene(_uiScene); // create menu scene
		}
		
		public void Update ()
        {
			UISystem.Update(Touch.GetData(0));
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
		
		public void Render()
        {
			AppMain.graphics.SetClearColor (0.0f, 0.0f, 0.0f, 0.0f);
            AppMain.graphics.Clear ();
			
            UISystem.Render();
			
			AppMain.graphics.SwapBuffers ();
        }
	}
}

