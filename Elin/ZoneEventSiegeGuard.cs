using System;

public class ZoneEventSiegeGuard : ZoneEventSiege
{
	public override Chara CreateChara()
	{
		return CharaGen.Create("guard", -1);
	}
}
