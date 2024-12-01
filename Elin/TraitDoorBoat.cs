public class TraitDoorBoat : Trait
{
	public override bool ShouldRefreshTile => true;

	public override bool IsOpenPath => true;

	public override bool IsChangeFloorHeight => true;
}
