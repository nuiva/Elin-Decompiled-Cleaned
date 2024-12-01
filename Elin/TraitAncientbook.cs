public class TraitAncientbook : TraitBaseSpellbook
{
	public override int Difficulty => 10 + owner.refVal * 15;

	public override Type BookType => Type.Ancient;

	public override bool CanStack => owner.isOn;

	public override bool HasCharges => !owner.isOn;

	public override int eleParent => 74;

	public override bool CanStackTo(Thing to)
	{
		if (owner.isOn)
		{
			return to.isOn;
		}
		return false;
	}
}
