using System;
using System.Collections.Generic;
using UnityEngine;

public class InvOwner : EClass
{
	public static bool HasTrader
	{
		get
		{
			return InvOwner.Trader != null;
		}
	}

	public static bool FreeTransfer
	{
		get
		{
			return !InvOwner.HasTrader || InvOwner.Trader.currency == CurrencyType.None;
		}
	}

	public virtual bool AllowAutouse
	{
		get
		{
			return true;
		}
	}

	public virtual bool AllowContext
	{
		get
		{
			return true;
		}
	}

	public virtual bool AllowSell
	{
		get
		{
			return this.currency == CurrencyType.None || this.owner.trait.AllowSell;
		}
	}

	public virtual bool AllowHold(Thing t)
	{
		if (!t.trait.CanBeDropped)
		{
			return false;
		}
		if (t.isEquipped && t.IsCursed)
		{
			return false;
		}
		if (this.Container.isChara && !this.Container.IsPC)
		{
			return !(t.id == "money") && !t.isGifted && !t.isNPCProperty && (!t.isEquipped || this.Container.Chara.affinity.CanForceTradeEquip());
		}
		return !t.trait.CanOnlyCarry && (!this.Container.isNPCProperty || !InvOwner.FreeTransfer);
	}

	public virtual bool AllowMoved(Thing t)
	{
		return true;
	}

	public virtual bool AlwaysShowTooltip
	{
		get
		{
			return false;
		}
	}

	public virtual bool UseGuide
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShouldShowGuide(Thing t)
	{
		return false;
	}

	public virtual bool AllowTransfer
	{
		get
		{
			return this.Container.isChara || !this.Container.isNPCProperty;
		}
	}

	public virtual bool AllowDropOnDrag
	{
		get
		{
			return true;
		}
	}

	public virtual bool AllowDrop(Thing t)
	{
		return !EClass._zone.IsRegion || t.trait is TraitAbility;
	}

	public virtual string langTransfer
	{
		get
		{
			return "actTransfer";
		}
	}

	public virtual int destInvY
	{
		get
		{
			return 0;
		}
	}

	public virtual Thing CreateDefaultContainer()
	{
		return ThingGen.Create("chest3", -1, -1);
	}

	public virtual bool HasTransaction
	{
		get
		{
			return false;
		}
	}

	public virtual bool CopyOnTransfer
	{
		get
		{
			return false;
		}
	}

	public virtual bool SingleTarget
	{
		get
		{
			return false;
		}
	}

	public virtual void BuildUICurrency(UICurrency uiCurrency, bool canReroll = false)
	{
		bool flag = this.Container.isChara && !this.Container.IsPC;
		uiCurrency.SetActive(this.currency > CurrencyType.None || flag);
		uiCurrency.target = this.owner;
		if (this.currency > CurrencyType.None || flag)
		{
			uiCurrency.Build(new UICurrency.Options
			{
				weight = flag,
				money = (this.currency == CurrencyType.Money),
				plat = (this.currency == CurrencyType.Plat),
				medal = (this.currency == CurrencyType.Medal),
				money2 = (this.currency == CurrencyType.Money2),
				influence = (this.currency == CurrencyType.Influence),
				casino = (this.currency == CurrencyType.Casino_coin),
				ecopo = (this.currency == CurrencyType.Ecopo)
			});
		}
	}

	public bool UseHomeResource
	{
		get
		{
			return this.homeResource != null;
		}
	}

	public bool IsMagicChest
	{
		get
		{
			return this.Container.trait is TraitMagicChest;
		}
	}

	public List<Thing> Things
	{
		get
		{
			if (!this.Container.things.HasGrid)
			{
				return this.Container.things;
			}
			return this.Container.things.grid;
		}
	}

	public Chara Chara
	{
		get
		{
			return this.owner as Chara;
		}
	}

	public ContainerType ContainerType
	{
		get
		{
			return this.Container.trait.ContainerType;
		}
	}

	public virtual bool InvertSell
	{
		get
		{
			return false;
		}
	}

	public virtual int numDragGrid
	{
		get
		{
			return 1;
		}
	}

	public virtual bool ShowNew
	{
		get
		{
			return this.owner.IsPC;
		}
	}

	public virtual bool DenyImportant
	{
		get
		{
			return true;
		}
	}

	public bool IsWeightOver(Thing t)
	{
		return false;
	}

	public InvOwner destInvOwner
	{
		get
		{
			if (!this.owner.IsPC)
			{
				return InvOwner.Main;
			}
			return InvOwner.Trader;
		}
	}

	public InvOwner(Card owner, Card container = null, CurrencyType _currency = CurrencyType.None, PriceType _price = PriceType.Default)
	{
		this.currency = _currency;
		this.priceType = _price;
		this.owner = owner;
		this.Container = (container ?? owner);
		if (this.currency == CurrencyType.BranchMoney)
		{
			this.homeResource = EClass.BranchOrHomeBranch.resources.money;
		}
	}

	public void Init()
	{
		if (this.owner == null)
		{
			CardBlueprint.SetNormalRarity(false);
			this.owner = this.CreateDefaultContainer();
			this.owner.c_lockLv = 0;
			this.owner.c_IDTState = 0;
			this.Container = (this.Container ?? this.owner);
		}
		this.OnInit();
		InvOwner.forceGive = new InvOwner.ForceGiveData();
	}

	public virtual void OnInit()
	{
	}

