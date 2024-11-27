using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapPiece : EScriptable
{
	public static MapPiece Instance
	{
		get
		{
			MapPiece result;
			if ((result = MapPiece._Instance) == null)
			{
				result = (MapPiece._Instance = Resources.Load<MapPiece>("World/Map/MapPiece"));
			}
			return result;
		}
	}

	public static bool IsEditor
	{
		get
		{
			return EClass.debug.enableMapPieceEditor;
		}
	}

	public PartialMap GetMap(MapPiece.Type type, string tag, float ruin)
	{
		this.Init();
		MapPiece.Item item = this.items.RandomItemWeighted(delegate(MapPiece.Item a)
		{
			if (a.chance != 0f || !(type.ToString() == a.id))
			{
				return a.chance * (float)((type == MapPiece.Type.Any || type.ToString() == a.id) ? 1 : 0);
			}
			return 1f;
		});
		if (item.paths.Count == 0)
		{
			Debug.Log("no path");
			return null;
		}
		string[] array = tag.IsEmpty() ? null : tag.Split(',', StringSplitOptions.None);
		List<MapPiece.MapPath> list = new List<MapPiece.MapPath>();
		foreach (MapPiece.MapPath mapPath in item.paths)
		{
			if (mapPath.tag.IsEmpty() || (array != null && array.Contains(tag)))
			{
				list.Add(mapPath);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		string path = list.RandomItem<MapPiece.MapPath>().path;
		Debug.Log("Loading PartialMap:" + path);
		PartialMap partialMap = MapPiece.CacheMap.TryGetValue(path, null);
		if (partialMap == null)
		{
			partialMap = PartialMap.Load(path);
			MapPiece.CacheMap.Add(path, partialMap);
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
		if (MapPiece.initialized)
		{
			return;
		}
		foreach (MapPiece.Item item in this.items)
		{
			item.paths.Clear();
			foreach (string text in Directory.GetFiles(CorePath.MapPieceSave + item.id, "*", SearchOption.AllDirectories))
			{
				if (text.EndsWith("mp"))
				{
					DirectoryInfo directory = new FileInfo(text).Directory;
					string tag = (directory.Parent.Name != item.id) ? "" : directory.Name;
					item.paths.Add(new MapPiece.MapPath
					{
						path = text,
						tag = tag
					});
				}
			}
		}
		MapPiece.initialized = true;
	}

	public void Reset()
	{
		MapPiece.initialized = false;
	}

	private static MapPiece _Instance;

	public static bool initialized;

	public List<MapPiece.Item> items;

	public static Dictionary<string, PartialMap> CacheMap = new Dictionary<string, PartialMap>();

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

		[HideInInspector]
		[NonSerialized]
		public List<MapPiece.MapPath> paths = new List<MapPiece.MapPath>();
	}
}
