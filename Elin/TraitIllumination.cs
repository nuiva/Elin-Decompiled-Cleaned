using System;

public class TraitIllumination : Trait
{
	public override bool AutoToggle
	{
		get
		{
			return true;
		}
	}

	public override Trait.TileMode tileMode
	{
		get
		{
			return Trait.TileMode.Illumination;
		}
	}
}
