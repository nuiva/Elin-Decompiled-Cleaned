using System;
using System.Collections.Generic;
using UnityEngine;

public class LayerInventory : ELayer
{
	public static LayerInventory GetTopLayer(Thing t, bool includePlayer = false, InvOwner exclude = null)
	{
		LayerInventory result = null;
		int num = -1;
		foreach (LayerInventory layerInventory in LayerInventory.listInv)
		{
			if (layerInventory.IsPlayerContainer(includePlayer) && layerInventory.Inv != exclude && !layerInventory.Inv.Container.IsToolbelt && !layerInventory.Inv.Container.things.IsFull(t, false, true))
			{
				int siblingIndex = layerInventory.transform.GetSiblingIndex();
				if (siblingIndex > num)
				{
					num = siblingIndex;
					result = layerInventory;
				}
			}
		}
		return result;
	}

	public static LayerInventory GetPCLayer()
	{
		LayerInventory result = null;
		foreach (LayerInventory layerInventory in LayerInventory.listInv)
		{
			if (layerInventory.Inv.Container.IsPC)
			{
				return layerInventory;
			}
		}
		return result;
	}

	public static void Close(Thing t)
	{
		foreach (LayerInventory layerInventory in LayerInventory.listInv)
		{
			if (layerInventory.invs[0].owner.Container == t)
			{
				layerInventory.Close();
				break;
			}
		}
	}

