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
	public class OptionScene: Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		private Sce.PlayStation.HighLevel.UI.Scene _uiScene;
		private Sce.PlayStation.HighLevel.UI.Slider sliderForMusic;
		private Sce.PlayStation.HighLevel.UI.Slider sliderForSound;
		public static bool isMusicCheckboxChanged = true; 
		public static bool isSoundCheckboxChanged = true; 
		
		public OptionScene ()
		{
			Console.WriteLine("---- option scene ----");
			this.Camera.SetViewFromViewport();
			
			Sce.PlayStation.HighLevel.UI.Panel panel = new Panel();//create panel
			panel.Width = Director.Instance.GL.Context.GetViewport().Width;
            panel.Height = Director.Instance.GL.Context.GetViewport().Height;
			
			Sce.PlayStation.HighLevel.UI.Label title_label = new Sce.PlayStation.HighLevel.UI.Label();//title label
			Sce.PlayStation.HighLevel.UI.Label music_label = new Sce.PlayStation.HighLevel.UI.Label();//music label
			Sce.PlayStation.HighLevel.UI.Label sound_label = new Sce.PlayStation.HighLevel.UI.Label();//sound label
			
			labelSetting(title_label,                   //label name
			             "Option",   					//label content
			             panel.Width/10,				//x position
			             10,							//y position
			             250,							//x size
			             100,							//y size
			             64,							//font size
			             FontStyle.Bold,				//font style
			             new UIColor(0, 255, 0, 255));	//font color
			
			labelSetting(music_label,                   
			             "Music :",   					
			             panel.Width/7,				
			             panel.Height/5,							
			             150,							
			             100,							 
			             32,							 
			             FontStyle.Regular,				
			             new UIColor(0, 255, 0, 255));
			
			labelSetting(sound_label,                   
			             "Sound :",   					
			             panel.Width/7,				
			             panel.Height/3f,							
			             150,							
			             100,							 
			             32,							 
			             FontStyle.Regular,				
			             new UIColor(0, 255, 0, 255));
	
			Sce.PlayStation.HighLevel.UI.CheckBox checkMusicButton = new CheckBox(); //music
			checkMusicButton.Checked = isMusicCheckboxChanged;
			checkMusicButton.SetPosition(panel.Width/3,panel.Height/4.5f);
			checkMusicButton.SetSize(50,50);
			checkMusicButton.CheckedChanged += HandleCheckMusicButtonCheckedChanged;
			
			sliderForMusic = new Sce.PlayStation.HighLevel.UI.Slider();
			sliderForMusic.SetPosition(panel.Width/2f,panel.Height/4.5f);
			sliderForMusic.SetSize(200,50);
			sliderForMusic.MinValue = 0;
			sliderForMusic.MaxValue = 1;
			sliderForMusic.Value = 0.5f;
			sliderForMusic.ValueChanging += HandleSliderForMusicValueChanging;//end music
			
			Sce.PlayStation.HighLevel.UI.CheckBox checkSoundButton = new CheckBox();//sound
			checkSoundButton.Checked = isSoundCheckboxChanged;
			checkSoundButton.SetPosition(panel.Width/3,panel.Height/2.5f);
			checkSoundButton.SetSize(50,50);
			checkSoundButton.CheckedChanged += HandleCheckSoundButtonCheckedChanged;
			
			sliderForSound = new Sce.PlayStation.HighLevel.UI.Slider();
			sliderForSound.SetPosition(panel.Width/2f,panel.Height/2.5f);
			sliderForSound.SetSize(200,50);
			sliderForSound.MinValue = 0;
			sliderForSound.MaxValue = 1;
			sliderForSound.Value = Support.SoundSystem.volumOfSound;
			sliderForSound.ValueChanging += HandleSliderForSoundValueChanging;//end sound
			
			Button buttonUI1 = new Button(); //buttons
            buttonUI1.Name = "go back";
            buttonUI1.Text = "go back";
            buttonUI1.Width = 250;
            buttonUI1.Height = 50;
            buttonUI1.Alpha = 0.8f;
            buttonUI1.SetPosition(panel.Width/5,panel.Height - 100);
            buttonUI1.TouchEventReceived += (sender, e) => {
				Support.SoundSystem.Instance.PlayNoClobber("ButtonClick.wav");
                Director.Instance.ReplaceScene(new MenuScene());
            };
			
			Button buttonUI2 = new Button();
            buttonUI2.Name = "reset";
            buttonUI2.Text = "reset";
            buttonUI2.Width = 250;
            buttonUI2.Height = 50;
            buttonUI2.Alpha = 0.8f;
            buttonUI2.SetPosition(panel.Width/2f,panel.Height - 100);
            buttonUI2.TouchEventReceived += (sender, e) => {
            	Support.SoundSystem.Instance.PlayNoClobber("ButtonClick.wav");
            }; 
			
			_uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
		    panel.AddChildLast(title_label);//add widgets in panel
			panel.AddChildLast(music_label);
			panel.AddChildLast(sound_label);
			panel.AddChildLast(checkMusicButton);
			panel.AddChildLast(checkSoundButton);
			panel.AddChildLast(sliderForMusic);
			panel.AddChildLast(sliderForSound);
			panel.AddChildLast(buttonUI1);
			panel.AddChildLast(buttonUI2);
			
			_uiScene.RootWidget.AddChildLast(panel);//add panel in rootwidget
            UISystem.SetScene(_uiScene); //add rootwidget in scene
            Scheduler.Instance.ScheduleUpdateForTarget(this,0,false); //run the loop
		}

		void HandleSliderForSoundValueChanging (object sender, SliderValueChangeEventArgs e)
		{
			Support.SoundSystem.volumOfSound = sliderForSound.Value;
		}
		
		void HandleSliderForMusicValueChanging (object sender, SliderValueChangeEventArgs e)
		{
			//right now you have to pass the music name and change the vlome based on that name
			Support.MusicSystem.MusicDatabase["game_game_music.mp3"].Volume = sliderForMusic.Value;
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
		
		void HandleCheckMusicButtonCheckedChanged (object sender, TouchEventArgs e)
		{
			if(isMusicCheckboxChanged == true)
			{
				sliderForMusic.Enabled = false;
				isMusicCheckboxChanged = false;
				
				//stop all the sound
				Support.MusicSystem.Instance.StopAll();
			}
			else
			{
				isMusicCheckboxChanged = true;
				sliderForMusic.Enabled = true;
				
				//stop all the sound
				Support.MusicSystem.Instance.Play("game_game_music.mp3");
			}
		}
		
		void HandleCheckSoundButtonCheckedChanged (object sender, TouchEventArgs e)
		{
		 	if(isSoundCheckboxChanged == true)
			{
				sliderForSound.Enabled = false;
				isSoundCheckboxChanged = false;
				
				Support.SoundSystem.volumOfSound = 0;
			}
			else
			{
				isSoundCheckboxChanged = true;
				sliderForSound.Enabled = true;
				
				Support.SoundSystem.volumOfSound = 0.5f;
			}
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
		
		~OptionScene()
		{
		   Support.SoundSystem.Instance.Stop("ButtonClick.wav");
		}
	}
}

