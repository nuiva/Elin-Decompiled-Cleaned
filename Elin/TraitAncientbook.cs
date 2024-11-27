using System;

public class TraitAncientbook : TraitBaseSpellbook
{
	public override int Difficulty
	{
		get
		{
			return 10 + this.owner.refVal * 15;
		}
	}

	public override TraitBaseSpellbook.Type BookType
	{
		get
		{
			return TraitBaseSpellbook.Type.Ancient;
		}
	}

	public override bool CanStack
	{
		get
		{
			return this.owner.isOn;
		}
	}

	public override bool CanStackTo(Thing to)
	{
		return this.owner.isOn && to.isOn;
	}

	public override bool HasCharges
	{
		get
		{
			return !this.owner.isOn;
		}
	}

	public override int eleParent
	{
		get
		{
			return 74;
		}
	}
}
