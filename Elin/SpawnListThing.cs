using System;

public class SpawnListThing : EClass
{
	public static SpawnList Get(string id, Func<SourceThing.Row, bool> func)
	{
		return SpawnList.Get(id, "thing", new ThingFilter
		{
			ShouldPass = func
		});
	}
}
