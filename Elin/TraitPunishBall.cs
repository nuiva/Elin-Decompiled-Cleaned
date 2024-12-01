public class TraitPunishBall : Trait
{
	public override bool CanBeDropped => false;

	public override bool CanStack => false;

	public override bool CanBeStolen => false;

	public override bool CanBeDestroyed => false;

	public override bool CanBeHeld => EClass.debug.enable;
}
