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
	public class Hits
	{
		public string ID;
		public int Xpos, Ypos;
		public float starting_X_pos;
		
		
		public Hits(string id, int x, int y)
		{
			ID = id;
			Xpos = x;
			Ypos = y;
		}		
	}
}

