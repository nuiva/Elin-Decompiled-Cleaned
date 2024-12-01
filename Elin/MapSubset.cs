using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class MapSubset : EClass
{
	[JsonProperty]
	public string id;

	[JsonProperty]
	public List<int> listClear = new List<int>();

	[JsonProperty]
	public SerializedCards serializedCards = new SerializedCards();

	public static bool Exist(string id)
	{
		return File.Exists(CorePath.ZoneSave + EClass._zone.idExport + "_" + id + ".s");
	}

	public static void Save(string id)
	{
		if (EClass._zone.subset == null)
		{
			EClass._zone.subset = new MapSubset();
		}
		EClass._zone.subset.OnSave(id);
		EClass._zone.idCurrentSubset = id;
		GameIO.SaveFile(CorePath.ZoneSave + EClass._zone.idExport + "_" + id + ".s", EClass._zone.subset);
	}

	public static MapSubset Load(string id)
	{
		return GameIO.LoadFile<MapSubset>(CorePath.ZoneSave + EClass._zone.idExport + "_" + id + ".s");
	}

	public void OnSave(string _id)
	{
		id = _id;
		listClear.Clear();
		EClass._map.ForeachCell(delegate(Cell c)
		{
			if (c.isClearArea)
			{
				listClear.Add(c.index);
			}
		});
		serializedCards.cards.Clear();
		foreach (Card card in EClass._map.Cards)
		{
			if (card.isSubsetCard)
			{
				serializedCards.Add(card);
			}
		}
		Debug.Log(listClear.Count);
	}

	public void Apply()
	{
		Debug.Log(listClear.Count);
		foreach (int item in listClear)
		{
			Cell cell = EClass._map.GetCell(item);
			cell.isClearArea = true;
			cell.Things.ForeachReverse(delegate(Thing t)
			{
				t.Destroy();
			});
		}
		serializedCards.Restore(EClass._map, null, addToZone: true);
	}
}
