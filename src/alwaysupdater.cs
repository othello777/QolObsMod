using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace DuckGame
{
	internal class alwaysupdater
	{
		public static void doLobbyStuff()
		{
			if (!ModLoader.modsEnabled || !(Level.current is TeamSelect2) || Steam.lobby == null || Steam.lobby.id == 0UL)
			{
                updateLobby = true;
				return;
			}
			string text;
			if (Level.current is TeamSelect2 && updateLobby && !string.IsNullOrEmpty(text = Steam.lobby.GetLobbyData("mods")))
			{
				int num = text.IndexOf(QolObsMod.replaceData);
				if (num < 0)
				{
                    updateLobby = false;
					return;
				}
				text = text.Remove(num, QolObsMod.replaceData.Length).Trim(new char[] { '|' }).Replace("||", "|");
				Steam.lobby.SetLobbyData("mods", text);
                updateLobby = false;
			}
		}

		public static void Update()
		{
            doLobbyStuff();
			if (startup)
			{
                startup = false;
                timer = new System.Threading.Timer(new TimerCallback(TimerUpdate), "Some state", TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0));
			}
			if (ShouldUpdateTextFile() && !writting)
			{
                writting = true;
                write();
                writting = false;
			}
		}

		public static void write()
		{
			File.WriteAllText(DuckFile.optionsDirectory + "/scores.txt", string.Empty);
			StringBuilder stringBuilder = new StringBuilder();

			for (int index = 0; index < DG.MaxPlayers; index++)
			{
				int playerNum = index + 1;
                stringBuilder.AppendLine("player" + playerNum.ToString() + "=" + scores[index].ToString());
                stringBuilder.AppendLine("name" + playerNum.ToString() + "=" + Names[index].CleanFormatting(Extensions.CleanMethod.Both));//CleanName(Names[index]));
            }
			stringBuilder.AppendLine("levelname=" + levelName);
			stringBuilder.AppendLine(TimerString);
			using (StreamWriter streamWriter = new StreamWriter(DuckFile.optionsDirectory + "/scores.txt", true))
			{
				streamWriter.Write(stringBuilder.ToString());
				streamWriter.Close();
			}
		}

		public static bool ShouldUpdateTextFile()
		{
			bool updatedField = false;
            for (int index = 0; index < DG.MaxPlayers; index++)
			{
				Profile p = Profiles.GetProfile(index);
                if (p != null)
                {
                    if (p.team != null && p.team.score != scores[index])
                    {
                        updatedField = true;
                        scores[index] = p.team.score;
                    }
                    if (Names[index] != p.name)
                    {
                        updatedField = true;
                        Names[index] = p.name;
                    }
                }
            }
			return updatedField;
		}

		public static void Reset()
		{
			File.WriteAllText(DuckFile.optionsDirectory + "/scores.txt", string.Empty);
			StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < DG.MaxPlayers; index++)
            {
                int playerNum = index + 1;
                stringBuilder.AppendLine("player" + playerNum.ToString() + "=0");
                stringBuilder.AppendLine("name" + playerNum.ToString() + "=player" + playerNum.ToString());
            }
			stringBuilder.AppendLine(TimerString);
			using (StreamWriter streamWriter = new StreamWriter(DuckFile.optionsDirectory + "/scores.txt", true))
			{
				streamWriter.Write(stringBuilder.ToString());
				streamWriter.Close();
			}
		}

		public static string CleanName(string s)
		{
			string text = "";
			s = s.Replace(" ", "").Replace("@", " ").Replace("|", " ");
			string[] array = s.Split(new char[] { ' ' });
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i].Replace(" ", "");
				if (sParseColor(text2) && sParseSprite(text2))
				{
					text += text2;
				}
			}
			text = text.Replace(" ", "");
			if (text == "")
			{
				return "no_name";
			}
			return WithMaxLength(text, 20);
		}

		public static bool sParseColor(string color)
		{
			return !(color == "RED") && !(color == "WHITE") && !(color == "BLACK") && !(color == "DARKNESS") && !(color == "BLUE") && !(color == "DGBLUE") && !(color == "DGRED") && !(color == "DGGREEN") && !(color == "DGYELLOW") && !(color == "DGORANGE") && !(color == "ORANGE") && !(color == "MENUORANGE") && !(color == "YELLOW") && !(color == "GREEN") && !(color == "LIME") && !(color == "GRAY") && !(color == "LIGHTGRAY") && !(color == "CREDITSGRAY") && !(color == "BLUEGRAY") && !(color == "PINK") && !(color == "PURPLE") && !(color == "DGPURPLE");
		}

		public static bool sParseSprite(string sprite)
		{
			return Input.GetTriggerSprite(sprite) == null;
		}

		public static string WithMaxLength(string value, int maxLength)
		{
			return value.Substring(0, Math.Min(value.Length, maxLength));
		}

		private static void TimerUpdate(object state)
		{
			if (Level.current != null && Level.current != Prevlevel)
			{
                string text = "";
				if (Level.current is GameLevel)
				{
					GameLevel gameLevel = Level.current as GameLevel;
					if (gameLevel.data != null && gameLevel.customLevel)
					{
						if (gameLevel.data.workshopData.name != null && gameLevel.data.workshopData.name != "")
						{
							text = gameLevel.data.workshopData.name;
						}
						else if (gameLevel.data.GetPath() != "" && gameLevel.data.GetPath() != null)
						{
							text = Path.GetFileNameWithoutExtension(gameLevel.data.GetPath());
						}
                        if (gameLevel.synchronizedLevelName != null)
                        {
                            text = gameLevel.synchronizedLevelName;
                        }
                        if (text == "")
						{
							text = "CUSTOM LEVEL";
						}
						//text = text.Replace("[QC] ", "");
					}
				}
				if (Level.current is TeamSelect2)
				{
					text = "Lobby";
				}
				else if (Level.current is RockScoreboard)
				{
					text = "Rock Throw";
				}
				if (text == "")
				{
					text = "Level Name";
				}
                levelName = text;
                Prevlevel = Level.current;
                TimerString = "timer=00:00";
                Seconds = 0;
                Mins = 0;
			}
            Seconds++;
			if (Seconds >= 60)
			{
                Seconds = 0;
                Mins++;
			}
			string text2 = Seconds.ToString();
			string MinsString = Mins.ToString();
			if (Seconds < 10)
			{
				text2 = "0" + text2;
			}
			if (Mins < 10)
			{
				MinsString = "0" + MinsString;
			}
            TimerString = "timer=" + MinsString.ToString() + ":" + text2.ToString();
            ShouldUpdateTextFile();
			if (!writting)
			{
                writting = true;
                write();
                writting = false;
			}
		}

		private static bool updateLobby;

		public static List<int> scores = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 };

		public static List<string> Names = new List<string> { "player1", "player2", "player3", "player4", "player5", "player6", "player7", "player8" };

		private static bool startup = true;

		private static System.Threading.Timer timer;

		private static Level Prevlevel;

		private static int Seconds = 0;

		private static int Mins = 0;

		private static string TimerString = "timer=00:00";

		private static bool writting = false;

		private static string levelName;
	}
}