	public static bool IsOpen(Thing t)
	{
		using (List<LayerInventory>.Enumerator enumerator = LayerInventory.listInv.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.invs[0].owner.Container == t)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void SetDirty(Thing t)
	{
		if (!ELayer.game.altInv || t == null)
		{
			return;
		}
		foreach (LayerInventory layerInventory in LayerInventory.listInv)
		{
			if (layerInventory.invs[0].owner.Container == t.parent || (layerInventory.mini && layerInventory.mini.gameObject.activeInHierarchy))
			{
				layerInventory.invs[0].dirty = true;
			}
		}
		if (t.invY == 1 || ELayer.pc.held == t)
		{
			WidgetCurrentTool.dirty = true;
		}
		if (t.isEquipped)
		{
			WidgetEquip.dirty = true;
		}
	}

	public static void SetDirtyAll(bool immediate = false)
	{
		foreach (LayerInventory layerInventory in LayerInventory.listInv)
		{
			layerInventory.invs[0].dirty = true;
			if (immediate)
			{
				layerInventory.invs[0].CheckDirty();
			}
		}
		if (WidgetEquip.Instance)
		{
			WidgetEquip.dirty = true;
			if (immediate)
			{
				WidgetEquip.Instance.CheckDirty();
			}
		}
		WidgetCurrentTool.dirty = true;
		if (immediate)
		{
			WidgetCurrentTool.Instance.CheckDirty();
		}
	}

	public static void TryShowGuide(UIList list)
	{
		List<ButtonGrid> list2 = new List<ButtonGrid>();
		foreach (UIList.ButtonPair buttonPair in list.buttons)
		{
			ButtonGrid buttonGrid = buttonPair.component as ButtonGrid;
			if (buttonGrid)
			{
				list2.Add(buttonGrid);
			}
		}
		LayerInventory.TryShowGuide(list2);
	}

	public static void TryShowGuide(List<ButtonGrid> list)
	{
		bool flag = InvOwner.HasTrader && InvOwner.Trader.UseGuide;
		bool flag2 = WidgetSearch.Instance && WidgetSearch.selected != null;
		if (!flag2 && WidgetEquip.dragEquip == null && LayerAbility.hotElement == null && !flag)
		{
			return;
		}
		foreach (ButtonGrid buttonGrid in list)
		{
			Thing thing = buttonGrid.card as Thing;
			if (LayerAbility.hotElement != null)
			{
				if (buttonGrid && buttonGrid.invOwner != null && (thing == null || thing.trait is TraitAbility) && buttonGrid.invOwner.owner == ELayer.pc && !(buttonGrid.invOwner is InvOwnerEquip))
				{
					buttonGrid.Attach("guide", false);
				}
			}
			else if (WidgetEquip.dragEquip != null && !flag)
			{
				InvOwnerEquip invOwnerEquip = buttonGrid.invOwner as InvOwnerEquip;
				if (invOwnerEquip != null && invOwnerEquip.slot.elementId == WidgetEquip.dragEquip.category.slot)
				{
					buttonGrid.Attach("guide", false);
				}
			}
			else if (flag2)
			{
				if (buttonGrid.card == WidgetSearch.selected || buttonGrid.card == WidgetSearch.selected.parent)
				{
					buttonGrid.Attach("guide", false);
				}
			}
			else if (thing != null)
			{
				bool show = InvOwner.Trader.ShouldShowGuide(thing);
				if (!show && thing.CanSearchContents)
				{
					thing.things.Foreach(delegate(Thing _t)
					{
						if (!show && InvOwner.Trader.ShouldShowGuide(_t))
						{
							show = true;
						}
					}, true);
				}
				if (show)
				{
					buttonGrid.Attach("guide", false);
				}
			}
		}
	}

	public InvOwner Inv
	{
		get
		{
			return this.invs[0].tabs[0].owner;
		}
	}

	public bool IsPlayerContainer(bool includePlayer = false)
	{
		return this.invs[0].tabs[0].mode == UIInventory.Mode.All && (includePlayer || this.invs[0].owner.Container != ELayer.pc) && this.invs[0].owner.Container.GetRootCard() == ELayer.pc;
	}

	public Card GetPlayerContainer()
	{
		UIInventory.Tab tab = this.invs[0].tabs[0];
		if (!this.IsPlayerContainer(false))
		{
			return null;
		}
		return tab.owner.Container;
	}

	public override bool HeaderIsListOf(int id)
	{
		return false;
	}

	public override void OnInit()
	{
		foreach (UIInventory uiinventory in this.invs)
		{
			uiinventory.OnInit();
		}
		LayerInventory.listInv.Add(this);
		if (this.Inv.Container == ELayer.pc)
		{
			InvOwner.Main = this.Inv;
		}
		if (!this.floatInv)
		{
			InvOwner.Trader = this.Inv;
			this.wasInventoryOpen = ELayer.ui.IsInventoryOpen;
			if (!this.wasInventoryOpen)
			{
				ELayer.ui.OpenFloatInv(true);
			}
		}
	}

	public override void OnAfterInit()
	{
		ELayer.core.actionsNextFrame.Add(delegate
		{
			ELayer.core.actionsNextFrame.Add(delegate
			{
				if (this.invs[0] && this.invs[0].gameObject)
				{
					this.invs[0].RefreshHighlight();
				}
			});
		});
	}

	public UIInventory SetInv(int idWindow = 0)
	{
		UIInventory uiinventory = this.invs[idWindow];
		uiinventory.window = this.windows[idWindow];
		uiinventory.layer = this;
		return uiinventory;
	}

	public override void OnUpdateInput()
	{
		if (EInput.action == EAction.MenuInventory || Input.GetKeyDown(KeyCode.Tab))
		{
			this.Close();
			EInput.WaitReleaseKey();
			return;
		}
		if (EInput.action == EAction.Dump)
		{
			TaskDump.TryPerform();
		}
		base.OnUpdateInput();
	}

	public override void OnKill()
	{
		LayerInventory.listInv.Remove(this);
		EInput.haltInput = false;
		if (this.Inv == InvOwner.Trader)
		{
			if (this.Inv.UseGuide)
			{
				LayerInventory.SetDirtyAll(false);
			}
			InvOwner.Trader = null;
			if (!this.wasInventoryOpen && ELayer.ui.IsInventoryOpen)
			{
				ELayer.ui.ToggleInventory(false);
			}
		}
		if (this.Inv.Container == ELayer.pc)
		{
			InvOwner.Main = null;
		}
		if (!ELayer.game.isKilling && this.Inv.owner == ELayer.pc)
		{
			SE.Play("pop_inventory_deactivate");
		}
	}

	private void OnDestroy()
	{
		if (this.invs.Count > 0)
		{
			LayerInventory.SetDirty(this.invs[0].owner.Container.Thing);
		}
		LayerInventory.listInv.Remove(this);
	}

	public override void OnRightClick()
	{
		if (this.invs[0].isList)
		{
			base.OnRightClick();
		}
		if (!this.invs[0].floatMode && InputModuleEX.GetComponentOf<ButtonGrid>() == null)
		{
			this.Close();
		}
	}

	public static LayerInventory _Create(string path = "")
	{
		if (ELayer.game.altInv && path.IsEmpty())
		{
			path = "LayerInventoryGrid";
		}
		return Layer.Create(path.IsEmpty() ? "LayerInventory" : ("LayerInventory/" + path)) as LayerInventory;
	}

	public static LayerInventory CreatePCBackpack(bool mousePos = false)
	{
		LayerInventory layerInventory = LayerInventory._Create("LayerInventoryFloatMain");
		Window window = layerInventory.windows[0];
		layerInventory.mainInv = true;
		window.setting.saveWindow = true;
		UIInventory uiinventory = layerInventory.SetInv(0);
		uiinventory.AddTab(new InvOwner(ELayer.pc, null, CurrencyType.None, PriceType.Default), UIInventory.Mode.All);
		uiinventory.SetHeader("stash");
		uiinventory.floatMode = true;
		return layerInventory;
	}

	public static bool CloseAllyInv(Chara c)
	{
		foreach (LayerInventory layerInventory in LayerInventory.listInv.Copy<LayerInventory>())
		{
			if (layerInventory.Inv.owner == c)
			{
				ELayer.ui.layerFloat.RemoveLayer(layerInventory);
				return true;
			}
		}
		return false;
	}

	public static void CloseAllyInv()
	{
		foreach (LayerInventory layerInventory in LayerInventory.listInv.Copy<LayerInventory>())
		{
			if (!layerInventory.IsPlayerContainer(true))
			{
				ELayer.ui.layerFloat.RemoveLayer(layerInventory);
			}
		}
	}

	public static LayerInventory CreateContainerAlly(Chara owner, Card container)
	{
		LayerInventory.SetDirty(container.Thing);
		LayerInventory.CloseAllyInv();
		LayerInventory layerInventory = LayerInventory._Create("LayerInventoryFloat");
		UIInventory uiinventory = layerInventory.SetInv(0);
		Window window = layerInventory.windows[0];
		window.buttonClose.SetActive(true);
		uiinventory.AddTab(new InvOwnerAlly(owner, container.Thing, CurrencyType.Money), UIInventory.Mode.All);
		uiinventory.tabs[0].textTab = container.Name;
		uiinventory.floatMode = true;
		if (ELayer.player.windowAllyInv == null)
		{
			Vector2 sizeDelta = window.Rect().sizeDelta;
			ELayer.player.windowAllyInv = new Window.SaveData
			{
				x = 0f,
				y = 200f,
				w = sizeDelta.x,
				h = sizeDelta.y,
				anchor = RectPosition.Center,
				useBG = ELayer.core.config.game.showInvBG,
				advDistribution = false
			};
		}
		window.saveData = ELayer.player.windowAllyInv;
		window.saveData.open = true;
		ELayer.ui.layerFloat.AddLayer(layerInventory);
		window.ClampToScreen();
		window.UpdateSaveData();
		return layerInventory;
	}

	public static bool IsOpen(Card container)
	{
		using (List<LayerInventory>.Enumerator enumerator = LayerInventory.listInv.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.GetPlayerContainer() == container)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static LayerInventory CreateContainerPC(Card container)
	{
		LayerInventory.SetDirty(container.Thing);
		foreach (LayerInventory layerInventory in LayerInventory.listInv)
		{
			if (layerInventory.GetPlayerContainer() == container)
			{
				ELayer.ui.layerFloat.RemoveLayer(layerInventory);
				return null;
			}
		}
		LayerInventory layerInventory2 = LayerInventory._Create("LayerInventoryFloat");
		UIInventory uiinventory = layerInventory2.SetInv(0);
		Window window = layerInventory2.windows[0];
		Vector2 vector = default(Vector2);
		bool flag = container.c_windowSaveData == null;
		window.buttonClose.SetActive(true);
		uiinventory.AddTab(new InvOwner(ELayer.pc, container.Thing, CurrencyType.None, PriceType.Default), UIInventory.Mode.All);
		uiinventory.tabs[0].textTab = container.Name;
		uiinventory.floatMode = true;
		if (container.c_windowSaveData == null)
		{
			vector = window.Rect().anchoredPosition + new Vector2(-80f, -80f);
			Vector2 sizeDelta = window.Rect().sizeDelta;
			container.c_windowSaveData = new Window.SaveData
			{
				x = vector.x,
				y = vector.y,
				w = sizeDelta.x,
				h = sizeDelta.y,
				anchor = RectPosition.Auto,
				useBG = ELayer.core.config.game.showInvBG,
				advDistribution = false
			};
			if (container.trait.IsFridge)
			{
				container.c_windowSaveData.onlyRottable = true;
			}
		}
		if (container.IsToolbelt)
		{
			container.c_windowSaveData.useBG = false;
		}
		window.saveData = container.c_windowSaveData;
		window.saveData.open = true;
		if (container.IsToolbelt)
		{
			return layerInventory2;
		}
		ELayer.ui.layerFloat.AddLayer(layerInventory2);
		if (flag)
		{
			RectTransform rectTransform = window.Rect();
			if (ELayer.player.openContainerCenter)
			{
				rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
				rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
				vector = new Vector2(0f, rectTransform.sizeDelta.y / 2f);
			}
			else
			{
				RectTransform rectTransform2 = null;
				RectTransform rectTransform3 = null;
				foreach (LayerInventory layerInventory3 in LayerInventory.listInv)
				{
					if (!(layerInventory3 == layerInventory2) && layerInventory3.IsFloat)
					{
						RectTransform rectTransform4 = layerInventory3.windows[0].Rect();
						if (!rectTransform3 || rectTransform4.Rect().localPosition.x < rectTransform3.Rect().localPosition.x)
						{
							rectTransform3 = rectTransform4;
						}
						if (!rectTransform2 || rectTransform4.Rect().localPosition.y > rectTransform2.Rect().localPosition.y)
						{
							rectTransform2 = rectTransform4;
						}
					}
				}
				if (uiinventory.tabs[0].owner.Container.things.width < 3 && rectTransform3)
				{
					rectTransform.anchorMin = rectTransform3.anchorMin;
					rectTransform.anchorMax = rectTransform3.anchorMax;
					vector.x = rectTransform3.anchoredPosition.x - rectTransform.sizeDelta.x - rectTransform3.sizeDelta.x * 0.5f + 35f;
					vector.y = rectTransform3.anchoredPosition.y;
				}
				else if (rectTransform2)
				{
					rectTransform.anchorMin = rectTransform2.anchorMin;
					rectTransform.anchorMax = rectTransform2.anchorMax;
					vector.x = rectTransform2.anchoredPosition.x;
					vector.y = rectTransform2.anchoredPosition.y + rectTransform.sizeDelta.y - 25f;
				}
				else
				{
					rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
					rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
					vector = Vector2.one;
				}
			}
			rectTransform.anchoredPosition = vector;
			window.ClampToScreen();
			window.UpdateSaveData();
		}
		return layerInventory2;
	}

	public static LayerInventory CreateContainer(Card owner)
	{
		if (owner.GetRootCard() == ELayer.pc)
		{
			return LayerInventory.CreateContainerPC(owner);
		}
		Card container = owner;
		if (owner.trait is TraitShippingChest)
		{
			container = ELayer.game.cards.container_shipping;
			ELayer.player.uidLastShippedZone = ELayer._zone.uid;
		}
		if (owner.trait is TraitDeliveryChest)
		{
			container = ELayer.game.cards.container_deliver;
			Tutorial.Play("deliver_box");
		}
		return LayerInventory.CreateContainer(owner, container);
	}

	public static LayerInventory CreateContainer(Card owner, Card container)
	{
		if (container.isChara)
		{
			SE.PopInventory();
		}
		LayerInventory layerInventory = LayerInventory._Create("");
		layerInventory.SetInv(0).AddTab(owner, UIInventory.Mode.Take, container.Thing).dest = ELayer.pc;
		if (container.c_windowSaveData == null)
		{
			container.c_windowSaveData = new Window.SaveData
			{
				useBG = true
			};
			if (container == ELayer.game.cards.container_shipping)
			{
				container.c_windowSaveData.autodump = AutodumpFlag.none;
			}
		}
		layerInventory.windows[0].saveData = container.c_windowSaveData;
		ELayer.ui.AddLayer(layerInventory);
		return layerInventory;
	}

	public static LayerInventory CreateContainer<T>(Card c, Card container, CurrencyType currency = CurrencyType.None) where T : InvOwner
	{
		LayerInventory layerInventory = LayerInventory._Create("");
		UIInventory uiinventory = layerInventory.SetInv(0);
		T t = Activator.CreateInstance(typeof(T), new object[]
		{
			c,
			container,
			currency
		}) as T;
		uiinventory.AddTab(t, UIInventory.Mode.Buy).dest = ELayer.pc;
		if (container.c_windowSaveData == null)
		{
			container.c_windowSaveData = new Window.SaveData
			{
				useBG = true
			};
		}
		layerInventory.windows[0].saveData = container.c_windowSaveData;
		ELayer.ui.AddLayer(layerInventory);
		return layerInventory;
	}

	public static LayerInventory CreateBuy(Card c, CurrencyType currency = CurrencyType.Money, PriceType price = PriceType.Default)
	{
		LayerInventory layerInventory = LayerInventory._Create("");
		UIInventory uiinventory = layerInventory.SetInv(0);
		Thing thing = c.things.Find("chest_merchant", -1, -1);
		SE.Play("shop_open");
		InvOwnerShop invOwnerShop = new InvOwnerShop(c, thing, currency, price);
		uiinventory.AddTab(invOwnerShop, UIInventory.Mode.Buy).dest = ELayer.pc;
		if (Window.dictData.TryGetValue("ChestMerchant", null) == null)
		{
			Window.dictData.Add("ChestMerchant", new Window.SaveData
			{
				useBG = true
			});
		}
		layerInventory.windows[0].saveData = thing.GetWindowSaveData();
		uiinventory.tabs[0].owner.BuildUICurrency(layerInventory.uiCurrency, c.trait.CostRerollShop != 0);
		ShopTransaction.current = new ShopTransaction
		{
			trader = invOwnerShop
		};
		layerInventory.SetOnKill(new Action(ShopTransaction.current.OnEndTransaction));
		return layerInventory;
	}

	public static List<LayerInventory> listInv = new List<LayerInventory>();

	public static InvOwner highlightInv;

	public List<UIInventory> invs = new List<UIInventory>();

	public UICurrency uiCurrency;

	public bool floatInv;

	public bool mainInv;

	public WindowCharaMini mini;

	[NonSerialized]
	public bool wasInventoryOpen;
}
