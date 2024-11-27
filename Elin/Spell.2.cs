using System;

public class Spell : Ability
{
	public override bool PotentialAsStock
	{
		get
		{
			return true;
		}
	}
}
