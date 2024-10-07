using System.Collections.Generic;
using UnityEngine;

namespace Funzilla
{
	internal class LevelManager
	{
		private static List<string> _levels;

		internal static List<string> Levels
		{
			get
			{
				if (_levels == null)
				{
					LoadLevels();
				}
				return _levels;
			}
		}

		internal static void LoadLevels()
		{
			LoadCsv("Levels");
		}

		private static void LoadCsv(string csvFile)
		{
			var csv = Resources.Load<TextAsset>(csvFile).text;
			var columns = new Dictionary<string, int>();
			_levels = new List<string>(100);
			CSVReader.LoadFromString(csv,
				(lineIndex, content) =>
				{
					if (lineIndex == 0)
					{
						for (var i = 0; i < content.Count; i++)
						{
							if (!string.IsNullOrEmpty(content[i]))
								columns.Add(content[i], i);
						}
					}
					else
					{
						_levels.Add(content[columns["Level"]]);
					}
				});
		}
	}
}