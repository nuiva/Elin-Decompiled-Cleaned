using System;
using System.Collections.Generic;
using UnityEngine;

public class LayerAbility : ELayer, IDragParent
{
	public new LayerAbility.Config config
	{
		get
		{
			return ELayer.player.layerAbilityConfig;
		}
	}

	public override void OnInit()
	{
		this.chara = ELayer.pc;
		foreach (string str in this.idGroup)
		{
			this.windows[0].AddTab("cat_" + str, this.windows[0].CurrentContent, null, null, null);
		}
		LayerAbility.Instance = this;
		this.list.sortMode = ELayer.player.pref.sortAbility;
		if (ELayer.game.altAbility)
		{
			UIButton buttonSort = this.windows[0].buttonSort;
			buttonSort.onClick.RemoveAllListeners();
			buttonSort.onClick.AddListener(delegate()
			{
				UIContextMenu uicontextMenu = ELayer.ui.CreateContextMenuInteraction();
				uicontextMenu.layoutGroup.childAlignment = TextAnchor.UpperLeft;
				uicontextMenu.alwaysPopLeft = true;
				UIContextMenu uicontextMenu2 = uicontextMenu.AddChild("sort", TextAnchor.UpperRight);
				UIList.SortMode[] sorts = this.list.sorts;
				for (int j = 0; j < sorts.Length; j++)
				{
					UIList.SortMode sortMode = sorts[j];
					string str2 = (this.list.sortMode == sortMode) ? "context_checker".lang() : "";
					UIList.SortMode _sort = sortMode;
					uicontextMenu2.AddButton(str2 + sortMode.ToString().lang(), delegate()
					{
						this.list.List(_sort);
						SE.Click();
					}, true);
				}
				UIContextMenu uicontextMenu3 = uicontextMenu.AddChild("config", TextAnchor.UpperRight);
				uicontextMenu3.AddSlider("alpha", (float a) => a.ToString() ?? "", (float)this.config.bgAlpha, delegate(float b)
				{
					this.config.bgAlpha = (int)b;
					this.RefreshConfig();
				}, 0f, 100f, true, false, false);
				uicontextMenu3.AddToggle("autoHideBG", this.config.autoHideBG, delegate(bool a)
				{
					this.config.autoHideBG = a;
					this.RefreshConfig();
				});
				uicontextMenu3.AddToggle("hideDepletedSpell", this.config.hideDepletedSpell, delegate(bool a)
				{
					this.config.hideDepletedSpell = a;
					this.list.List();
				});
				uicontextMenu.Show();
			});
		}
		this.windows[0].idTab = ELayer.player.pref.lastIdTabAbility;
		this.RefreshConfig();
	}

	public void RefreshConfig()
	{
		Window window = this.windows[0];
		window.imageBG.color = Color.white.SetAlpha(0.01f * (float)this.config.bgAlpha);
		window.listCgFloat.Clear();
		if (this.config.autoHideBG)
		{
			window.listCgFloat.Add(window.cgBG);
		}
		window.cgBG.enabled = this.config.autoHideBG;
	}

	public static void SetDirty(Element a)
	{
		if (LayerAbility.Instance)
		{
			LayerAbility.Instance.list.Redraw();
		}
		foreach (LayerInventory layerInventory in LayerInventory.listInv)
		{
			foreach (UIList.ButtonPair buttonPair in layerInventory.invs[0].list.buttons)
			{
				ButtonGrid buttonGrid = buttonPair.component as ButtonGrid;
				if (buttonGrid.card != null && buttonGrid.card.trait is TraitAbility)
				{
					layerInventory.invs[0].list.callbacks.OnRedraw(buttonGrid.card, buttonGrid, buttonGrid.index);
				}
			}
		}
		if (WidgetCurrentTool.Instance)
		{
			foreach (UIList.ButtonPair buttonPair2 in WidgetCurrentTool.Instance.list.buttons)
			{
				ButtonGrid buttonGrid2 = buttonPair2.component as ButtonGrid;
				if (buttonGrid2.card != null && buttonGrid2.card.trait is TraitAbility)
				{
					WidgetCurrentTool.Instance.list.callbacks.OnRedraw(buttonGrid2.card, buttonGrid2, buttonGrid2.index);
				}
			}
			WidgetCurrentTool.RefreshCurrentHotItem();
		}
	}

	public override void OnSwitchContent(Window window)
	{
		ELayer.player.pref.lastIdTabAbility = window.idTab;
		this.SelectGroup(this.windows[0].setting.tabs[window.idTab].idLang.Replace("cat_", ""));
	}

