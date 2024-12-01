using Newtonsoft.Json;

public class ZonePreEnterDigStairs : ZonePreEnterEvent
{
	[JsonProperty]
	public Point pos;

	[JsonProperty]
	public bool fromAbove;

	[JsonProperty]
	public int uidZone;

	public override void Execute()
	{
		if (pos.HasBlock)
		{
			pos.SetBlock();
		}
		if (pos.HasObj)
		{
			pos.SetObj();
		}
		Trait trait = null;
		foreach (Thing thing2 in pos.Things)
		{
			if (thing2.trait is TraitStairsLocked)
			{
				trait = thing2.trait;
				break;
			}
			if (thing2.trait is TraitNewZone traitNewZone && ((fromAbove && traitNewZone.IsUpstairs) || (!fromAbove && traitNewZone.IsDownstairs)))
			{
				trait = traitNewZone;
				break;
			}
		}
		if (trait != null)
		{
			return;
		}
		if (EClass._zone.IsSkyLevel)
		{
			for (int i = pos.z - 1; i < pos.z + 2; i++)
			{
				for (int j = pos.x - 1; j < pos.x + 2; j++)
				{
					Point point = new Point(j, i);
					if (point.IsValid && point.sourceFloor.tileType == TileType.Sky)
					{
						EClass._map.SetFloor(point.x, point.z, EClass._zone.biome.interior.floor.mat, EClass._zone.biome.interior.floor.id);
					}
				}
			}
		}
		Thing thing = ThingGen.Create(EClass._zone.biome.style.GetIdStairs(fromAbove));
		EClass._zone.AddCard(thing, pos.x, pos.z);
		thing.c_uidZone = uidZone;
		thing.SetPlaceState(PlaceState.installed);
		thing.isPlayerCreation = true;
	}
}
