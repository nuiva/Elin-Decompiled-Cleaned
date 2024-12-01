public class TraitCurtain : Trait
{
	public override bool CanBeOnlyBuiltInHome => true;

	public override bool UseAltTiles => owner.isOn;

	public override bool AlwaysHideOnLowWall => true;

	public override ToggleType ToggleType => ToggleType.Curtain;

	public override bool ShouldRefreshTile => true;
}
