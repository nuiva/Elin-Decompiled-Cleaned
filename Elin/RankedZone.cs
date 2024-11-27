using System;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine;

public class RankedZone : EClass
{
	public int Value
	{
		get
		{
			return this.value;
		}
	}

	public string Name
	{
		get
		{
			return this.z.Name;
		}
	}

	public string GetFactionName()
	{
		if (this.z.IsPCFaction)
		{
			return EClass.pc.faction.Name;
		}
		if (!this.z.source.faction.IsEmpty())
		{
			return EClass.sources.factions.map[this.z.source.faction].GetName();
		}
		return " - ";
	}

	public Sprite GetSprite()
	{
		return TilemapUtils.GetOrCreateTileSprite(EClass.scene.elomap.actor.tileset, this.z.icon, 0f);
	}

	public Zone z;

	public int rank;

	public int value;
}
