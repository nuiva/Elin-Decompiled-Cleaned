using System;

public class InvOwnerEquip : InvOwner
{
	public override bool AlwaysShowTooltip
	{
		get
		{
			return true;
		}
	}

	public override bool ShowNew
	{
		get
		{
			return true;
		}
	}

	public override bool AllowDrop(Thing t)
	{
		return t.blessedState >= BlessedState.Normal;
	}

	public InvOwnerEquip(Card owner, BodySlot slot, Card container = null, CurrencyType _currency = CurrencyType.None) : base(owner, container, _currency, PriceType.Default)
	{
		this.slot = slot;
	}

	public override void ListInteractions(InvOwner.ListInteraction list, Thing t, Trait trait, ButtonGrid b, bool context)
	{
		list.Add("actUnequip", 0, delegate()
		{
			if (this.IsFailByCurse(t))
			{
				return;
			}
			if (EClass.pc.things.IsFull(0))
			{
				Msg.Say("backpack_full");
				SE.BeepSmall();
				return;
			}
			EClass.pc.body.Unequip(t, true);
			EClass.Sound.Play("equip");
		}, null);
	}

	public override bool IsFailByCurse(Thing t)
	{
		if (t != null && t.blessedState <= BlessedState.Cursed)
		{
			Msg.Say("unequipCursed", t, null, null, null);
			SE.Play("curse3");
			return true;
		}
		return false;
	}

	public override void OnWriteNote(ButtonGrid button, UINote n)
	{
		if (button.card == null)
		{
			n.Clear();
			n.AddHeader(this.slot.name, null);
			n.AddText("noEQ".lang(), FontColor.DontChange);
			if (this.slot.elementId == 35)
			{
				Thing.AddAttackEvaluation(n, base.Chara, null);
			}
			return;
		}
		base.OnWriteNote(button, n);
	}

	public BodySlot slot;
}
