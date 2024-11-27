using System;

public class TraitTreeEther : Trait
{
	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeHeld
	{
		get
		{
			return false;
		}
	}
}
