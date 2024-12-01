public class RefArea : EClass
{
	public static BaseArea Get(int uid)
	{
		if (uid != 0)
		{
			return EClass._map.rooms.mapIDs.TryGetValue(uid);
		}
		return null;
	}

	public static int Set(BaseArea a)
	{
		return a?.uid ?? 0;
	}
}