	public virtual void OnClick(ButtonGrid button)
	{
		Card card = button.card;
		Card card2 = button.invOwner.owner;
		if (card != null && EClass.ui.currentDrag == null)
		{
			bool flag = false;
			if (card.Thing.isEquipped && card.Thing.IsEquipmentOrRanged && card.Thing.IsCursed)
			{
				SE.Play("curse3");
			}
			else if (!this.AllowHold(card.Thing) && card2.isChara && !card2.IsPC && card2.IsPCFaction)
			{
				if (InvOwner.forceGive.card != card)
				{
					InvOwner.forceGive.card = card;
					InvOwner.forceGive.tries = 0;
				}
				else
				{
					if (!EInput.isShiftDown)
					{
						InvOwner.forceGive.tries++;
					}
					if (InvOwner.forceGive.tries >= 2)
					{
						if (card.HasTag(CTAG.gift) && card.isGifted)
						{
							EClass.ui.CloseLayers();
							card2.Say("angry", card2, null, null);
							card2.ShowEmo(Emo.angry, 0f, true);
							card2.Talk("noGiveRing", null, null, false);
							card2.Chara.ModAffinity(EClass.pc, -30, true);
							card2.Chara.InstantEat(card.Thing, true);
							return;
						}
						InvOwner.forceGive.card = null;
						bool flag2 = card.trait is TraitCurrency;
						card.isGifted = false;
						card.isNPCProperty = false;
						card2.Talk(flag2 ? "forceGiveCurrency" : "forceGive", null, null, false);
						int num = flag2 ? 3 : 1;
						if (card.id == "money")
						{
							num += card.Num / 1000;
						}
						if (num >= 5)
						{
							num = 5;
						}
						EClass.player.ModKarma(-num);
						EClass.pc.Pick(card.Thing, true, true);
						return;
					}
				}
			}
			if (this.AllowHold(card.Thing) || flag)
			{
				if (EInput.isAltDown)
				{
					if (this.CanAltClick(button))
					{
						this.OnAltClick(button);
						return;
					}
					SE.BeepSmall();
					return;
				}
				else if (EInput.isCtrlDown)
				{
					if (this.CanCtrlClick(button))
					{
						this.OnCtrlClick(button);
						return;
					}
					SE.BeepSmall();
					return;
				}
				else if (EInput.isShiftDown)
				{
					if (this.CanShiftClick(button, false))
					{
						this.OnShiftClick(button, false);
						return;
					}
					SE.BeepSmall();
					return;
				}
				else
				{
					if (!this.owner.IsPC)
					{
						new InvOwner.Transaction(button, (InvOwner.HasTrader && !InvOwner.FreeTransfer) ? 1 : card.Num, null).Process(true);
						return;
					}
					if (button.card != null && this.IsFailByCurse(button.card.Thing))
					{
						return;
					}
					EClass.ui.StartDrag(new DragItemCard(button, true));
					return;
				}
			}
			else
			{
				SE.BeepSmall();
				if (card2.isChara && !card2.IsPC)
				{
					card2.Talk("noGive", null, null, false);
				}
			}
		}
	}

	public virtual void OnRightClick(ButtonGrid button)
	{
		if (!this.AllowAutouse)
		{
			this.OnClick(button);
		}
		if (button.card == null)
		{
			return;
		}
		this.AutoUse(button, false);
	}

	public virtual void OnRightPressed(ButtonGrid button)
	{
		if (!EInput.rightMouse.pressedLong)
		{
			return;
		}
		float num = 1f;
		float pressedTimer = EInput.rightMouse.pressedTimer;
		if (pressedTimer > 2f)
		{
			num = 2f;
		}
		else if (pressedTimer > 4f)
		{
			num = 5f;
		}
		else if (pressedTimer > 6f)
		{
			num = 50f;
		}
		InvOwner.clickTimer -= Core.delta * num;
		if (InvOwner.clickTimer < 0f)
		{
			InvOwner.clickTimer = 0.1f;
			if (button.card != null)
			{
				this.AutoUse(button, true);
			}
		}
	}

	public InvOwner GetShitDestOwner(ButtonGrid b, bool rightMouse = false)
	{
		Thing thing = b.card.Thing;
		if (rightMouse && !b.invOwner.owner.IsPC)
		{
			LayerInventory pclayer = LayerInventory.GetPCLayer();
			if (pclayer == null)
			{
				return null;
			}
			return pclayer.Inv;
		}
		else if (InvOwner.Trader != null)
		{
			if (b.invOwner.owner.IsPC)
			{
				if (!InvOwner.Trader.Container.things.IsFull(thing, true, true))
				{
					return InvOwner.Trader;
				}
				return null;
			}
			else
			{
				LayerInventory topLayer = LayerInventory.GetTopLayer(thing, true, InvOwner.Trader);
				if (topLayer == null)
				{
					return null;
				}
				return topLayer.Inv;
			}
		}
		else
		{
			LayerInventory topLayer2 = LayerInventory.GetTopLayer(thing, true, this);
			if (topLayer2 == null)
			{
				return null;
			}
			return topLayer2.Inv;
		}
	}

	public virtual void OnShiftClick(ButtonGrid b, bool rightMouse = false)
	{
		InvOwner shitDestOwner = this.GetShitDestOwner(b, rightMouse);
		Thing thing = b.card.Thing;
		Card container = shitDestOwner.Container;
		if (rightMouse && !this.owner.IsPC)
		{
			EClass.pc.Pick(thing, false, true);
			return;
		}
		if ((thing.parent == container || thing == container) && !thing.IsHotItem)
		{
			if (EInput.isShiftDown)
			{
				SE.BeepSmall();
			}
			return;
		}
		thing.PlaySoundDrop(false);
		if (thing.IsHotItem && thing.parent == container)
		{
			container.RemoveCard(thing);
		}
		if (container.things.CanStack(thing, -1, -1) != thing)
		{
			container.things.TryStack(thing, -1, -1);
			return;
		}
		container.AddThing(thing, true, -1, -1);
	}

	public virtual bool CanShiftClick(ButtonGrid b, bool rightMouse = false)
	{
		if (!EInput.isShiftDown && !rightMouse)
		{
			return false;
		}
		if (!EClass.pc.HasNoGoal || this.Container.isNPCProperty || this.currency != CurrencyType.None)
		{
			return false;
		}
		if (rightMouse && !InvOwner.HasTrader)
		{
			return false;
		}
		if (InvOwner.HasTrader)
		{
			if (InvOwner.Trader is InvOwnerDraglet || InvOwner.Trader.Container.isNPCProperty || InvOwner.Trader.currency != CurrencyType.None)
			{
				return false;
			}
			if (InvOwner.Trader.Container.isChara)
			{
				return false;
			}
			Card card = b.card;
			if (card != null && card.c_isImportant)
			{
				return false;
			}
		}
		InvOwner shitDestOwner = this.GetShitDestOwner(b, rightMouse);
		if (shitDestOwner == null)
		{
			return false;
		}
		Thing thing = b.card.Thing;
		if (thing.trait.IsContainer && thing.things.Count > 0)
		{
			return false;
		}
		if (!this.AllowHold(thing))
		{
			return false;
		}
		if (InvOwner.HasTrader)
		{
			if (!InvOwner.Trader.Container.GetRootCard().IsPC && thing.trait is TraitCatalyst)
			{
				return false;
			}
			if (!InvOwner.Trader.AllowMoved(thing))
			{
				return false;
			}
		}
		if (shitDestOwner.Container.isChara && !shitDestOwner.owner.IsPC && !shitDestOwner.owner.Chara.CanAcceptItem(thing, -1))
		{
			return false;
		}
		if (b.invOwner.owner.isChara && b.invOwner.owner.Chara.IsPCFaction && !b.invOwner.owner.IsPC && thing.IsRangedWeapon)
		{
			thing.ammoData = null;
			thing.c_ammo = 0;
		}
		if (EInput.isShiftDown)
		{
			LayerInventory.highlightInv = shitDestOwner;
		}
		return true;
	}

