using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapPiece : EScriptable
{
	public class MapPath
	{
		public string path;

		public string tag;
	}

	public enum Type
	{
		Any,
		Deco,
		Treasure,
		Trap,
		Concert,
		Farm
	}

	[Serializable]
	public class Item
	{
		public string id;

		public float chance;

		[NonSerialized]
		[HideInInspector]
		public List<MapPath> paths = new List<MapPath>();
	}

	private static MapPiece _Instance;

	public static bool initialized;

	public List<Item> items;

	public static Dictionary<string, PartialMap> CacheMap = new Dictionary<string, PartialMap>();

	public static MapPiece Instance => _Instance ?? (_Instance = Resources.Load<MapPiece>("World/Map/MapPiece"));

	public static bool IsEditor => EClass.debug.enableMapPieceEditor;

	public PartialMap GetMap(Type type, string tag, float ruin)
	{
		Init();
		Item item = items.RandomItemWeighted((Item a) => (a.chance != 0f || !(type.ToString() == a.id)) ? (a.chance * (float)((type == Type.Any || type.ToString() == a.id) ? 1 : 0)) : 1f);
		if (item.paths.Count == 0)
		{
			Debug.Log("no path");
			return null;
		}
		string[] array = (tag.IsEmpty() ? null : tag.Split(','));
		List<MapPath> list = new List<MapPath>();
		foreach (MapPath path2 in item.paths)
		{
			if (path2.tag.IsEmpty() || (array != null && array.Contains(tag)))
			{
				list.Add(path2);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		string path = list.RandomItem().path;
		Debug.Log("Loading PartialMap:" + path);
		PartialMap partialMap = CacheMap.TryGetValue(path);
		if (partialMap == null)
		{
			partialMap = PartialMap.Load(path);
			CacheMap.Add(path, partialMap);
		}
		if (partialMap.allowRotate)
		{
			partialMap.dir = EScriptable.rnd(4);
		}
		partialMap.procedural = true;
		partialMap.ruinChance = ruin;
		return partialMap;
	}

	public void Init()
	{
		if (initialized)
		{
			return;
		}
		foreach (Item item in items)
		{
			item.paths.Clear();
			string[] files = Directory.GetFiles(CorePath.MapPieceSave + item.id, "*", SearchOption.AllDirectories);
			foreach (string text in files)
			{
				if (text.EndsWith("mp"))
				{
					DirectoryInfo directory = new FileInfo(text).Directory;
					string tag = ((directory.Parent.Name != item.id) ? "" : directory.Name);
					item.paths.Add(new MapPath
					{
						path = text,
						tag = tag
					});
				}
			}
		}
		initialized = true;
	}

	public void Reset()
	{
		initialized = false;
	}
}
