using System;

public class TraitCurtain : Trait
{
	public override bool CanBeOnlyBuiltInHome
	{
		get
		{
			return true;
		}
	}

	public override bool UseAltTiles
	{
		get
		{
			return this.owner.isOn;
		}
	}

	public override bool AlwaysHideOnLowWall
	{
		get
		{
			return true;
		}
	}

	public override ToggleType ToggleType
	{
		get
		{
			return ToggleType.Curtain;
		}
	}

	public override bool ShouldRefreshTile
	{
		get
		{
			return true;
		}
	}
}
