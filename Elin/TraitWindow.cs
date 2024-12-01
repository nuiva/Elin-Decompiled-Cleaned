public class TraitWindow : Trait
{
	public override bool CanBeOnlyBuiltInHome => true;

	public override bool UseAltTiles => EClass.world.date.IsNight;

	public override bool AlwaysHideOnLowWall => true;

	public override bool IsOpenSight => true;

	public override bool ShouldRefreshTile => true;

	public override bool UseExtra
	{
		get
		{
			if (EClass.world.date.IsNight || EClass._map.IsIndoor)
			{
				return !owner.Cell.isCurtainClosed;
			}
			return false;
		}
	}
}
