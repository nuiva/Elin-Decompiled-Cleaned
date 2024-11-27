using System;

public class TraitPuppy : TraitUniqueChara
{
	public override bool CanBeBanished
	{
		get
		{
			return false;
		}
	}
}