	public virtual string GetTextShiftClick(ButtonGrid b)
	{
		return "actTransfer".lang();
	}

	public virtual void OnCtrlClick(ButtonGrid button)
	{
		button.card.Thing.ShowSplitMenu(button, this.HasTransaction ? new InvOwner.Transaction(button, 1, null) : null);
	}

	public virtual bool CanCtrlClick(ButtonGrid b)
	{
		return EInput.isCtrlDown && EClass.pc.HasNoGoal && !this.Container.isNPCProperty && b.card.Num > 1;
	}

	public virtual string GetTextCtrlClick(ButtonGrid b)
	{
		return "actSplit".lang();
	}

	public virtual void OnAltClick(ButtonGrid button)
	{
		EClass.pc.DropThing(button.card.Thing, -1);
	}

	public virtual bool CanAltClick(ButtonGrid b)
	{
		return EInput.isAltDown && EClass.pc.HasNoGoal && !this.Container.isNPCProperty && this.currency == CurrencyType.None && this.AllowDrop(b.card.Thing);
	}

	public virtual string GetTextAltClick(ButtonGrid b)
	{
		return "actDrop".lang();
	}

	public void Grab(DragItemCard.DragInfo from)
	{
		bool isEquipped = from.thing.isEquipped;
		if (isEquipped)
		{
			from.invOwner.owner.Chara.body.Unequip(from.thing, true);
		}
		from.invOwner.Container.RemoveCard(from.thing);
		if (isEquipped)
		{
			from.list.Redraw();
		}
		LayerInventory.SetDirty(from.thing);
	}

	public Thing PutBack(DragItemCard.DragInfo from)
	{
		Thing thing = from.thing;
		if (from.invY == 1 && from.container.things.IsOccupied(from.invX, from.invY))
		{
			return EClass.pc.Pick(thing, false, true);
		}
		if (from.container.things.IsFull(thing, false, false))
		{
			return EClass.pc.Pick(thing, false, true);
		}
		thing.invY = from.invY;
		thing = from.container.AddThing(thing, true, from.invX, from.invY);
		if (from.equippedSlot != null)
		{
			from.invOwner.owner.Chara.body.Equip(thing, from.equippedSlot, true);
			from.list.Redraw();
		}
		else
		{
			thing.invX = from.invX;
		}
		if (from.invY == 1)
		{
			WidgetCurrentTool.dirty = true;
		}
		return thing;
	}

	public void OnStartDrag(DragItemCard.DragInfo from)
	{
		from.thing.PlaySoundDrop(false);
		if (from.thing.parent != null)
		{
			this.Grab(from);
		}
		if (from.thing.IsEquipment)
		{
			WidgetEquip.dragEquip = from.thing;
			WidgetEquip.Redraw();
		}
	}

	public virtual bool OnCancelDrag(DragItemCard.DragInfo from)
	{
		if (this.Container.isNPCProperty)
		{
			return false;
		}
		this.PutBack(from);
		return true;
	}

	public bool OnDrag(DragItemCard.DragInfo from, DragItemCard.DragInfo to, bool execute, bool cancel = false)
	{
		if (cancel)
		{
			return this.OnCancelDrag(from);
		}
		bool flag = false;
		string text = "";
		if (EClass.ui.GetLayer<LayerRegisterHotbar>(false) == null)
		{
			EClass.ui.AddLayer<LayerRegisterHotbar>().SetItem(from.thing);
		}
		LayerInventory componentOf = InputModuleEX.GetComponentOf<LayerInventory>();
		if (!EClass.ui.isPointerOverUI && !EClass._zone.IsRegion)
		{
			flag = this.AllowDropOnDrag;
			text = ((!this.AllowDropOnDrag) ? "" : ((from.thing.trait is TraitAbility) ? "dragForget" : "dragDropThing"));
			if (execute)
			{
				if (!this.AllowDropOnDrag || from.thing.c_isImportant)
				{
					return this.OnCancelDrag(from);
				}
				EClass.pc.DropThing(from.thing, -1);
				return true;
			}
		}
		else if (!EClass.core.config.game.useGrid && componentOf)
		{
			if (to.button && to.invOwner != null && to.thing != null && to.thing.CanStackTo(from.thing))
			{
				InvOwner.Transaction transaction = new InvOwner.Transaction(from, to, from.thing.Num);
				flag = transaction.IsValid();
				text = transaction.GetTextDetail();
				if (execute)
				{
					return transaction.Process(false);
				}
			}
			else
			{
				InvOwner.Transaction transaction2 = new InvOwner.Transaction(from, componentOf.invs[0], from.thing.Num);
				flag = transaction2.IsValid();
				text = transaction2.GetTextDetail();
				if (execute)
				{
					return transaction2.Process(false);
				}
			}
		}
		else if (to.button && to.invOwner != null && to.button.interactable)
		{
			if (to.invOwner is InvOwnerEquip)
			{
				InvOwnerEquip invOwnerEquip = to.invOwner as InvOwnerEquip;
				Chara chara = invOwnerEquip.owner.Chara;
				BodySlot slot = invOwnerEquip.slot;
				if (execute && to.thing != null && to.thing.blessedState <= BlessedState.Cursed)
				{
					Msg.Say("unequipCursed", to.thing, null, null, null);
					return false;
				}
				if (from.thing.category.slot == slot.elementId)
				{
					flag = true;
					text = "dragEquip";
					if (execute)
					{
						if (!chara.body.IsEquippable(from.thing, slot, true))
						{
							return false;
						}
						chara.AddCard(from.thing);
						if (to.thing != null)
						{
							EClass.ui.nextDrag = new DragItemCard(to.button, true);
							this.Grab(to);
						}
						chara.body.Equip(from.thing, invOwnerEquip.slot, true);
						EClass.Sound.Play("equip");
						if (EClass.game.UseGrid)
						{
							from.list.Redraw();
						}
						to.list.Redraw();
						to.invOwner.OnProcess(from.thing);
						return true;
					}
				}
			}
			else
			{
				if (to.invOwner is InvOwnerCopyShop && !to.invOwner.owner.trait.CanCopy(from.thing))
				{
					return false;
				}
				if (!to.invOwner.AllowTransfer)
				{
					return false;
				}
				InvOwner.Transaction transaction3 = new InvOwner.Transaction(from, to, from.thing.Num);
				flag = transaction3.IsValid();
				text = transaction3.GetTextDetail();
				if (execute)
				{
					return transaction3.Process(false);
				}
			}
		}
		EClass.ui.hud.SetDragText(flag ? text : "", (from.thing.Num > 1) ? (from.thing.Num.ToString() ?? "") : null);
		CursorSystem.SetCursor(flag ? null : CursorSystem.Invalid, 100);
		return false;
	}

