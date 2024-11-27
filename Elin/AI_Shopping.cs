using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_Shopping : AIAct
{
	public static bool TryShop(Chara c, bool realtime)
	{
		if (c.memberType != FactionMemberType.Guest || !EClass._zone.IsPCFaction || EClass._map.props.sales.Count == 0)
		{
			return false;
		}
		Card card = EClass._map.props.sales.RandomItem<Card>();
		int c_allowance = c.c_allowance;
		if (!EClass.debug.enable && c_allowance <= 0)
		{
			return false;
		}
		bool flag = card.IsContainer && card.things.Count > 0;
		List<Card> list = new List<Card>();
		if (flag)
		{
			using (List<Thing>.Enumerator enumerator = card.things.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Thing thing = enumerator.Current;
					if (TraitSalesTag.CanTagSale(thing, true))
					{
						list.Add(thing);
					}
				}
				goto IL_C8;
			}
		}
		list.Add(card);
		IL_C8:
		foreach (Card card2 in list)
		{
			int num = 25;
			int price = card2.GetPrice(CurrencyType.Money, true, PriceType.PlayerShop, null);
			if (price >= c_allowance)
			{
				num = num * 10 * price / c_allowance;
			}
			if (price >= 10000)
			{
				num *= 15;
			}
			else if (price >= 5000)
			{
				num *= 10;
			}
			else if (price >= 1000)
			{
				num *= 6;
			}
			else if (price >= 200)
			{
				num *= 3;
			}
			if (EClass.Branch != null)
			{
				if (EClass.Branch.policies.IsActive(2817, -1))
				{
					num = num * (300 + EClass.Branch.Evalue(2817)) / 100;
				}
				if (EClass.Branch.policies.IsActive(2816, -1))
				{
					num = num * (1000 + EClass.Branch.Evalue(2816) * 2) / 100;
				}
			}
			if (!realtime)
			{
				num /= 4;
			}
			if (EClass.debug.enable || EClass.rnd(num + 1) == 0)
			{
				if (realtime)
				{
					c.SetAI(new AI_Shopping
					{
						container = (flag ? card : null),
						dest = card2
					});
					return true;
				}
				AI_Shopping.Buy(c, card2, false, flag ? card : null);
			}
		}
		return false;
	}

	public static bool TryRestock(Chara c, bool realtime)
	{
		for (int i = 0; i < (realtime ? 1 : 10); i++)
		{
			bool flag = false;
			foreach (Thing thing in EClass._map.things)
			{
				if (EClass.rnd(2) != 0 && thing.IsInstalled)
				{
					TraitSalesTag traitSalesTag = thing.trait as TraitSalesTag;
					if (traitSalesTag != null && traitSalesTag.IsOn && !thing.GetStr(11, null).IsEmpty())
					{
						Thing thing2 = EClass._zone.TryGetRestock<TraitSpotStock>(thing.GetStr(11, null));
						if (thing2 != null)
						{
							if (realtime)
							{
								thing.PlaySound("restock", 1f, true);
							}
							thing.Destroy();
							thing2.isSale = true;
							EClass._zone.AddCard(thing2, thing.pos).Install();
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				break;
			}
		}
		return false;
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		Card _dest = this.container ?? this.dest;
		if (_dest.ExistsOnMap)
		{
			yield return base.DoGoto(_dest.pos, 1, false, null);
		}
		if (!_dest.ExistsOnMap || !_dest.isSale)
		{
			yield return base.Success(null);
		}
		AI_Shopping.Buy(this.owner, this.dest, true, this.container);
		yield return base.Success(null);
		yield break;
	}

	public static void SellChara(Chara c)
	{
		Msg.Say("sell_resident", c, null, null, null);
		c.homeBranch.BanishMember(c, true);
		EClass.player.ModKarma(-1);
	}

	public static void Buy(Chara c, Card dest, bool realtime, Card container)
	{
		Point point = dest.pos.Copy();
		int price = dest.GetPrice(CurrencyType.Money, true, PriceType.PlayerShop, null);
		if (dest.isThing && (dest.isDestroyed || dest.Thing.IsSharedContainer))
		{
			return;
		}
		if (container == null && !dest.isSale)
		{
			return;
		}
		if (container != null && container.c_lockLv != 0)
		{
			return;
		}
		Card card = dest.isChara ? dest : dest.Split(1);
		EClass.Branch.incomeShop += price;
		card.isSale = false;
		if (card == dest)
		{
			EClass._map.props.sales.Remove(card);
		}
		if (realtime)
		{
			c.Talk("shop_buy", null, null, false);
			c.PlaySound("money", 1f, true);
			Msg.alwaysVisible = true;
			Msg.Say("shop_buy", c, card, Lang._currency(price, "money"), null);
			if (card.isThing)
			{
				c.AddCard(card);
				c.c_allowance -= price;
				if (!c.TryUse(card.Thing))
				{
					card.ModNum(-1, true);
				}
			}
			else
			{
				AI_Shopping.SellChara(card.Chara);
			}
		}
		else if (card.isThing)
		{
			card.ModNum(-1, true);
		}
		else
		{
			AI_Shopping.SellChara(card.Chara);
		}
		if (card.isStolen || card.isChara)
		{
			Guild.Thief.AddContribution((int)Mathf.Sqrt((float)price) / 2);
		}
		if (container != null)
		{
			if (container.things.Count == 0)
			{
				container.isSale = false;
				EClass._map.props.sales.Remove(container);
			}
		}
		else if (!dest.ExistsOnMap && dest.isThing)
		{
			Thing thing = ThingGen.Create("tag_sell", -1, -1);
			thing.isOn = true;
			thing.SetStr(11, dest.category.id);
			EClass._zone.AddCard(thing, point).Install();
		}
		EClass.Branch.Log("shop_buy", c, card, Lang._currency(price, "money"), null);
	}

	public Card container;

	public Card dest;
}
