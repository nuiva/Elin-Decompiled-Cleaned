using System;
using UnityEngine;
using UnityEngine.UI;

public class TraitFigure : Trait
{
	public override bool CanBeHallucinated
	{
		get
		{
			return false;
		}
	}

	public CardRow source
	{
		get
		{
			return EClass.sources.cards.map.TryGetValue(this.owner.c_idRefCard, null);
		}
	}

	public virtual bool ShowShadow
	{
		get
		{
			return true;
		}
	}

	public virtual int GetMatColor()
	{
		return -1;
	}

	public override void OnSetCardGrid(ButtonGrid b)
	{
		if (this.owner.c_idRefCard.IsEmpty())
		{
			return;
		}
		RenderRow renderRow = EClass.sources.charas.map[this.owner.c_idRefCard];
		Transform transform = b.Attach<Transform>("figure", false);
		renderRow.SetImage(transform.GetChild(0).GetComponent<Image>(), null, 0, false, 0, 0);
	}

	public override int GetValue()
	{
		if (this.source == null)
		{
			return base.GetValue();
		}
		return (base.GetValue() + this.source.LV * 50) * (this.source.multisize ? 2 : 1) * ((this.source.quality >= 4) ? 2 : 1);
	}
}
