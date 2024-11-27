using System;

public class TraitSnow : Trait
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
			return ThrowType.Snow;
		}
	}

	public override EffectDead EffectDead
	{
		get
		{
			return EffectDead.None;
		}
	}
}
