public class TraitStairs : TraitNewZone
{
	public override bool CanUseInTempDungeon => true;

	public override bool CanToggleAutoEnter => true;

	public override Point GetExitPos()
	{
		return new Point(owner.pos);
	}
}
