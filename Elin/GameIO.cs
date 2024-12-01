using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class GameIO : EClass
{
	public static JsonSerializerSettings jsReadGame = new JsonSerializerSettings
	{
		NullValueHandling = NullValueHandling.Ignore,
		DefaultValueHandling = DefaultValueHandling.Ignore,
		PreserveReferencesHandling = PreserveReferencesHandling.Objects,
		TypeNameHandling = TypeNameHandling.Auto,
		Error = IO.OnError
	};

	public static JsonSerializerSettings jsWriteGame = new JsonSerializerSettings
	{
		NullValueHandling = NullValueHandling.Ignore,
		DefaultValueHandling = DefaultValueHandling.Ignore,
		PreserveReferencesHandling = PreserveReferencesHandling.Objects,
		TypeNameHandling = TypeNameHandling.Auto,
		ContractResolver = ShouldSerializeContractResolver.Instance,
		Error = IO.OnError
	};

	public static Formatting formatting = Formatting.Indented;

	public static string pathBackup => CorePath.RootSave + "Backup/";

	public static string pathSaveRoot => CorePath.RootSave;

	public static string pathCurrentSave => pathSaveRoot + Game.id + "/";

	public static string pathTemp => pathCurrentSave + "Temp/";

	public static int NumBackup => (int)MathF.Max(5f, EClass.core.config.game.numBackup);

	public static bool compressSave
	{
		get
		{
			if (EClass.core.config.compressSave)
			{
				return !EClass.debug.dontCompressSave;
			}
			return false;
		}
	}

	public static void ResetTemp()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(pathTemp);
		if (directoryInfo.Exists)
		{
			directoryInfo.Delete(recursive: true);
		}
		IO.CreateDirectory(pathTemp);
	}

	public static void ClearTemp()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(pathTemp);
		if (directoryInfo.Exists)
		{
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			for (int i = 0; i < directories.Length; i++)
			{
				directories[i].Delete(recursive: true);
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
		string text = JsonConvert.SerializeObject(EClass.core.game, formatting, jsWriteGame);
		string path = pathCurrentSave + "game.txt";
		GameIndex gameIndex = new GameIndex().Create(EClass.core.game);
		gameIndex.id = Game.id;
		IO.SaveFile(pathCurrentSave + "index.txt", gameIndex);
		if (compressSave)
		{
			IO.Compress(path, text);
		}
		else
		{
			File.WriteAllText(path, text);
		}
		DirectoryInfo[] directories = new DirectoryInfo(pathCurrentSave).GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			if (int.TryParse(directoryInfo.Name, out var result) && !EClass.game.spatials.map.ContainsKey(result))
			{
				IO.DeleteDirectory(directoryInfo.FullName);
				Debug.Log("Deleting unused map:" + directoryInfo.FullName);
			}
		}
		ClearTemp();
		return gameIndex;
	}

	public static void MakeBackup(GameIndex index, string suffix = "")
	{
		Debug.Log("Start backup:" + index.id);
		string id = index.id;
		IO.CreateDirectory(pathBackup);
		string text = pathBackup + id;
		IO.CreateDirectory(text);
		Debug.Log(text);
		List<DirectoryInfo> dirs = new DirectoryInfo(text).GetDirectories().ToList();
		dirs.ForeachReverse(delegate(DirectoryInfo i)
		{
			if (!int.TryParse(i.Name, out var _))
			{
				dirs.Remove(i);
			}
		});
		dirs.Sort((DirectoryInfo a, DirectoryInfo b) => int.Parse(a.Name) - int.Parse(b.Name));
		int count = dirs.Count;
		Debug.Log("Deleting excess backup:" + dirs.Count + "/" + NumBackup);
		if (count > NumBackup)
		{
			for (int j = 0; j < count - NumBackup; j++)
			{
				IO.DeleteDirectory(dirs[j].FullName);
			}
		}
		Debug.Log("Copying backup:");
		string newId = GetNewId(text + "/", "", (dirs.Count == 0) ? 1 : int.Parse(dirs.LastItem().Name));
		IO.CopyDir(pathSaveRoot + id + "/", text + "/" + newId, (string s) => s == "Temp");
	}

	public static Game LoadGame(string id, string root)
	{
		Game.id = id;
		ClearTemp();
		string path = root + "/game.txt";
		return JsonConvert.DeserializeObject<Game>(IO.IsCompressed(path) ? IO.Decompress(path) : File.ReadAllText(path), jsReadGame);
	}

	public static void UpdateGameIndex(GameIndex i)
	{
		i.madeBackup = true;
		IO.SaveFile(i.path + "/index.txt", i);
	}

	public static void SaveFile(string path, object obj)
	{
		IO.SaveFile(path, obj, compressSave, jsWriteGame);
	}

	public static T LoadFile<T>(string path) where T : new()
	{
		return IO.LoadFile<T>(path, compressSave, jsReadGame);
	}

	public static bool FileExist(string id)
	{
		return File.Exists(pathSaveRoot + Game.id + "/" + id + ".txt");
	}

	public static void DeleteGame(string id, bool deleteBackup = true)
	{
		if (!Directory.Exists(pathSaveRoot + id))
		{
			return;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(pathSaveRoot + id);
		if (directoryInfo.Exists)
		{
			directoryInfo.Delete(recursive: true);
		}
		if (deleteBackup)
		{
			directoryInfo = new DirectoryInfo(pathBackup + id);
			if (directoryInfo.Exists)
			{
				directoryInfo.Delete(recursive: true);
			}
		}
	}

	public static void MakeDirectories(string id)
	{
		if (!Directory.Exists(pathSaveRoot + id))
		{
			Directory.CreateDirectory(pathSaveRoot + id);
		}
		if (!Directory.Exists(pathSaveRoot + id + "/Temp"))
		{
			Directory.CreateDirectory(pathSaveRoot + id + "/Temp");
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
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		foreach (DirectoryInfo directoryInfo2 in directories)
		{
			if (File.Exists(directoryInfo2?.ToString() + "/index.txt"))
			{
				try
				{
					GameIndex gameIndex = IO.LoadFile<GameIndex>(directoryInfo2?.ToString() + "/index.txt");
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
				int.TryParse(a.id, out var result);
				int.TryParse(b.id, out var result2);
				return result2 - result;
			});
		}
		else
		{
			list.Sort((GameIndex a, GameIndex b) => b.real.GetRawReal() - a.real.GetRawReal());
		}
		return list;
	}

	public static void DeleteEmptyGameFolders()
	{
		DirectoryInfo[] directories = new DirectoryInfo(pathSaveRoot).GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			if (directoryInfo.Name != "Backup" && !File.Exists(directoryInfo?.ToString() + "/game.txt"))
			{
				directoryInfo.Delete(recursive: true);
			}
		}
	}

	public static string GetNewId(string path, string prefix = "", int start = 1)
	{
		string text = "";
		for (int i = start; i < 999999; i++)
		{
			text = prefix + i;
			if (!Directory.Exists(path + text))
			{
				break;
			}
		}
		return text;
	}
}
