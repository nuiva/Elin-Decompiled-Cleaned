using System;

public class RefArea : EClass
{
	public static BaseArea Get(int uid)
	{
		if (uid != 0)
		{
			return EClass._map.rooms.mapIDs.TryGetValue(uid, null);
		}
		return null;
	}

	public static int Set(BaseArea a)
	{
		if (a == null)
		{
			return 0;
		}
		return a.uid;
	}
}
