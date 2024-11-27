using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

public class NewsList : EClass
{
	public static void Init()
	{
		if (NewsList.dict != null)
		{
			return;
		}
		NewsList.dict = new Dictionary<string, List<NewsList.Item>>();
		NewsList.listAll.Clear();
		NewsList.<Init>g__AddDir|3_0(CorePath.CorePackage.News);
	}

	public static List<NewsList.Item> GetNews(int seed)
	{
		NewsList.Init();
		Rand.SetSeed(seed);
		List<NewsList.Item> list = new List<NewsList.Item>();
		for (int i = 0; i < 1000; i++)
		{
			NewsList.Item item = NewsList.listAll.RandomItem<NewsList.Item>();
			if (!list.Contains(item))
			{
				list.Add(item);
				if (list.Count >= 3)
				{
					break;
				}
			}
		}
		Rand.SetSeed(-1);
		return list;
	}

	[CompilerGenerated]
	internal static void <Init>g__AddDir|3_0(string path)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		NewsList.Item item = null;
		foreach (FileInfo fileInfo in directoryInfo.GetFiles())
		{
			if (!(fileInfo.Extension != ".txt"))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
				List<NewsList.Item> list = NewsList.dict.TryGetValue(fileNameWithoutExtension, null);
				StreamReader streamReader = new StreamReader(fileInfo.FullName);
				string text = null;
				while (!streamReader.EndOfStream)
				{
					string text2 = streamReader.ReadLine();
					if (!string.IsNullOrEmpty(text2))
					{
						if (text2.StartsWith('@') && item != null)
						{
							foreach (string item2 in text2.TrimStart('@').Split(',', StringSplitOptions.None))
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
							item = new NewsList.Item
							{
								title = text,
								content = text2
							};
							if (list == null)
							{
								list = new List<NewsList.Item>();
								NewsList.dict.Add(fileNameWithoutExtension, list);
							}
							list.Add(item);
							NewsList.listAll.Add(item);
							text = null;
						}
					}
				}
			}
		}
	}

	public static Dictionary<string, List<NewsList.Item>> dict;

	public static List<NewsList.Item> listAll = new List<NewsList.Item>();

	public class Item : EClass
	{
		public string title;

		public string content;

		public List<string> listImageId = new List<string>();
	}
}
