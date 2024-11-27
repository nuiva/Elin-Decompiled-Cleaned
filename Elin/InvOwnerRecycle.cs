using System;
using UnityEngine;

public class InvOwnerRecycle : InvOwnerDraglet
{
	public override string langTransfer
	{
		get
		{
			return "invRecycle";
		}
	}

	public override InvOwnerDraglet.ProcessType processType
	{
		get
		{
			return InvOwnerDraglet.ProcessType.Consume;
		}
	}

	public override bool InvertSell
	{
		get
		{
			return false;
		}
	}

	public override bool DenyImportant
	{
		get
		{
			return true;
		}
	}

	public InvOwnerRecycle(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.Ecopo) : base(owner, container, _currency)
	{
	}

	public override void BuildUICurrency(UICurrency uiCurrency, bool canReroll = false)
	{
		uiCurrency.SetActive(true);
		uiCurrency.Build(new UICurrency.Options
		{
			ecopo = true,
			influence = canReroll
		});
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return !t.c_isImportant && t.things.Count == 0 && t.trait.CanBeDestroyed && !t.trait.CanOnlyCarry && t.rarity < Rarity.Artifact && t.category.GetRoot().id != "currency" && !(t.trait is TraitRecycle);
	}

	public override void _OnProcess(Thing t)
	{
		SE.Play("trash");
		Msg.Say("dump", t, this.Container.Name, null, null);
		int num = t.Num * Mathf.Clamp(t.GetPrice(CurrencyType.Money, false, PriceType.Default, null) / 100, 1, 100);
		num = EClass.rndHalf(num);
		if (num != 0)
		{
			EClass.pc.Pick(ThingGen.Create("ecopo", -1, -1).SetNum(num / 10 + 1), true, true);
		}
		t.Destroy();
	}

	public TraitRecycle recycle;
}
