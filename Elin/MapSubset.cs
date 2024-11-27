using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class MapSubset : EClass
{
	public static bool Exist(string id)
	{
		return File.Exists(string.Concat(new string[]
		{
			CorePath.ZoneSave,
			EClass._zone.idExport,
			"_",
			id,
			".s"
		}));
	}

	public static void Save(string id)
	{
		if (EClass._zone.subset == null)
		{
			EClass._zone.subset = new MapSubset();
		}
		EClass._zone.subset.OnSave(id);
		EClass._zone.idCurrentSubset = id;
		GameIO.SaveFile(string.Concat(new string[]
		{
			CorePath.ZoneSave,
			EClass._zone.idExport,
			"_",
			id,
			".s"
		}), EClass._zone.subset);
	}

	public static MapSubset Load(string id)
	{
		return GameIO.LoadFile<MapSubset>(string.Concat(new string[]
		{
			CorePath.ZoneSave,
			EClass._zone.idExport,
			"_",
			id,
			".s"
		}));
	}

	public void OnSave(string _id)
	{
		this.id = _id;
		this.listClear.Clear();
		EClass._map.ForeachCell(delegate(Cell c)
		{
			if (c.isClearArea)
			{
				this.listClear.Add(c.index);
			}
		});
		this.serializedCards.cards.Clear();
		foreach (Card card in EClass._map.Cards)
		{
			if (card.isSubsetCard)
			{
				this.serializedCards.Add(card);
			}
		}
		Debug.Log(this.listClear.Count);
	}

	public void Apply()
	{
		Debug.Log(this.listClear.Count);
		foreach (int index in this.listClear)
		{
			Cell cell = EClass._map.GetCell(index);
			cell.isClearArea = true;
			cell.Things.ForeachReverse(delegate(Thing t)
			{
				t.Destroy();
			});
		}
		this.serializedCards.Restore(EClass._map, null, true, null);
	}

	[JsonProperty]
	public string id;

	[JsonProperty]
	public List<int> listClear = new List<int>();

	[JsonProperty]
	public SerializedCards serializedCards = new SerializedCards();
}
