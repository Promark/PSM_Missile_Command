using System.Collections.Generic;
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Research_Game
{
	public static class Support
	{
		//class for supporting spriteSheet, music, sound ...	
		public class SoundSystem
		{
			public static float volumOfSound = 0.5f; //change the volume in optionScene
			public static SoundSystem Instance = new SoundSystem("/Application/resources/soundNmusic/");
			public string AssetsPrefix;
			
			public Dictionary<string, SoundPlayer> SoundDatabase;

			public SoundSystem(string assets_prefix)
			{
				AssetsPrefix = assets_prefix;
				SoundDatabase = new Dictionary<string,SoundPlayer>();
			}

			public void CheckCache(string name)
			{
				if (SoundDatabase.ContainsKey(name)){
					return;
				}
				
				Console.WriteLine(AssetsPrefix + name);
				using (var sound = new Sound(AssetsPrefix + name) )
				{
					var player = sound.CreatePlayer();
					SoundDatabase[name] = player;
				}
			}

			public void Play(string name)
			{
				CheckCache(name);

				// replace any playing instance
				SoundDatabase[name].Stop();
				SoundDatabase[name].Play();
				SoundDatabase[name].Volume = volumOfSound;
			}

			public void Stop(string name)
			{
				CheckCache(name);
				SoundDatabase[name].Stop();
			}

			public void PlayNoClobber(string name)
			{
				CheckCache(name);

				if (SoundDatabase[name].Status == SoundStatus.Playing){
					return;
				}

				SoundDatabase[name].Play();
				SoundDatabase[name].Volume = volumOfSound;
			}
		}
		
		public class MusicSystem
		{
			public static MusicSystem Instance = new MusicSystem("/Application/resources/soundNmusic/");
			public string AssetsPrefix;
			public static Dictionary<string, BgmPlayer> MusicDatabase;

			public MusicSystem(string assets_prefix)
			{
				AssetsPrefix = assets_prefix;
				MusicDatabase = new Dictionary<string, BgmPlayer>();
			}

			public void StopAll()
			{
				foreach (KeyValuePair<string, BgmPlayer> kv in MusicDatabase)
				{
					kv.Value.Stop();
					kv.Value.Dispose();
				}

				MusicDatabase.Clear();
			}

			public void Play(string name)
			{
				StopAll();

				using (var music = new Bgm(AssetsPrefix + name) )
				{
					var player = music.CreatePlayer();
					MusicDatabase[name] = player;
					MusicDatabase[name].Play();
					MusicDatabase[name].Loop = true;
					MusicDatabase[name].Volume = 0.5f;
				}
			}

			public void Stop(string name)
			{
				StopAll();
			}

			public void PlayNoClobber(string name)
			{
				if (MusicDatabase.ContainsKey(name))
				{
					if (MusicDatabase[name].Status == BgmStatus.Playing)
					{
						return;
					}
				}

				Play(name);
			}
		}
		
		public class ImagesInSystem //use for creating Texture2D and add to spriteList
		{
			public static ImagesInSystem Instance = new ImagesInSystem("/Application/resources/");
			public string AssetsPrefix;
			
			private TextureInfo tInfor;
       		private Texture2D textureforOpening;
			private Sce.PlayStation.HighLevel.GameEngine2D.SpriteList spriteList;
			private SpriteUV sprite;
			
			public ImagesInSystem(string assets_prefix)
			{
				AssetsPrefix = assets_prefix;
			}
			
			public void OpenTextureInFile(string name)
			{
				textureforOpening = new Texture2D(AssetsPrefix + name,false);
				tInfor = new TextureInfo(textureforOpening);
				sprite = new SpriteUV(tInfor);
				spriteList = new Sce.PlayStation.HighLevel.GameEngine2D.SpriteList(tInfor);
				spriteList.AddChild(sprite);
			}
			
			public void setPosition(float posX, float posY)
			{
				sprite.Position = new Vector2(posX, posY);
			}
			
			public void setScale(float scalX, float scalY)
			{
				sprite.Scale = new Vector2(tInfor.TextureSizef.X / scalX, tInfor.TextureSizef.Y/ scalY);
			}
			
			public void setPivot(float pivotX, float pivotY)
			{
				sprite.Pivot = new Vector2(pivotX, pivotY);	
			}
			
			public void addToChild(Sce.PlayStation.HighLevel.GameEngine2D.Node node)
			{
				node.AddChild(spriteList);	
			}
			
			public void removeFromChild(Sce.PlayStation.HighLevel.GameEngine2D.Node node)
			{
				node.Cleanup();	
			}
		}
		
		public class SpriteSheet //filestream, streamReader are not available in static class
		{
//			public static SpriteSheet Instance = new SpriteSheet("/Application/resources/");
//			public string AssetsPrefix;
//			
//			private string spriteSheetPic, spriteSheetXML;
//			private TextureInfo _textureInfo;
//			private Texture2D _texture;
//			private System.Collections.Generic.Dictionary<string,Sce.PlayStation.HighLevel.GameEngine2D.Base.Vector2i> _sprites; 
//			
//			public SpriteSheet(string assetsPrefix)
//			{
//				AssetsPrefix = assetsPrefix;
//			}
//			
//			public void LoadTexture2DNXMLFiles(string spriteSheetPic,string spriteSheetXML)
//			{
//				sthis.spriteSheetPic = spriteSheetPic;
//				this.spriteSheetXML = spriteSheetXML;
//				
//				FileStream fileStream = File.OpenRead(AssetsPrefix + spriteSheetXML);//XML
//				StreamReader fileStreamReader = new StreamReader(fileStream);
//				string xml = fileStreamReader.ReadToEnd();
//				fileStreamReader.Close();
//				fileStream.Close();
//				XDocument doc = XDocument.Parse(xml);
//				
//				var lines = from sprite in doc.Root.Elements("sprite")
//					select new
//					{
//						Name = sprite.Attribute("n").Value,
//						X1 = (int)sprite.Attribute ("x"),
//						Y1 = (int)sprite.Attribute ("y"),
//						Height = (int)sprite.Attribute ("h"),
//						Width = (int)sprite.Attribute("w")
//					};
//				
//				_sprites = new Dictionary<string,Sce.PlayStation.HighLevel.GameEngine2D.Base.Vector2i>();
//				foreach(var curLine in lines)
//				{
//					_sprites.Add(curLine.Name,new Vector2i(((curLine.X1)/curLine.Width),4-(curLine.Y1/curLine.Height)));//row of spritesheet
//				}
//				
//				_texture = new Texture2D(AssetsPrefix + spriteSheetPic,false);//Texture2D
//				_textureInfo = new TextureInfo(_texture,new Vector2i(4,5));//determine by the row and column of spritesheet
//			}
//			
//			public void DisposeTextureNInfo()
//			{
//				_texture.Dispose();
//				_textureInfo.Dispose();
//			}
//			
//			public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile Get(int x, int y)
//			{
//				var spriteTile = new SpriteTile(_textureInfo);
//				spriteTile.TileIndex2D = new Vector2i(x,y);
//				spriteTile.Quad.S = new Sce.PlayStation.Core.Vector2(128,96);
//				return spriteTile;
//			}
//			
//			public Sce.PlayStation.HighLevel.GameEngine2D.SpriteTile Get(string name)
//			{
//				return Get (_sprites[name].X,_sprites[name].Y);
//			}	
		}
		
		
	}
}

