using System;

public class TraitFlyer : Trait
{
	public override ThrowType ThrowType
	{
		get
		{
			return ThrowType.Flyer;
		}
	}
}
