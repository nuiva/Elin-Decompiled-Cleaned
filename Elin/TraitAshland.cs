using System;

public class TraitAshland : TraitUniqueChara
{
	public override bool CanInvite
	{
		get
		{
			return EClass._zone.id == "lothria";
		}
	}
}
