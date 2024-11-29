using System;
using UnityEngine;

public class Ability : Act
{
	public override bool CanLink(ElementContainer owner)
	{
		return owner.Card == null && !base.IsGlobalElement;
	}

	public override int GetSourceValue(int v, int lv, SourceValueType type)
	{
		if (type != SourceValueType.Chara)
		{
			return base.GetSourceValue(v, lv, type);
		}
		return 10 * (100 + (lv - 1) * base.source.lvFactor / 10) / 100;
	}

	public override bool CanPressRepeat
	{
		get
		{
			return base.source.tag.Contains("repeat");
		}
	}

	public override int GetPower(Card c)
	{
		int num = base.Value * 8 + 50;
		if (!c.IsPC)
		{
			num = Mathf.Max(num, c.LV * 6 + 30);
			if (c.IsPCFactionOrMinion && !base.source.aliasParent.IsEmpty())
			{
				num = Mathf.Max(num, c.Evalue(base.source.aliasParent) * 4 + 30);
			}
		}
		num = EClass.curve(num, 400, 100, 75);
		if (this is Spell)
		{
			num = num * (100 + c.Evalue(411)) / 100;
		}
		return num;
	}

	public override void OnChangeValue()
	{
		Card card = this.owner.Card;
		if (card != null && card._IsPC)
		{
			LayerAbility.SetDirty(this);
		}
	}
}
