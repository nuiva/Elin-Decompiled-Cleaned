using System;
using System.Collections.Generic;
using UnityEngine;

public class WidgetSearch : WidgetCodex
{
	public override WidgetCodex.SearchType type
	{
		get
		{
			return WidgetCodex.SearchType.Search;
		}
	}

	public override void OnActivate()
	{
		base.OnActivate();
		WidgetSearch.Instance = this;
		WidgetSearch.selected = null;
		this.RefreshList();
	}

	public override bool CheckClose()
	{
		return Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F);
	}

	public override void Search(string s)
	{
		s = s.ToLower();
		this.buttonClear.SetActive(this.field.text != "");
		if (s == this.lastSearch)
		{
			return;
		}
		HashSet<Card> newCards = new HashSet<Card>();
		if (!s.IsEmpty())
		{
			if (EMono._zone.IsTown || EMono._zone.IsPCFaction || EMono._zone is Zone_Tent)
			{
				foreach (Chara chara in EMono._map.charas)
				{
					if (chara.IsNeutralOrAbove() && (chara.Name.ToLower().Contains(s) || chara.sourceCard.GetSearchName(false).Contains(s)))
					{
						newCards.Add(chara);
					}
				}
			}
			if (EMono._zone.IsPCFaction || EMono._zone is Zone_Tent)
			{
				foreach (Thing thing in EMono._map.things)
				{
					if (thing.Name.ToLower().Contains(s) || thing.sourceCard.GetSearchName(false).Contains(s))
					{
						newCards.Add(thing);
					}
				}
				foreach (Card card in EMono._map.props.stocked.all.Values)
				{
					if (card.parent is Thing && (card.parent as Thing).c_lockLv <= 0 && (card.Name.ToLower().Contains(s) || card.sourceCard.GetSearchName(false).Contains(s)))
					{
						newCards.Add(card);
					}
				}
			}
			Action<Thing> <>9__0;
			for (int i = 0; i < 2; i++)
			{
				foreach (Chara chara2 in EMono._map.charas)
				{
					if ((i != 0 || chara2 != EMono.pc) && (i != 1 || chara2 == EMono.pc) && chara2.IsPCFactionOrMinion)
					{
						ThingContainer things = chara2.things;
						Action<Thing> action;
						if ((action = <>9__0) == null)
						{
							action = (<>9__0 = delegate(Thing t)
							{
								Card card2 = t.parent as Card;
								if (((card2 != null) ? card2.trait : null) is TraitChestMerchant)
								{
									return;
								}
								if (t.Name.ToLower().Contains(s) || t.source.GetSearchName(false).Contains(s))
								{
									newCards.Add(t);
								}
							});
						}
						things.Foreach(action, true);
					}
				}
			}
		}
		if (!newCards.SetEquals(this.cards))
		{
			this.cards = newCards;
			this.RefreshList();
		}
		this.cgResult.alpha = ((this.list.ItemCount > 0) ? 1f : 0f);
		this.lastSearch = s;
	}

	public override void RefreshList()
	{
		LayerInventory.SetDirtyAll(false);
		WidgetSearch.selected = null;
		BaseList list = this.list;
		UIList.Callback<Card, ButtonGrid> callback = new UIList.Callback<Card, ButtonGrid>();
		callback.onClick = delegate(Card a, ButtonGrid b)
		{
			Card rootCard = a.GetRootCard();
			if (rootCard == EMono.pc)
			{
				LayerInventory.SetDirtyAll(false);
				WidgetSearch.selected = a;
				SE.Click();
				return;
			}
			if (!EMono.pc.ai.IsNoGoal)
			{
				SE.Beep();
				return;
			}
			if (a.isDestroyed || !rootCard.pos.IsValid)
			{
				SE.Beep();
				this.Search(this.field.text);
				return;
			}
			SE.Click();
			WidgetSearch.selected = null;
			EMono.pc.SetAIImmediate(new AI_Goto(rootCard.pos.Copy(), 0, false, false));
			ActionMode.Adv.SetTurbo(3);
			base.Close();
		};
		callback.onRedraw = delegate(Card a, ButtonGrid b, int i)
		{
			b.SetCard(a, ButtonGrid.Mode.Search, null);
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (Card o in this.cards)
			{
				this.list.Add(o);
			}
		};
		list.callbacks = callback;
		this.list.List();
		this.list.dsv.OnResize();
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		LayerInventory.SetDirtyAll(false);
	}

	public static WidgetSearch Instance;

	public HashSet<Card> cards = new HashSet<Card>();

	public static Card selected;

	public CanvasGroup cgResult;
}
