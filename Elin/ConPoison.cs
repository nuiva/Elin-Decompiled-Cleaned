using System;

public class ConPoison : BadCondition
{
	public override Emo2 EmoIcon
	{
		get
		{
			return Emo2.poison;
		}
	}

	public override bool PreventRegen
	{
		get
		{
			return true;
		}
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner, false);
		this.elements = new ElementContainer();
		this.elements.SetParent(this.owner);
	}

	public override void OnChangePhase(int lastPhase, int newPhase)
	{
		switch (newPhase)
		{
		case 1:
			this.elements.SetBase(70, -10, 0);
			return;
		case 2:
			this.elements.SetBase(70, -10, 0);
			return;
		case 3:
			this.elements.SetBase(70, -15, 0);
			return;
		default:
			this.elements.SetBase(70, -5, 0);
			return;
		}
	}

	public override void Tick()
	{
		if (EClass.rnd(5) == 0)
		{
			this.owner.DamageHP(EClass.rnd(this.owner.END / 10 + 2) + 1, AttackSource.Condition, null);
		}
		base.Mod(-1, false);
	}
}