	public virtual void OnProcess(Thing t)
	{
	}

	public bool CanOpenContainer(Thing t)
	{
		if (!t.trait.CanOpenContainer)
		{
			return false;
		}
		if (t.parent != EClass.pc)
		{
			Thing thing = t.parent as Thing;
			return ((thing != null) ? thing.trait : null) is TraitToolBelt;
		}
		return true;
	}

	public InvOwner.ListInteraction ListInteractions(ButtonGrid b, bool context)
	{
		InvOwner.ListInteraction listInteraction = new InvOwner.ListInteraction();
		if (b == null || b.card == null || this is InvOwnerAlly)
		{
			return listInteraction;
		}
		Thing t = b.card.Thing;
		if (t == null)
		{
			return listInteraction;
		}
		listInteraction.thing = t;
		Trait trait = t.trait;
		bool flag = trait is TraitAbility;
		if (InvOwner.HasTrader)
		{
			if (InvOwner.Trader == null || this.destInvOwner == null)
			{
				return listInteraction;
			}
			if (this.CanOpenContainer(t))
			{
				listInteraction.Add("actContainer", 10, delegate()
				{
					(t.trait as TraitContainer).TryOpen();
				}, null);
			}
			bool flag2 = !flag && !trait.CanOnlyCarry && (!this.destInvOwner.UseGuide || this.destInvOwner.ShouldShowGuide(t));
			bool flag3 = ShopTransaction.current != null && ShopTransaction.current.CanSellBack(t, -1);
			if (this.destInvOwner == InvOwner.Trader && !InvOwner.Trader.AllowSell && !flag3)
			{
				flag2 = false;
			}
			if (!this.AllowHold(t))
			{
				flag2 = false;
			}
			if (!InvOwner.Trader.AllowMoved(t))
			{
				flag2 = false;
			}
			if (flag2 && (!t.c_isImportant || !this.destInvOwner.DenyImportant))
			{
				if (!flag3 && (InvOwner.Trader.currency == CurrencyType.None || t.GetPrice(CurrencyType.Money, false, PriceType.Default, null) == 0))
				{
					InvOwner.Transaction trans = new InvOwner.Transaction(b, t.Num, null);
					if (trans.IsValid())
					{
						Action <>9__2;
						listInteraction.Add(this.Container.isNPCProperty ? "actSteal".lang().TagColor(FontColor.Bad, SkinManager.DarkColors) : this.destInvOwner.langTransfer, 0, delegate()
						{
							if (this.Container.isNPCProperty)
							{
								Action action;
								if ((action = <>9__2) == null)
								{
									action = (<>9__2 = delegate()
									{
										trans.Process(false);
									});
								}
								Dialog.TryWarnCrime(action);
								return;
							}
							trans.Process(false);
						}, null);
					}
				}
				else
				{
					bool isShiftDown = EInput.isShiftDown;
					InvOwner.Transaction trans = new InvOwner.Transaction(b, 1, null);
					listInteraction.Add(trans.GetTextDetail(), 0, delegate()
					{
						trans.Process(false);
					}, null).repeatable = true;
					if (t.Num > 1)
					{
						InvOwner.Transaction trans2 = new InvOwner.Transaction(b, t.Num, null);
						listInteraction.Add(trans2.GetTextDetail(), 10, delegate()
						{
							trans2.Process(false);
						}, null);
						if (isShiftDown)
						{
							listInteraction.Add(listInteraction[0]);
							listInteraction.RemoveAt(0);
						}
					}
				}
			}
		}
		this.ListInteractions(listInteraction, t, trait, b, context);
		if (this.AllowHold(t) && !t.isEquipped && !InvOwner.HasTrader)
		{
			InvOwner.Interaction item = listInteraction.Add((EClass.pc.held == t) ? "actPick" : "actHold", 60, delegate()
			{
				this.TryHold(t);
			}, null);
			if (t.trait.HoldAsDefaultInteraction)
			{
				listInteraction.Remove(item);
				listInteraction.Insert(0, item);
			}
			if (t.trait.CanBeHeldAsFurniture)
			{
				listInteraction.Add("actHoldTool", 70, delegate()
				{
					if (t != EClass.pc.held)
					{
						if (this.TryHold(t))
						{
							HotItemHeld.disableTool = true;
							return;
						}
					}
					else
					{
						HotItemHeld.disableTool = true;
					}
				}, "remove");
			}
		}
		if (context)
		{
			bool flag4 = false;
			if (this.AllowHold(t) && !this.Container.isNPCProperty)
			{
				if (t.Num > 1)
				{
					listInteraction.Add("actSplit", 50, delegate()
					{
						t.ShowSplitMenu(b, (InvOwner.HasTrader && this.currency != CurrencyType.None && !this.owner.IsPC) ? new InvOwner.Transaction(b, 1, null) : null);
					}, null);
				}
				if (this.owner.IsPC && this.AllowDrop(t))
				{
					flag4 = true;
					listInteraction.Add(t.c_isImportant ? "important_off" : "important_on", 299, delegate()
					{
						t.c_isImportant = !t.c_isImportant;
						LayerInventory.SetDirty(t);
						SE.ClickOk();
					}, null);
					if (!EClass._zone.IsRegion)
					{
						listInteraction.Add(flag ? "dragForget" : "actDrop", 300, delegate()
						{
							EClass.pc.DropThing(t, -1);
						}, null);
					}
				}
			}
			if (!flag4 && t.c_isImportant)
			{
				listInteraction.Add(t.c_isImportant ? "important_off" : "important_on", 299, delegate()
				{
					t.c_isImportant = !t.c_isImportant;
					LayerInventory.SetDirty(t);
					SE.ClickOk();
				}, null);
			}
		}
		return listInteraction;
	}

