using System;

public class TraitThrownExplosive : TraitThrown
{
	public override bool ShowAsTool
	{
		get
		{
			return false;
		}
	}

	public override ThrowType ThrowType
	{
		get
		{
			return ThrowType.Explosive;
		}
	}
}
