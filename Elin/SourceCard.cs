using System;
using System.Collections.Generic;
using UnityEngine;

public class SourceCard : EClass
{
	public void Init()
	{
		foreach (SourceThing.Row row in EClass.sources.things.rows)
		{
			this.AddRow(row, false);
		}
		foreach (SourceChara.Row row2 in EClass.sources.charas.rows)
		{
			this.AddRow(row2, true);
		}
		SourceChara.rowDefaultPCC = EClass.sources.charas.map["chara"];
	}

	public TraitCrafter GetModelCrafter(string id)
	{
		return this.crafters.GetOrCreate(id, () => (this.map[id].model.trait as TraitCrafter) ?? Trait.SelfFactory);
	}

	public void AddRow(CardRow row, bool isChara = false)
	{
		if (row.id.IsEmpty())
		{
			Debug.Log("assf");
			return;
		}
		row.isChara = isChara;
		row.elementMap = Element.GetElementMap(row.elements);
		if (row.isChara)
		{
			(row as SourceChara.Row).tileType = TileType.Obj;
		}
		else
		{
			SourceThing.Row row2 = row as SourceThing.Row;
			if (row2._tileType.IsEmpty())
			{
				row2.tileType = TileType.Obj;
			}
			else
			{
				row2.tileType = TileType.dict[row2._tileType];
			}
		}
		if (!row._origin.IsEmpty())
		{
			row.origin = EClass.sources.things.map[row._origin];
			if (!this.firstVariations.ContainsKey(row._origin))
			{
				this.firstVariations[row._origin] = row;
			}
			row.origin.isOrigin = true;
		}
		row.SetRenderData();
		this.rows.Add(row);
		this.map[row.id] = row;
	}

	public List<CardRow> rows = new List<CardRow>();

	public Dictionary<string, CardRow> map = new Dictionary<string, CardRow>();

	public Dictionary<string, CardRow> firstVariations = new Dictionary<string, CardRow>();

	public Dictionary<string, TraitCrafter> crafters = new Dictionary<string, TraitCrafter>();
}
