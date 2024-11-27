using System;

public class SpatialGen : EClass
{
	public static Spatial CreateRecursive(string id, Spatial parent = null)
	{
		Spatial spatial = SpatialGen.Create(id, parent, true, -99999, -99999, 0);
		foreach (SourceZone.Row row in EClass.sources.zones.rows)
		{
			if ((!row.tag.Contains("debug") || EClass.debug.enable) && row.parent == id)
			{
				SpatialGen.CreateRecursive(row.id, spatial);
			}
		}
		return spatial;
	}

	public static Spatial Create(string id, Spatial parent, bool register, int x = -99999, int y = -99999, int icon = 0)
	{
		SourceZone.Row row = EClass.sources.zones.map[id];
		Spatial spatial = ClassCache.Create<Spatial>(row.type, "Elin");
		if (x == -99999)
		{
			x = ((row.pos.Length != 0) ? row.pos[0] : -1000);
			y = ((row.pos.Length != 0) ? row.pos[1] : -1000);
		}
		if (icon == 0 && row.pos.Length != 0)
		{
			icon = row.pos[2];
		}
		spatial.Create(id, x, y, icon);
		if (register)
		{
			spatial.Register();
		}
		if (parent != null)
		{
			parent.AddChild(spatial);
		}
		spatial.OnAfterCreate();
		return spatial;
	}

	public static Zone CreateInstance(string id, ZoneInstance instance)
	{
		Zone topZone = EClass._zone.GetTopZone();
		Zone zone = SpatialGen.Create(id, EClass._zone.Region, true, topZone.x, topZone.y, 0) as Zone;
		zone.instance = instance;
		instance.x = EClass.pc.pos.x;
		instance.z = EClass.pc.pos.z;
		instance.uidZone = EClass._zone.uid;
		zone.dateExpire = EClass.world.date.GetRaw(0) + 1440;
		return zone;
	}
}
