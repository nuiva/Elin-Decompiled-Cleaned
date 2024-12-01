public class TraitRod : TraitTool
{
	public override bool InvertHeldSprite => true;

	public override bool HasCharges => true;

	public virtual string aliasEle => null;

	public virtual SourceElement.Row source => null;

	public virtual int Power
	{
		get
		{
			if (owner.sourceCard.vals.Length <= 2)
			{
				return 100;
			}
			return owner.sourceCard.vals[2].ToInt();
		}
	}

	public virtual bool IsNegative => owner.IsNegativeGift;

	public virtual EffectId IdEffect => owner.sourceCard.vals[1].ToEnum<EffectId>();

	public virtual string N1
	{
		get
		{
			if (owner.sourceCard.vals.Length <= 3)
			{
				return "";
			}
			return owner.sourceCard.vals[3];
		}
	}

	public override bool DisableAutoCombat => true;

	public override SourceElement.Row GetRefElement()
	{
		return source;
	}

	public override void OnCreate(int lv)
	{
		owner.c_charges = EClass.rnd(12);
		if (owner.id == "rod_wish")
		{
			owner.c_charges = 1;
		}
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.TrySetAct(new ActZap
		{
			trait = this
		}, owner);
	}
}
