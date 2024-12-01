using System;

public class SpawnListChara : EClass
{
	public static SpawnList Get(string id, Func<SourceChara.Row, bool> func)
	{
		return SpawnList.Get(id, "chara", new CharaFilter
		{
			ShouldPass = func
		});
	}
}
