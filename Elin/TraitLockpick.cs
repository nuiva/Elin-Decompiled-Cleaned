public class TraitLockpick : Trait
{
	public override bool CanStack => false;

	public override bool HasCharges => true;

	public override void OnCreate(int lv)
	{
		owner.c_charges = EClass.rnd(20) + 1;
	}
}
