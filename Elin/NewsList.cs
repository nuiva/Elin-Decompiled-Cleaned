using System.Collections.Generic;
using System.IO;

public class NewsList : EClass
{
	public class Item : EClass
	{
		public string title;

		public string content;

		public List<string> listImageId = new List<string>();
	}

	public static Dictionary<string, List<Item>> dict;

	public static List<Item> listAll = new List<Item>();

	public static void Init()
	{
		if (dict == null)
		{
			dict = new Dictionary<string, List<Item>>();
			listAll.Clear();
			AddDir(CorePath.CorePackage.News);
		}
		static void AddDir(string path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			Item item = null;
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				if (!(fileInfo.Extension != ".txt"))
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
					List<Item> list = dict.TryGetValue(fileNameWithoutExtension);
					StreamReader streamReader = new StreamReader(fileInfo.FullName);
					string text = null;
					while (!streamReader.EndOfStream)
					{
						string text2 = streamReader.ReadLine();
						if (!string.IsNullOrEmpty(text2))
						{
							if (text2.StartsWith('@') && item != null)
							{
								string[] array = text2.TrimStart('@').Split(',');
								foreach (string item2 in array)
								{
									item.listImageId.Add(item2);
								}
							}
							else if (text == null)
							{
								text = text2;
							}
							else
							{
								item = new Item
								{
									title = text,
									content = text2
								};
								if (list == null)
								{
									list = new List<Item>();
									dict.Add(fileNameWithoutExtension, list);
								}
								list.Add(item);
								listAll.Add(item);
								text = null;
							}
						}
					}
				}
			}
		}
	}

	public static List<Item> GetNews(int seed)
	{
		Init();
		Rand.SetSeed(seed);
		List<Item> list = new List<Item>();
		for (int i = 0; i < 1000; i++)
		{
			Item item = listAll.RandomItem();
			if (!list.Contains(item))
			{
				list.Add(item);
				if (list.Count >= 3)
				{
					break;
				}
			}
		}
		Rand.SetSeed();
		return list;
	}
}
