using CreativeSpore.SuperTilemapEditor;
using UnityEngine;

public class RankedZone : EClass
{
	public Zone z;

	public int rank;

	public int value;

	public int Value => value;

	public string Name => z.Name;

	public string GetFactionName()
	{
		if (z.IsPCFaction)
		{
			return EClass.pc.faction.Name;
		}
		if (!z.source.faction.IsEmpty())
		{
			return EClass.sources.factions.map[z.source.faction].GetName();
		}
		return " - ";
	}

	public Sprite GetSprite()
	{
		return TilemapUtils.GetOrCreateTileSprite(EClass.scene.elomap.actor.tileset, z.icon);
	}
}
