using System;

public class TraitScrollRandom : TraitScrollStatic
{
	public override EffectId idEffect
	{
		get
		{
			return this.source.proc[0].ToEnum(true);
		}
	}

	public override int Power
	{
		get
		{
			return 200;
		}
	}

	public override string AliasEle
	{
		get
		{
			return this.source.aliasRef;
		}
	}

	public override string N1
	{
		get
		{
			return this.source.proc.TryGet(1, true);
		}
	}

	public override SourceElement.Row source
	{
		get
		{
			return EClass.sources.elements.map[this.owner.refVal];
		}
	}

	public override string GetName()
	{
		return Lang.TryGet("scroll_" + this.source.alias) ?? "scroll_".lang(this.source.GetName().ToLower(), null, null, null, null);
	}

	public override void OnCreate(int lv)
	{
		this.owner.refVal = TraitScrollRandom.selector.Select(lv);
	}

	public static ElementSelecter selector = new ElementSelecter
	{
		type = "S",
		lvMod = 20
	};
}
