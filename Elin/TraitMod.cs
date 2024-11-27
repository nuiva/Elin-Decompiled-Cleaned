using System;

public class TraitMod : TraitItem
{
	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public SourceElement.Row source
	{
		get
		{
			return EClass.sources.elements.map[this.owner.refVal];
		}
	}

	public override void OnCreate(int lv)
	{
		Tuple<SourceElement.Row, int> tuple = Thing.GetEnchant(lv, (SourceElement.Row r) => r.tag.Contains("modRanged"), false);
		if (tuple == null)
		{
			tuple = new Tuple<SourceElement.Row, int>(EClass.sources.elements.map[600], EClass.rnd(10) + 1);
		}
		this.owner.refVal = tuple.Item1.id;
		this.owner.encLV = tuple.Item2;
	}

	public override bool OnUse(Chara c)
	{
		LayerDragGrid.Create(new InvOwnerMod(this.owner, null, CurrencyType.None), false);
		return false;
	}

	public override void SetName(ref string s)
	{
		s = "_of".lang(this.source.GetName(), s, null, null, null);
	}

	public override int GetValue()
	{
		int num = this.source.value * this.owner.encLV;
		return base.GetValue() * num / 100;
	}
}
