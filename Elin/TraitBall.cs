using System;

public class TraitBall : Trait
{
	public override bool IsThrowMainAction
	{
		get
		{
			return true;
		}
	}

	public override ThrowType ThrowType
	{
		get
		{
			return ThrowType.Ball;
		}
	}
}
