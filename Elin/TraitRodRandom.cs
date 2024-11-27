using System;

public class TraitRodRandom : TraitRod
{
	public override SourceElement.Row source
	{
		get
		{
			return EClass.sources.elements.map[this.owner.refVal];
		}
	}

	public override string aliasEle
	{
		get
		{
			return this.source.aliasRef;
		}
	}

	public override int Power
	{
		get
		{
			return 100;
		}
	}

	public override EffectId IdEffect
	{
		get
		{
			return this.source.proc[0].ToEnum(true);
		}
	}

	public override string N1
	{
		get
		{
			return this.source.proc.TryGet(1, true);
		}
	}

	public override bool IsNegative
	{
		get
		{
			return base.IsNegative || this.source.tag.Contains("neg");
		}
	}

	public override void OnCreate(int lv)
	{
		this.owner.c_charges = EClass.rnd(this.source.charge * 150 / 100);
		this.owner.refVal = TraitRodRandom.selecter.Select(lv);
	}

	public override string GetName()
	{
		return "rod_".lang(this.source.GetName().ToLower(), null, null, null, null);
	}

	public static ElementSelecter selecter = new ElementSelecter
	{
		type = "R",
		lvMod = 10
	};
}
