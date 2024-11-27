using System;

public class TraitSalesTag : Trait
{
	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public override bool IsTool
	{
		get
		{
			return !this.owner.isOn;
		}
	}

	public override bool UseAltTiles
	{
		get
		{
			return this.owner.isOn;
		}
	}

	public static bool CanTagSale(Card t, bool insideContainer = false)
	{
		if (t.isSale)
		{
			return false;
		}
		if (t.isChara)
		{
			Chara chara = t.Chara;
			if (chara.trait is TraitUniqueChara || chara.IsUnique || EClass.game.cards.listAdv.Contains(chara))
			{
				return false;
			}
			if (!chara.IsPCFaction || chara.IsPCParty)
			{
				return false;
			}
		}
		else
		{
			if (!insideContainer && !t.IsInstalled)
			{
				return false;
			}
			if (!t.trait.CanBeHeld || !t.trait.CanBeDestroyed || !t.trait.CanBeShipped || t.isMasked || t.trait is TraitSpot)
			{
				return false;
			}
		}
		return t.GetPrice(CurrencyType.Money, true, PriceType.PlayerShop, null) != 0;
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		if (!EClass._zone.IsPCFaction || this.owner.isOn)
		{
			return;
		}
		p.pos.ListCards(false).ForEach(delegate(Card t)
		{
			if (t.trait is TraitSalesTag && t.isOn)
			{
				p.TrySetAct("actRemove", delegate()
				{
					t.Destroy();
					SE.Trash();
					return false;
				}, t, null, 1, false, true, false);
			}
			if (t.isSale)
			{
				p.TrySetAct("actRemoveSalesTag".lang(t.Name, null, null, null, null), delegate()
				{
					SE.Play("paper");
					t.SetSale(false);
					return false;
				}, null, 1);
				return;
			}
			if (!TraitSalesTag.CanTagSale(t, false))
			{
				return;
			}
			int price = t.GetPrice(CurrencyType.Money, true, PriceType.PlayerShop, null);
			p.TrySetAct("actSalesTag".lang(Lang._currency(price, "money"), t.Name, null, null, null), delegate()
			{
				SE.Play("paper");
				t.SetSale(true);
				return false;
			}, null, 1);
		});
	}

	public override bool CanStackTo(Thing to)
	{
		return false;
	}
}
