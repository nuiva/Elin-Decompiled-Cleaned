using System;

public class RefZone : EClass
{
	public static Zone Get(int uid)
	{
		if (uid != 0)
		{
			return EClass.game.spatials.map.TryGetValue(uid, null) as Zone;
		}
		return null;
	}

	public static int Set(Zone zone)
	{
		if (zone == null)
		{
			return 0;
		}
		return zone.uid;
	}
}