	public bool TryHold(Thing t)
	{
		if (!this.AllowHold(t) || t.isEquipped || InvOwner.HasTrader)
		{
			return false;
		}
		if (EClass.pc.held == t)
		{
			if (t.trait.CanOnlyCarry)
			{
				SE.Beep();
				return true;
			}
			EClass.pc.PickHeld(false);
		}
		else
		{
			EClass.pc.HoldCard(t, -1);
		}
		EClass.player.RefreshCurrentHotItem();
		SE.SelectHotitem();
		return true;
	}

	public virtual void ListInteractions(InvOwner.ListInteraction list, Thing t, Trait trait, ButtonGrid b, bool context)
	{
		if (this.owner.IsPC)
		{
			if (this.CanOpenContainer(t) && !list.Contains("actContainer"))
			{
				list.Add(LayerInventory.IsOpen(t) ? "close" : "actContainer", 100, delegate()
				{
					(t.trait as TraitContainer).TryOpen();
				}, null);
			}
			if (!InvOwner.HasTrader)
			{
				CharaBody body = this.owner.Chara.body;
				BodySlot slot = body.GetSlot(t, false, EInput.isShiftDown);
				if (slot != null)
				{
					Card tParent = t.parentCard;
					int tInvX = t.invX;
					int tInvY = t.invY;
					if (tInvY != 1)
					{
						list.Add("invEquip", 90, delegate()
						{
							if (slot.thing != null && slot.thing.blessedState <= BlessedState.Cursed)
							{
								Msg.Say("unequipCursed", slot.thing, null, null, null);
								SE.Play("curse3");
								return;
							}
							if (EClass.pc.held == t)
							{
								EClass.pc.PickHeld(false);
							}
							Thing thing = slot.thing;
							body.Equip(t, slot, true);
							EClass.Sound.Play("equip");
							if (thing != null)
							{
								if (tParent != null)
								{
									tParent.AddThing(thing, true, -1, -1);
									thing.invX = tInvX;
									thing.invY = tInvY;
									return;
								}
								if (thing.parent is Card && (thing.parent as Card).things.IsOverflowing())
								{
									thing.parent.RemoveCard(thing);
									EClass.pc.Pick(thing, true, true);
								}
							}
						}, null);
					}
				}
				if (trait.CanRead(EClass.pc))
				{
					list.Add("invRead", 110, delegate()
					{
						t.DoAct(new AI_Read
						{
							target = t
						});
					}, "remove");
				}
				if (trait.CanUse(EClass.pc))
				{
					list.Add(trait.LangUse, 120, delegate()
					{
						if (trait.OnUse(EClass.pc))
						{
							EClass.player.EndTurn(true);
						}
					}, "use");
				}
				if (trait.CanDrink(EClass.pc))
				{
					list.Add("invDrink", 130, delegate()
					{
						t.DoAct(new AI_Drink
						{
							target = t
						});
					}, "remove");
				}
				if (trait.CanEat(EClass.pc))
				{
					list.Add("invFood", 140, delegate()
					{
						t.DoAct(new AI_Eat
						{
							cook = false,
							target = t
						});
					}, "remove");
				}
				if (trait.IsBlendBase)
				{
					list.Add("invBlend", 150, delegate()
					{
						LayerDragGrid.Create(new InvOwnerBlend(t, null, CurrencyType.None), false);
					}, "blend");
				}
				if (context)
				{
					if (trait is TraitCard)
					{
						list.Add("invCollect", 150, delegate()
						{
							ContentCodex.Collect(t);
						}, null);
					}
					if (trait.CanName)
					{
						Action<bool, string> <>9__9;
						list.Add("changeName", 200, delegate()
						{
							string langDetail = "dialogChangeName";
							string text2 = t.c_refText.IsEmpty("");
							Action<bool, string> onClose;
							if ((onClose = <>9__9) == null)
							{
								onClose = (<>9__9 = delegate(bool cancel, string text)
								{
									if (!cancel)
									{
										t.c_refText = text;
									}
								});
							}
							Dialog.InputName(langDetail, text2, onClose, Dialog.InputType.Default);
						}, null);
					}
				}
			}
		}
	}

	public virtual string GetAutoUseLang(ButtonGrid button)
	{
		if (button == null || button.gameObject == null)
		{
			return "";
		}
		InvOwner.ListInteraction list = this.ListInteractions(button, false);
		foreach (InvOwner.Interaction interaction in list)
		{
			if (EClass.player.IsPriorityAction(interaction.idPriority, list.thing))
			{
				list.Insert(0, interaction);
				break;
			}
		}
		if (button.card.trait.HoldAsDefaultInteraction && list.Count > 0)
		{
			InvOwner.Interaction interaction2 = list[0];
			if (interaction2.name == "actPick" || interaction2.name == "actHold" || interaction2.name == "actHoldTool")
			{
				return null;
			}
		}
		list.ForeachReverse(delegate(InvOwner.Interaction a)
		{
			if (a.name == "actPick" || a.name == "actHold" || a.name == "actHoldTool")
			{
				list.Remove(a);
			}
		});
		if (list.Count == 0)
		{
			return null;
		}
		if (list[0].name == "actTransfer")
		{
			return null;
		}
		return list[0].name.lang();
	}

