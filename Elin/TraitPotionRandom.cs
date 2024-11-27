using System;

public class TraitPotionRandom : TraitPotion
{
	public SourceElement.Row source
	{
		get
		{
			return EClass.sources.elements.map[this.owner.refVal];
		}
	}

	public override SourceElement.Row GetRefElement()
	{
		return this.source;
	}

	public override string AliasEle
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
			return 200;
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

	public override bool IsNeg
	{
		get
		{
			return this.source.tag.Contains("neg");
		}
	}

	public override int GetValue()
	{
		return this.source.value * 120 / 100;
	}

	public override void OnCreate(int lv)
	{
		this.owner.refVal = TraitPotionRandom.selecter.Select(lv);
	}

	public override string GetName()
	{
		return Lang.TryGet("potion_" + this.source.alias) ?? "potion_".lang(this.source.GetName().ToLower(), null, null, null, null);
	}

	public static ElementSelecter selecter = new ElementSelecter
	{
		type = "P",
		lvMod = 10
	};
}
