using System;
using Newtonsoft.Json;

public class ZonePreEnterDigStairs : ZonePreEnterEvent
{
	public override void Execute()
	{
		if (this.pos.HasBlock)
		{
			this.pos.SetBlock(0, 0);
		}
		if (this.pos.HasObj)
		{
			this.pos.SetObj(0, 1, 0);
		}
		Trait trait = null;
		foreach (Thing thing in this.pos.Things)
		{
			if (thing.trait is TraitStairsLocked)
			{
				trait = thing.trait;
				break;
			}
			TraitNewZone traitNewZone = thing.trait as TraitNewZone;
			if (traitNewZone != null && ((this.fromAbove && traitNewZone.IsUpstairs) || (!this.fromAbove && traitNewZone.IsDownstairs)))
			{
				trait = traitNewZone;
				break;
			}
		}
		if (trait == null)
		{
			if (EClass._zone.IsSkyLevel)
			{
				for (int i = this.pos.z - 1; i < this.pos.z + 2; i++)
				{
					for (int j = this.pos.x - 1; j < this.pos.x + 2; j++)
					{
						Point point = new Point(j, i);
						if (point.IsValid && point.sourceFloor.tileType == TileType.Sky)
						{
							EClass._map.SetFloor(point.x, point.z, EClass._zone.biome.interior.floor.mat, EClass._zone.biome.interior.floor.id);
						}
					}
				}
			}
			Thing thing2 = ThingGen.Create(EClass._zone.biome.style.GetIdStairs(this.fromAbove), -1, -1);
			EClass._zone.AddCard(thing2, this.pos.x, this.pos.z);
			thing2.c_uidZone = this.uidZone;
			thing2.SetPlaceState(PlaceState.installed, false);
			thing2.isPlayerCreation = true;
		}
	}

	[JsonProperty]
	public Point pos;

	[JsonProperty]
	public bool fromAbove;

	[JsonProperty]
	public int uidZone;
}
