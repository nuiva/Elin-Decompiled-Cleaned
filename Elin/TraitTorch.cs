using System;

public class TraitTorch : Trait
{
	public override bool UseExtra
	{
		get
		{
			return this.owner.isOn && !this.owner.Cell.isCurtainClosed;
		}
	}

	public override ToggleType ToggleType
	{
		get
		{
			return ToggleType.Fire;
		}
	}
}
