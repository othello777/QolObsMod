using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace DuckGame
{
	public class QolObsMod : DisabledMod
	{
		public override Priority priority
		{
			get
			{
				return base.priority;
			}
		}

		public static string replaceData
		{
			get
			{
				string text;
				if (!config.isWorkshop)
				{
					text = "LOCAL";
				}
				else
				{
					text = steamIdField.GetValue(config, new object[0]).ToString();
				}
				return text;
			}
		}

		public static bool disabled
		{
			get
			{
				return (bool)disabledField.GetValue(config, new object[0]);
			}
			set
			{
                disabledField.SetValue(config, value, new object[0]);
			}
		}

		protected override void OnPreInitialize()
		{
			alwaysupdater.Reset();
            config = configuration;
			base.OnPreInitialize();
		}

		protected override void OnPostInitialize()
		{
			//((Form)Control.FromHandle(MonoMain.instance.Window.Handle)).FormClosing += new FormClosingEventHandler(this.FormClosed);
			(typeof(Game).GetField("updateableComponents", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).GetValue(MonoMain.instance) as List<IUpdateable>).Add(new update());
		}

		private void FormClosed(object sender, EventArgs e)
		{
			if (Program.commandLine.Contains("-download"))
			{
                disabled = false;
				typeof(ModLoader).GetMethod("DisabledModsChanged", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[0]);
			}
		}

		public QolObsMod()
		{
		}

		public static ModConfiguration config;

		private static PropertyInfo steamIdField = typeof(ModConfiguration).GetProperty("workshopID", BindingFlags.Instance | BindingFlags.NonPublic);

		private static PropertyInfo disabledField = typeof(ModConfiguration).GetProperty("disabled", BindingFlags.Instance | BindingFlags.NonPublic);

	}
}
