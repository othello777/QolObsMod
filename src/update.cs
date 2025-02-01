using System;
using Microsoft.Xna.Framework;

namespace DuckGame
{
	internal class update : IUpdateable
	{
		public bool Enabled
		{
			get
			{
				return true;
			}
		}

		public int UpdateOrder
		{
			get
			{
				return 1;
			}
		}

		public event EventHandler<EventArgs> EnabledChanged;

		public event EventHandler<EventArgs> UpdateOrderChanged;

		public void Update(GameTime gameTime)
		{
			alwaysupdater.Update();
		}
	}
}