	public void AutoUse(ButtonGrid button, bool repeat = false)
	{
		Card card = button.card;
		if (!ActionMode.Adv.IsActive && !ActionMode.Region.IsActive)
		{
			return;
		}
		if (!EClass.pc.HasNoGoal)
		{
			SE.Beep();
			return;
		}
		InvOwner.ListInteraction listInteraction = this.ListInteractions(button, false);
		if (listInteraction.Count == 0)
		{
			SE.Beep();
			return;
		}
		foreach (InvOwner.Interaction interaction in listInteraction)
		{
			if (EClass.player.IsPriorityAction(interaction.idPriority, listInteraction.thing))
			{
				listInteraction.Insert(0, interaction);
				break;
			}
		}
		if (repeat && !listInteraction[0].repeatable)
		{
			return;
		}
		listInteraction[0].action();
	}

	public void ShowContextMenu(ButtonGrid button)
	{
		if (!EClass.pc.HasNoGoal)
		{
			SE.Beep();
			return;
		}
		InvOwner.ListInteraction listInteraction = this.ListInteractions(button, true);
		if (listInteraction.Count == 0)
		{
			SE.BeepSmall();
			return;
		}
		UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
		listInteraction.Sort((InvOwner.Interaction a, InvOwner.Interaction b) => a.priority - b.priority);
		foreach (InvOwner.Interaction interaction in listInteraction)
		{
			uicontextMenu.AddButton(interaction.name, interaction.action, true);
		}
		uicontextMenu.Show();
	}

	public virtual int GetPrice(Thing t, CurrencyType currency, int num, bool sell)
	{
		return ShopTransaction.current.GetPrice(t, num, sell);
	}

	public virtual string GetTextDetail(Thing t, CurrencyType currency, int num, bool sell)
	{
		int price = this.GetPrice(t, currency, num, sell);
		string @ref = InvOwner.Trader.UseHomeResource ? InvOwner.Trader.homeResource.Name : ((currency == CurrencyType.Influence) ? "influence".lang() : EClass.sources.things.map[this.IDCurrency].GetName());
		string ref2 = (price == 0) ? "" : "invInteraction3".lang(price.ToFormat() ?? "", @ref, null, null, null);
		string text = "invInteraction1".lang(num.ToString() ?? "", ref2, (sell ? "invSell" : "invBuy").lang(), null, null);
		if (!sell && EClass.pc.GetCurrency(Currency.ToID(currency)) < price)
		{
			text = text.TagColor(FontColor.Bad, SkinManager.DarkColors);
		}
		return text;
	}

