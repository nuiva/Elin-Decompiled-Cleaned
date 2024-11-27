using System;
using UnityEngine;

public class TraitTicketFurniture : Trait
{
	public Zone zone
	{
		get
		{
			return RefZone.Get(this.owner.refVal);
		}
		set
		{
			this.owner.refVal = ((value != null) ? value.uid : 0);
		}
	}

	public override bool IsTool
	{
		get
		{
			return true;
		}
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		if (EClass._zone.GetTopZone() != this.zone)
		{
			return;
		}
		p.pos.Things.ForEach(delegate(Thing t)
		{
			if (!t.IsInstalled || !t.isNPCProperty || !t.trait.CanBeHeld || t.trait.IsDoor || t.isMasked || t.source.value == 0)
			{
				return;
			}
			if (!(EClass._zone is Zone_LittleGarden) && !t.trait.CanBeDestroyed)
			{
				return;
			}
			int price = this.GetPrice(t) * t.Num;
			p.TrySetAct("actCollectFurniture".lang(price.ToString() ?? "", t.Name, null, null, null) + ((t.category.ticket >= 10) ? "ticketNotIntended".lang() : ""), delegate()
			{
				if (this.owner.Num < price)
				{
					Msg.Say("notEnoughTicket");
					return false;
				}
				SE.Pay();
				this.owner.ModNum(-price, true);
				t.isNPCProperty = false;
				if (t.trait is TraitPillow)
				{
					t.noSell = true;
				}
				EClass.pc.Pick(t, true, true);
				return false;
			}, null, 1);
		});
	}

	public int GetPrice(Thing t)
	{
		int num = (t.GetPrice(CurrencyType.Money, false, PriceType.Default, null) / 500 + 1) * t.category.ticket;
		if (EClass._zone is Zone_LittleGarden)
		{
			if (num >= 10)
			{
				num = 10;
			}
			num = Mathf.Max(num + EClass.player.little_dead - EClass.player.little_saved / 5, num);
		}
		return num;
	}

	public override void SetName(ref string s)
	{
		if (this.zone != null)
		{
			s = "_of".lang(this.zone.Name, s, null, null, null);
		}
	}

	public static void SetZone(Zone zone, Thing t)
	{
		int num = 0;
		if (zone != null)
		{
			if (zone.IsTown || zone is Zone_LittleGarden)
			{
				num = zone.GetTopZone().uid;
			}
			if (zone.IsPCFaction)
			{
				num = EClass.game.spatials.Find("mysilia").uid;
			}
		}
		if (num == 0)
		{
			num = EClass.game.spatials.Find("palmia").uid;
		}
		t.refVal = num;
	}
}
