using System;

public class TraitRod : TraitTool
{
	public override bool InvertHeldSprite
	{
		get
		{
			return true;
		}
	}

	public override bool HasCharges
	{
		get
		{
			return true;
		}
	}

	public virtual string aliasEle
	{
		get
		{
			return null;
		}
	}

	public virtual SourceElement.Row source
	{
		get
		{
			return null;
		}
	}

	public override SourceElement.Row GetRefElement()
	{
		return this.source;
	}

	public virtual int Power
	{
		get
		{
			if (this.owner.sourceCard.vals.Length <= 2)
			{
				return 100;
			}
			return this.owner.sourceCard.vals[2].ToInt();
		}
	}

	public virtual bool IsNegative
	{
		get
		{
			return this.owner.IsNegativeGift;
		}
	}

	public virtual EffectId IdEffect
	{
		get
		{
			return this.owner.sourceCard.vals[1].ToEnum(true);
		}
	}

	public virtual string N1
	{
		get
		{
			if (this.owner.sourceCard.vals.Length <= 3)
			{
				return "";
			}
			return this.owner.sourceCard.vals[3];
		}
	}

	public override bool DisableAutoCombat
	{
		get
		{
			return true;
		}
	}

	public override void OnCreate(int lv)
	{
		this.owner.c_charges = EClass.rnd(12);
		if (this.owner.id == "rod_wish")
		{
			this.owner.c_charges = 1;
		}
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.TrySetAct(new ActZap
		{
			trait = this
		}, this.owner);
	}
}
