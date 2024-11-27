using System;

public class TraitManhole : TraitStairsDown
{
	public override bool AutoEnter
	{
		get
		{
			return false;
		}
	}
}