	public override void OnUpdateInput()
	{
		if (EInput.middleMouse.down)
		{
			ButtonAbility componentOf = InputModuleEX.GetComponentOf<ButtonAbility>();
			if (componentOf)
			{
				ELayer.player.SetCurrentHotItem(new HotItemAct(componentOf.source));
				SE.SelectHotitem();
				return;
			}
		}
		base.OnUpdateInput();
	}

	public static void ClearHotElement()
	{
		ButtonAbility buttonAbility = LayerAbility.hotElement;
		LayerAbility.hotElement = null;
		LayerInventory.SetDirtyAll(false);
		if (buttonAbility && buttonAbility.attach != null)
		{
			UnityEngine.Object.Destroy(buttonAbility.attach.gameObject);
		}
		EInput.Consume(false, 1);
		ELayer.ui.hud.SetDragImage(null, null, null);
	}

	public void SelectGroup(string id)
	{
		this.list.Clear();
		BaseList baseList = this.list;
		UIList.Callback<Element, ButtonAbility> callback = new UIList.Callback<Element, ButtonAbility>();
		callback.onClick = delegate(Element a, ButtonAbility b)
		{
			if (ELayer.ui.IsActive)
			{
				SE.BeepSmall();
				return;
			}
			SE.Equip();
			LayerAbility.hotElement = b;
			b.attach = Util.Instantiate("UI/Element/Grid/Attach/guide_ability", b.transform);
			LayerInventory.SetDirtyAll(false);
			ELayer.ui.hud.SetDragImage(b.icon, null, null);
		};
		callback.onRedraw = delegate(Element a, ButtonAbility b, int i)
		{
			b.dragParent = this;
			b.SetAct(this.chara, a);
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			List<Element> list = new List<Element>();
			foreach (Element element in ELayer.pc.elements.dict.Values)
			{
				string categorySub = element.source.categorySub;
				if (element.Value != 0 && (!this.config.hideDepletedSpell || !element.PotentialAsStock || element.vPotential > 0))
				{
					if (id == "favAbility" && ELayer.player.favAbility.Contains(element.id))
					{
						list.Add(element);
					}
					else if (id == categorySub || (id == "all" && this.idGroup.Contains(categorySub)))
					{
						list.Add(element);
					}
				}
			}
			list.Sort((Element a, Element b) => a.GetSortVal(m) - b.GetSortVal(m));
			foreach (Element o in list)
			{
				this.list.Add(o);
			}
		};
		baseList.callbacks = callback;
		this.list.List();
	}

	public static void Redraw()
	{
		if (LayerAbility.Instance)
		{
			LayerAbility.Instance._Redraw();
		}
	}

	public void _Redraw()
	{
		this.list.List();
	}

	public void OnStartDrag(UIButton b)
	{
		ELayer.ui.hud.SetDragImage(b.icon, null, null);
	}

	public void OnDrag(UIButton b)
	{
		ButtonAbility buttonAbility = b as ButtonAbility;
		string text = "";
		LayerRegisterHotbar layer = ELayer.ui.GetLayer<LayerRegisterHotbar>(false);
		if (((layer != null) ? layer.GetButton() : null) ?? null)
		{
			text = "hotitemRegister";
		}
		else if (!layer)
		{
			ELayer.ui.AddLayer<LayerRegisterHotbar>().SetItem(buttonAbility.source);
		}
		ELayer.ui.hud.SetDragText(text, null);
	}

	public void OnEndDrag(UIButton b, bool cancel = false)
	{
		ELayer.ui.hud.imageDrag.SetActive(false);
		SourceElement.Row source = (b as ButtonAbility).source;
		LayerRegisterHotbar layer = ELayer.ui.GetLayer<LayerRegisterHotbar>(false);
		if (layer)
		{
			layer.OnEndDrag();
			ELayer.ui.RemoveLayer<LayerRegisterHotbar>();
			return;
		}
	}

	public void OnDisable()
	{
		ELayer.player.pref.sortAbility = this.list.sortMode;
		ELayer.ui.hud.imageDrag.SetActive(false);
		ELayer.ui.RemoveLayer<LayerRegisterHotbar>();
	}

	public static LayerAbility Instance;

	public static ButtonAbility hotElement;

	public UIDynamicList list;

	public Chara chara;

	public Transform headerRow;

	[NonSerialized]
	public string[] idGroup = new string[]
	{
		"all",
		"attack",
		"defense",
		"util",
		"ability",
		"favAbility"
	};

	[Serializable]
	public class Config
	{
		public bool autoHideBG;

		public bool hideDepletedSpell;

		public int bgAlpha;
	}
}
