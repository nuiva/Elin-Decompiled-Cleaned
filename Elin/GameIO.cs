using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class GameIO : EClass
{
	public static string pathBackup
	{
		get
		{
			return CorePath.RootSave + "Backup/";
		}
	}

	public static string pathSaveRoot
	{
		get
		{
			return CorePath.RootSave;
		}
	}

	public static string pathCurrentSave
	{
		get
		{
			return GameIO.pathSaveRoot + Game.id + "/";
		}
	}

	public static string pathTemp
	{
		get
		{
			return GameIO.pathCurrentSave + "Temp/";
		}
	}

	public static int NumBackup
	{
		get
		{
			return (int)MathF.Max(5f, (float)EClass.core.config.game.numBackup);
		}
	}

	public static bool compressSave
	{
		get
		{
			return EClass.core.config.compressSave && !EClass.debug.dontCompressSave;
		}
	}

	public static void ResetTemp()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(GameIO.pathTemp);
		if (directoryInfo.Exists)
		{
			directoryInfo.Delete(true);
		}
		IO.CreateDirectory(GameIO.pathTemp);
	}

	public static void ClearTemp()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(GameIO.pathTemp);
		if (directoryInfo.Exists)
		{
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			for (int i = 0; i < directories.Length; i++)
			{
				directories[i].Delete(true);
			}
			FileInfo[] files = directoryInfo.GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				files[i].Delete();
			}
		}
	}

	public static GameIndex SaveGame()
	{
		string text = JsonConvert.SerializeObject(EClass.core.game, GameIO.formatting, GameIO.jsWriteGame);
		string path = GameIO.pathCurrentSave + "game.txt";
		GameIndex gameIndex = new GameIndex().Create(EClass.core.game);
		gameIndex.id = Game.id;
		IO.SaveFile(GameIO.pathCurrentSave + "index.txt", gameIndex, false, null);
		if (GameIO.compressSave)
		{
			IO.Compress(path, text);
		}
		else
		{
			File.WriteAllText(path, text);
		}
		foreach (DirectoryInfo directoryInfo in new DirectoryInfo(GameIO.pathCurrentSave).GetDirectories())
		{
			int key;
			if (int.TryParse(directoryInfo.Name, out key) && !EClass.game.spatials.map.ContainsKey(key))
			{
				IO.DeleteDirectory(directoryInfo.FullName);
				Debug.Log("Deleting unused map:" + directoryInfo.FullName);
			}
		}
		GameIO.ClearTemp();
		return gameIndex;
	}

	public static void MakeBackup(GameIndex index, string suffix = "")
	{
		Debug.Log("Start backup:" + index.id);
		string id = index.id;
		IO.CreateDirectory(GameIO.pathBackup);
		string text = GameIO.pathBackup + id;
		IO.CreateDirectory(text);
		Debug.Log(text);
		List<DirectoryInfo> dirs = new DirectoryInfo(text).GetDirectories().ToList<DirectoryInfo>();
		dirs.ForeachReverse(delegate(DirectoryInfo i)
		{
			int num;
			if (!int.TryParse(i.Name, out num))
			{
				dirs.Remove(i);
			}
		});
		dirs.Sort((DirectoryInfo a, DirectoryInfo b) => int.Parse(a.Name) - int.Parse(b.Name));
		int count = dirs.Count;
		Debug.Log("Deleting excess backup:" + dirs.Count.ToString() + "/" + GameIO.NumBackup.ToString());
		if (count > GameIO.NumBackup)
		{
			for (int j = 0; j < count - GameIO.NumBackup; j++)
			{
				IO.DeleteDirectory(dirs[j].FullName);
			}
		}
		Debug.Log("Copying backup:");
		string newId = GameIO.GetNewId(text + "/", "", (dirs.Count == 0) ? 1 : int.Parse(dirs.LastItem<DirectoryInfo>().Name));
		IO.CopyDir(GameIO.pathSaveRoot + id + "/", text + "/" + newId, (string s) => s == "Temp");
	}

	public static Game LoadGame(string id, string root)
	{
		Game.id = id;
		GameIO.ClearTemp();
		string path = root + "/game.txt";
		return JsonConvert.DeserializeObject<Game>(IO.IsCompressed(path) ? IO.Decompress(path) : File.ReadAllText(path), GameIO.jsReadGame);
	}

	public static void UpdateGameIndex(GameIndex i)
	{
		i.madeBackup = true;
		IO.SaveFile(i.path + "/index.txt", i, false, null);
	}

	public static void SaveFile(string path, object obj)
	{
		IO.SaveFile(path, obj, GameIO.compressSave, GameIO.jsWriteGame);
	}

	public static T LoadFile<T>(string path) where T : new()
	{
		return IO.LoadFile<T>(path, GameIO.compressSave, GameIO.jsReadGame);
	}

	public static bool FileExist(string id)
	{
		return File.Exists(string.Concat(new string[]
		{
			GameIO.pathSaveRoot,
			Game.id,
			"/",
			id,
			".txt"
		}));
	}

	public static void DeleteGame(string id, bool deleteBackup = true)
	{
		if (!Directory.Exists(GameIO.pathSaveRoot + id))
		{
			return;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(GameIO.pathSaveRoot + id);
		if (directoryInfo.Exists)
		{
			directoryInfo.Delete(true);
		}
		if (deleteBackup)
		{
			directoryInfo = new DirectoryInfo(GameIO.pathBackup + id);
			if (directoryInfo.Exists)
			{
				directoryInfo.Delete(true);
			}
		}
	}

	public static void MakeDirectories(string id)
	{
		if (!Directory.Exists(GameIO.pathSaveRoot + id))
		{
			Directory.CreateDirectory(GameIO.pathSaveRoot + id);
		}
		if (!Directory.Exists(GameIO.pathSaveRoot + id + "/Temp"))
		{
			Directory.CreateDirectory(GameIO.pathSaveRoot + id + "/Temp");
		}
	}

	public static List<GameIndex> GetGameList(string path, bool sortByName = false)
	{
		List<GameIndex> list = new List<GameIndex>();
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		if (!directoryInfo.Exists)
		{
			return list;
		}
		foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
		{
			DirectoryInfo directoryInfo3 = directoryInfo2;
			if (File.Exists(((directoryInfo3 != null) ? directoryInfo3.ToString() : null) + "/index.txt"))
			{
				try
				{
					DirectoryInfo directoryInfo4 = directoryInfo2;
					GameIndex gameIndex = IO.LoadFile<GameIndex>(((directoryInfo4 != null) ? directoryInfo4.ToString() : null) + "/index.txt", false, null);
					gameIndex.id = directoryInfo2.Name;
					gameIndex.path = directoryInfo2.FullName;
					list.Add(gameIndex);
				}
				catch (Exception)
				{
				}
			}
		}
		if (sortByName)
		{
			list.Sort(delegate(GameIndex a, GameIndex b)
			{
				int num;
				int.TryParse(a.id, out num);
				int num2;
				int.TryParse(b.id, out num2);
				return num2 - num;
			});
		}
		else
		{
			list.Sort((GameIndex a, GameIndex b) => b.real.GetRawReal(0) - a.real.GetRawReal(0));
		}
		return list;
	}

	public static void DeleteEmptyGameFolders()
	{
		foreach (DirectoryInfo directoryInfo in new DirectoryInfo(GameIO.pathSaveRoot).GetDirectories())
		{
			if (directoryInfo.Name != "Backup")
			{
				DirectoryInfo directoryInfo2 = directoryInfo;
				if (!File.Exists(((directoryInfo2 != null) ? directoryInfo2.ToString() : null) + "/game.txt"))
				{
					directoryInfo.Delete(true);
				}
			}
		}
	}

	public static string GetNewId(string path, string prefix = "", int start = 1)
	{
		string text = "";
		for (int i = start; i < 999999; i++)
		{
			text = prefix + i.ToString();
			if (!Directory.Exists(path + text))
			{
				break;
			}
		}
		return text;
	}

	public static JsonSerializerSettings jsReadGame = new JsonSerializerSettings
	{
		NullValueHandling = NullValueHandling.Ignore,
		DefaultValueHandling = DefaultValueHandling.Ignore,
		PreserveReferencesHandling = PreserveReferencesHandling.Objects,
		TypeNameHandling = TypeNameHandling.Auto,
		Error = new EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>(IO.OnError)
	};

	public static JsonSerializerSettings jsWriteGame = new JsonSerializerSettings
	{
		NullValueHandling = NullValueHandling.Ignore,
		DefaultValueHandling = DefaultValueHandling.Ignore,
		PreserveReferencesHandling = PreserveReferencesHandling.Objects,
		TypeNameHandling = TypeNameHandling.Auto,
		ContractResolver = ShouldSerializeContractResolver.Instance,
		Error = new EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>(IO.OnError)
	};

	public static Formatting formatting = Formatting.Indented;
}
