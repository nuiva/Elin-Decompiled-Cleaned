public class RefZone : EClass
{
	public static Zone Get(int uid)
	{
		if (uid != 0)
		{
			return EClass.game.spatials.map.TryGetValue(uid) as Zone;
		}
		return null;
	}

	public static int Set(Zone zone)
	{
		return zone?.uid ?? 0;
	}
}
