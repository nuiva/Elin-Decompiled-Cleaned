public class TraitBananaPeel : TraitTrap
{
	public override bool IsNegativeEffect => false;

	public override bool StartHidden => false;

	public override bool CanDisarmTrap => false;

	public override bool CanBeHeld => true;
}
