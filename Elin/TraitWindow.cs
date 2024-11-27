using System;

public class TraitWindow : Trait
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
			return EClass.world.date.IsNight;
		}
	}

	public override bool AlwaysHideOnLowWall
	{
		get
		{
			return true;
		}
	}

	public override bool IsOpenSight
	{
		get
		{
			return true;
		}
	}

	public override bool ShouldRefreshTile
	{
		get
		{
			return true;
		}
	}

	public override bool UseExtra
	{
		get
		{
			return (EClass.world.date.IsNight || EClass._map.IsIndoor) && !this.owner.Cell.isCurtainClosed;
		}
	}
}
