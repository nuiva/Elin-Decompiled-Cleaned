using System;

public class TraitBoard : Trait
{
	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeDisassembled
	{
		get
		{
			return true;
		}
	}
}
