public class TraitTrapSister : TraitTrap
{
	public override bool CanDisarmTrap => false;

	public override int DestroyChanceOnActivateTrap => 100;
}