	public virtual void OnWriteNote(ButtonGrid button, UINote n)
	{
		Thing thing = button.card as Thing;
		if (thing == null)
		{
			return;
		}
		bool flag;
		if (InvOwner.HasTrader && InvOwner.Trader.currency != CurrencyType.None)
		{
			if (this.destInvOwner == InvOwner.Trader)
			{
				if (!InvOwner.Trader.AllowSell)
				{
					ShopTransaction current = ShopTransaction.current;
					flag = (current != null && current.CanSellBack(thing, -1));
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
		}
		else
		{
			flag = false;
		}
		bool flag2 = flag;
		if (flag2 && InvOwner.Trader.UseGuide && !InvOwner.Trader.ShouldShowGuide(thing))
		{
			flag2 = false;
		}
		if (flag2)
		{
			InvOwner.Transaction transaction = new InvOwner.Transaction(button, 1, null);
			n.Space(8, 1);
			UIItem uiitem = n.AddExtra<UIItem>("costPrice");
			string id = this.IDCostIcon(thing);
			int price = transaction.GetPrice();
			uiitem.text1.SetText(Lang._currency(price, false, 14), transaction.IsValid() ? FontColor.Good : FontColor.Bad);
			uiitem.image1.sprite = (InvOwner.Trader.UseHomeResource ? InvOwner.Trader.homeResource.Sprite : SpriteSheet.Get(id));
		}
	}

	public virtual string IDCostIcon(Thing t)
	{
		return "icon_" + this.IDCurrency;
	}

	public string IDCurrency
	{
		get
		{
			return Currency.ToID(InvOwner.Trader.currency);
		}
	}

	public virtual bool IsFailByCurse(Thing t)
	{
		return false;
	}

	public static InvOwner.ForceGiveData forceGive = new InvOwner.ForceGiveData();

	public static InvOwner Trader;

	public static InvOwner Main;

	public static float clickTimer;

	public bool includeChildren;

	public CurrencyType currency;

	public PriceType priceType;

	public HomeResource homeResource;

	public Card owner;

	public Card Container;

	public List<ButtonGrid> buttons = new List<ButtonGrid>();

	public class ErrorMessage
	{
		public void Set(string _lang, Card c = null, string _sound = "beep_small")
		{
			this.lang = _lang;
			if (c != null)
			{
				this.card = c;
			}
			this.sound = _sound;
		}

		public string sound = "beep_small";

		public string lang = "";

		public Card card;
	}

	public class ForceGiveData
	{
		public Card card;

		public int tries;
	}

	public class Transaction
	{
		public InvOwner destInv
		{
			get
			{
				if (this.to != null)
				{
					return this.to.invOwner;
				}
				if (this.destUIInv != null)
				{
					return this.destUIInv.owner;
				}
				if (!this.inv.owner.IsPC)
				{
					return InvOwner.Main;
				}
				return InvOwner.Trader;
			}
		}

		public bool sell
		{
			get
			{
				return (!this.destInv.InvertSell && !this.destInv.owner.IsPC) || (this.destInv.InvertSell && this.destInv.owner.IsPC);
			}
		}

		public string IDCurrency
		{
			get
			{
				return this.currency.ToString().ToLower();
			}
		}

		public CurrencyType currency
		{
			get
			{
				return InvOwner.Trader.currency;
			}
		}

		public bool FreeTrade
		{
			get
			{
				return !InvOwner.HasTrader || this.currency == CurrencyType.None || (this.to != null && this.to.invOwner.owner == EClass.pc) || (this.destUIInv && this.destUIInv.owner.owner == EClass.pc);
			}
		}

		public Transaction(DragItemCard.DragInfo from, UIInventory destUIInv, int num = 1)
		{
			this.from = from;
			this.button = from.button;
			this.thing = from.thing;
			this.num = num;
			this.inv = from.invOwner;
			this.destUIInv = destUIInv;
		}

		public Transaction(DragItemCard.DragInfo from, DragItemCard.DragInfo to, int num = 1)
		{
			this.from = from;
			this.to = to;
			this.button = from.button;
			this.thing = from.thing;
			this.num = num;
			this.inv = from.invOwner;
		}

		public Transaction(ButtonGrid button, int num = 1, InvOwner owner = null)
		{
			this.button = button;
			this.thing = (button.card as Thing);
			this.num = num;
			this.inv = (owner ?? button.invOwner);
		}

		public bool Process(bool startTransaction = false)
		{
			InvOwner.Transaction.error = new InvOwner.ErrorMessage
			{
				card = this.thing
			};
			if (!this.IsValid())
			{
				SE.Play(InvOwner.Transaction.error.sound);
				if (!InvOwner.Transaction.error.lang.IsEmpty())
				{
					Msg.Say(InvOwner.Transaction.error.lang, InvOwner.Transaction.error.card, null, null, null);
				}
				return false;
			}
			if (this.inv.IsFailByCurse(this.thing))
			{
				return false;
			}
			if (this.destInv.CopyOnTransfer)
			{
				int num = this.thing.Num;
				if (this.from != null && !(this.from.invOwner is InvOwnerAlly))
				{
					this.thing = this.from.invOwner.PutBack(this.from);
				}
				else if (this.thing.parent != this.inv.Container)
				{
					this.thing = this.inv.Container.AddThing(this.thing, true, -1, -1);
				}
				if (num != this.thing.Num)
				{
					this.thing = this.thing.Split(num);
					this.thing = EClass.pc.Pick(this.thing, false, false);
				}
				if (this.destInv.SingleTarget)
				{
					this.thing = this.thing.Split(1);
				}
				if (this.to != null)
				{
					this.to.button.SetCardGrid(this.thing, this.to.button.invOwner);
				}
				else
				{
					int currentIndex = LayerDragGrid.Instance.currentIndex;
					this.destInv.buttons[currentIndex].SetCardGrid(this.thing, this.destInv.buttons[currentIndex].invOwner);
				}
				this.destInv.OnProcess(this.thing);
				if (this.GetPrice() != 0)
				{
					SE.Play("buy");
					EClass.pc.ModCurrency(this.GetPrice() * -1, this.IDCurrency);
				}
				return true;
			}
			if (!startTransaction && this.to == null && (this.destInv.owner.IsPC ? EClass.pc : this.destInv.Container).things.IsFull(this.thing, true, true))
			{
				if (this.destInv.owner == EClass.pc)
				{
					InvOwner.Transaction.error.Set("backpack_full", null, "beep_small");
				}
				SE.Beep();
				return false;
			}
			Thing thing = this.thing.parent as Thing;
			if (thing != null && thing.isNPCProperty)
			{
				Msg.Say("steal_container", thing, this.thing, null, null);
				this.thing.isNPCProperty = false;
				EClass.player.ModKarma(-1);
				EClass.pc.pos.TryWitnessCrime(EClass.pc, null, 4, null);
			}
			DragItemCard dragItemCard = new DragItemCard(this.button, startTransaction);
			Thing thing2 = this.thing.Split(this.num);
			bool flag = false;
			if (this.FreeTrade)
			{
				flag = true;
			}
			else
			{
				SE.Play(this.sell ? "sell" : "buy");
				int price = this.GetPrice();
				if (InvOwner.Trader.UseHomeResource)
				{
					InvOwner.Trader.homeResource.Mod(this.sell ? price : (-price), true);
				}
				else
				{
					if (this.sell)
					{
						EClass.pc.ModCurrency(price, this.IDCurrency);
					}
					else
					{
						EClass.pc.ModCurrency(-price, this.IDCurrency);
					}
					ShopTransaction.current.Process(thing2, thing2.Num, this.sell);
					Msg.Say(this.sell ? "sold" : "bought", thing2, Lang._currency(Mathf.Abs(price), this.IDCurrency), null, null);
					if (thing2.id == "statue_weird" && this.sell)
					{
						EClass.pc.Say("statue_install", null, null);
					}
				}
			}
			if (this.destInv.Container.isChara && !this.destInv.owner.IsPC && !this.destInv.owner.Chara.CanAcceptItem(thing2, -1))
			{
				this.destInv.owner.Chara.Talk("tooHeavy", null, null, false);
				return false;
			}
			if (thing2.c_isImportant && !this.destInv.owner.IsPC && this.destInv.DenyImportant)
			{
				Msg.Say("markedImportant");
				return false;
			}
			if (this.inv.owner.isChara && this.inv.owner.Chara.IsPCFaction && !this.inv.owner.IsPC && thing2.IsRangedWeapon)
			{
				thing2.ammoData = null;
				thing2.c_ammo = 0;
			}
			if (startTransaction)
			{
				dragItemCard.from.thing = thing2;
				EClass.ui.StartDrag(dragItemCard);
			}
			else
			{
				Thing thing3;
				if (this.to != null)
				{
					thing2.invY = this.to.invY;
					thing3 = this.to.container.AddThing(thing2, true, this.to.invX, this.to.invY);
					if (thing3 == thing2)
					{
						if (this.to.thing != null)
						{
							if (this.FreeTrade && !this.destInv.IsMagicChest)
							{
								if (!EClass.core.config.game.doubleClickToHold || !EInput.leftMouse.down || EClass.ui.dragDuration >= 0.35f || this.from == null || !(this.from.button == this.to.button) || this.to.button.invOwner is InvOwnerHotbar)
								{
									EClass.ui.nextDrag = new DragItemCard(this.to.button, true);
									this.to.invOwner.Grab(this.to);
									flag = false;
								}
							}
							else if (this.destInv.Container != thing2.parent)
							{
								thing3 = this.destInv.Container.AddThing(thing2, true, -1, -1);
							}
						}
						this.to.grid[this.to.invX] = thing2;
						thing2.invX = this.to.invX;
						this.to.button.card = thing2;
					}
				}
				else
				{
					bool useGrid = EClass.game.UseGrid;
					if (!EClass.game.UseGrid && this.destUIInv)
					{
						RectTransform rectTransform = new GameObject().AddComponent<RectTransform>();
						rectTransform.SetParent(this.destUIInv.list.transform);
						rectTransform.SetAnchor(0f, 0f, 0f, 0f);
						rectTransform.localScale = Vector3.one;
						rectTransform.sizeDelta = Vector3.one;
						rectTransform.position = Input.mousePosition;
						this.from.thing.posInvX = (int)rectTransform.anchoredPosition.x;
						this.from.thing.posInvY = (int)rectTransform.anchoredPosition.y;
					}
					if (this.destInv.owner.IsPC)
					{
						thing3 = EClass.pc.Pick(thing2, false, true);
					}
					else
					{
						thing3 = this.destInv.Container.AddThing(thing2, useGrid, -1, -1);
					}
				}
				this.destInv.OnProcess(thing3);
			}
			if (this.destInv.Container.trait is TraitDeliveryChest)
			{
				thing2.SetInt(102, EClass._zone.uid);
			}
			else
			{
				thing2.SetInt(102, 0);
			}
			Chara chara = this.destInv.Container.Chara;
			if (chara != null && !chara.IsPC)
			{
				chara.TryEquip(thing2, false);
				if (thing2.id == "lovepotion" || thing2.id == "dreambug")
				{
					EClass.pc.GiveLovePotion(chara, thing2);
					EClass.ui.CloseLayers();
					return true;
				}
			}
			if (thing2.id == "statue_weird")
			{
				if (!this.destInv.owner.IsPC)
				{
					EClass.pc.Say("statue_sell", null, null);
				}
				else if (thing == null || thing.GetRootCard() != EClass.pc)
				{
					EClass.pc.Say("statue_pick", null, null);
				}
			}
			if (flag)
			{
				SE.Drop();
			}
			return true;
		}

		public int GetPrice()
		{
			if (InvOwner.Trader != null)
			{
				return InvOwner.Trader.GetPrice(this.thing, this.currency, this.num, this.sell);
			}
			return 0;
		}

		public bool IsValid()
		{
			if (this.destInv is InvOwnerToolbelt)
			{
				return false;
			}
			if (this.destInv.UseGuide && !this.destInv.ShouldShowGuide(this.thing))
			{
				return false;
			}
			if (this.destInv.Container.c_lockLv != 0)
			{
				return false;
			}
			if (this.thing.trait is TraitAbility && this.destInv.owner != EClass.pc)
			{
				return false;
			}
			if (this.destInv.CopyOnTransfer && this.to != null && this.to.button.index != LayerDragGrid.Instance.currentIndex)
			{
				return false;
			}
			if (!this.destInv.AllowTransfer)
			{
				return false;
			}
			if (this.thing.trait.IsContainer && this.destInv.Container != EClass.pc && !(this.destInv.Container.trait is TraitToolBelt) && this.thing.things.Count > 0 && (!this.destInv.CopyOnTransfer || !this.destInv.ShouldShowGuide(this.thing)) && (this.destInv != InvOwner.Trader || ShopTransaction.current == null || !ShopTransaction.current.HasBought(this.thing)))
			{
				InvOwner.Transaction.error.Set("errorUnemptyContainer", null, "beep_small");
				return false;
			}
			if (InvOwner.HasTrader && InvOwner.Trader.currency != CurrencyType.None)
			{
				int price = this.GetPrice();
				if (this.sell)
				{
					if (this.destInv.destInvY == 0 && this.destInv.Container.things.IsFull(this.thing, true, true))
					{
						return false;
					}
					if (this.thing.c_isImportant)
					{
						return false;
					}
					if (ShopTransaction.current == null || !ShopTransaction.current.CanSellBack(this.thing, -1))
					{
						if (!InvOwner.Trader.AllowSell)
						{
							return false;
						}
						if (!this.FreeTrade && price == 0)
						{
							return false;
						}
					}
				}
				else if (!this.FreeTrade)
				{
					if (InvOwner.Trader.UseHomeResource)
					{
						if (InvOwner.Trader.homeResource.value < price)
						{
							return false;
						}
					}
					else if (EClass.pc.GetCurrency(this.IDCurrency) < price)
					{
						return false;
					}
				}
			}
			if (this.to != null)
			{
				if (this.to.invOwner.IsMagicChest && this.to.container.things.IsFull(0))
				{
					return false;
				}
				if (this.to.thing != null && !this.to.thing.isDestroyed && !this.destInv.AllowHold(this.to.thing))
				{
					return false;
				}
			}
			if (this.destInv.owner.IsPC && !this.destInv.Container.IsPC && this.destInv.IsWeightOver(this.thing))
			{
				InvOwner.Transaction.error.Set("errorOverweight", null, "beep_small");
				return false;
			}
			return true;
		}

		public string GetTextDetail()
		{
			if (this.FreeTrade || (this.destInv.UseGuide && !this.destInv.ShouldShowGuide(this.thing)))
			{
				return "";
			}
			return this.destInv.GetTextDetail(this.thing, this.currency, this.num, this.sell);
		}

		public static InvOwner.ErrorMessage error = new InvOwner.ErrorMessage();

		public Thing thing;

		public ButtonGrid button;

		public int num = 1;

		public DragItemCard.DragInfo from;

		public DragItemCard.DragInfo to;

		public UIInventory destUIInv;

		public InvOwner inv;
	}

	public class Interaction
	{
		public string name;

		public string idPriority;

		public Action action;

		public int priority;

		public bool repeatable;
	}

	public class ListInteraction : List<InvOwner.Interaction>
	{
		public InvOwner.Interaction Add(string s, int priority, Action action, string idPriority = null)
		{
			InvOwner.Interaction interaction = new InvOwner.Interaction
			{
				name = s,
				priority = priority,
				idPriority = idPriority,
				action = delegate()
				{
					if (!idPriority.IsEmpty())
					{
						if (idPriority == "remove")
						{
							EClass.player.SetPriorityAction(null, this.thing);
						}
						else
						{
							EClass.player.SetPriorityAction(idPriority, this.thing);
						}
					}
					action();
				}
			};
			base.Add(interaction);
			return interaction;
		}

		public bool Contains(string s)
		{
			using (List<InvOwner.Interaction>.Enumerator enumerator = base.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.name == s)
					{
						return true;
					}
				}
			}
			return false;
		}

		public Thing thing;
	}
}
