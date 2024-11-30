using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerDragGrid : LayerBaseCraft
{
	public ButtonGrid CurrentButton
	{
		get
		{
			return this.buttons[this.currentIndex];
		}
	}

	public override bool CanCancelAI
	{
		get
		{
			return true;
		}
	}

	public override bool RepeatAI
	{
		get
		{
			return true;
		}
	}

	public void OnOpen()
	{
		LayerDragGrid.Instance = this;
		InvOwner.Trader = this.owner;
		this.wasInventoryOpen = ELayer.ui.IsInventoryOpen;
		if (!this.wasInventoryOpen)
		{
			ELayer.ui.OpenFloatInv(true);
		}
	}

	public LayerDragGrid SetInv(InvOwnerDraglet owner, bool refuelFromLayerDragGrid = false)
	{
		owner.Init();
		owner.owner.SetImage(this.imageOwner);
		this.owner = owner;
		this.textTitle.text = owner.langTransfer.lang();
		this.OnOpen();
		this.buttonOwner.SetCardGrid(owner.owner, null);
		owner.Container.things.RefreshGrid();
		for (int j = 0; j < this.buttons.Count; j++)
		{
			this.buttons[j].gameObject.AddComponent<CanvasGroup>();
			if (owner.numDragGrid > j)
			{
				this.buttons[j].SetCardGrid(null, owner);
				this.buttons[j].index = j;
				owner.buttons.Add(this.buttons[j]);
			}
			else
			{
				this.buttons[j].SetActive(false);
			}
		}
		this.uiIngredients.Refresh();
		this.textFuel.SetActive(owner.owner.trait.IsRequireFuel);
		owner.BuildUICurrency(this.uiCurrency, false);
		base.InvokeRepeating("RefreshCost", 0f, 0.2f);
		this.RebuildLayout(true);
		this.RefreshCurrentGrid();
		this.RefreshCost();
		this.RebuildLayout(true);
		this.buttonRefuel.SetActive(owner.ShowFuel);
		this.buttonAutoRefuel.SetActive(owner.ShowFuel);
		this.buttonStock.SetActive(owner.AllowStockIngredients);
		if (owner.CanTargetAlly && ELayer.pc.party.members.Count > 1)
		{
			BaseList baseList = this.listAlly;
			UIList.Callback<Chara, UIButton> callback = new UIList.Callback<Chara, UIButton>();
			callback.onInstantiate = delegate(Chara a, UIButton b)
			{
				a.SetImage(b.icon);
				b.SetTooltipLang(a.Name);
			};
			callback.onClick = delegate(Chara c, UIButton i)
			{
				if (LayerInventory.CloseAllyInv(c))
				{
					return;
				}
				SE.PopInventory();
				LayerInventory.CreateContainerAlly(c, c);
			};
			callback.onSort = delegate(Chara a, UIList.SortMode m)
			{
				a.SetSortVal(m, CurrencyType.Money);
				return -a.sortVal;
			};
			callback.onList = delegate(UIList.SortMode m)
			{
				foreach (Chara chara in ELayer.pc.party.members)
				{
					if (!chara.IsPC)
					{
						this.listAlly.Add(chara);
					}
				}
			};
			baseList.callbacks = callback;
			this.listAlly.List(false);
		}
		else
		{
			this.listAlly.SetActive(false);
		}
		Action <>9__7;
		this.buttonRefuel.SetOnClick(delegate
		{
			if (!ELayer.pc.HasNoGoal)
			{
				SE.BeepSmall();
				return;
			}
			this.windows[0].SetInteractable(false, 0f);
			this.info.InitFuel(owner.owner);
			Layer layer = LayerDragGrid.Create(new InvOwnerRefuel(owner.owner, null, CurrencyType.None), true);
			Action onKill;
			if ((onKill = <>9__7) == null)
			{
				onKill = (<>9__7 = delegate()
				{
					if (!this.isDestroyed)
					{
						this.windows[0].SetInteractable(true, 0.5f);
						this.info.Init(owner.owner);
						this.OnOpen();
						this.RefreshCurrentGrid();
						this.Redraw();
						owner.OnAfterRefuel();
					}
				});
			}
			layer.SetOnKill(onKill);
		});
		this.buttonAutoRefuel.SetOnClick(delegate
		{
			SE.Click();
			owner.owner.autoRefuel = !owner.owner.autoRefuel;
			this.RefreshCost();
			if (owner.owner.trait.IsFuelEnough(1, null, true))
			{
				owner.OnAfterRefuel();
			}
		});
		this.buttonStock.SetOnClick(delegate
		{
			SE.Click();
			owner.owner.c_isDisableStockUse = !owner.owner.c_isDisableStockUse;
			this.uiIngredients.Refresh();
			this.RefreshCost();
		});
		try
		{
			if (owner is InvOwnerRefuel)
			{
				if (refuelFromLayerDragGrid)
				{
					this.info.SetActive(false);
				}
				else
				{
					Debug.Log(owner.owner);
					this.info.InitFuel(owner.owner);
				}
			}
			else
			{
				this.info.Init(owner.owner);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
		}
		return this;
	}

	public int GetPrice()
	{
		return this.owner.price;
	}

	public void RefreshCost()
	{
		this.buttonAutoRefuel.mainText.text = (this.owner.owner.autoRefuel ? "On" : "Off");
		this.buttonAutoRefuel.icon.SetAlpha(this.owner.owner.autoRefuel ? 1f : 0.4f);
		this.buttonStock.mainText.text = ((!this.owner.owner.c_isDisableStockUse) ? "On" : "Off");
		this.buttonStock.icon.SetAlpha((!this.owner.owner.c_isDisableStockUse) ? 1f : 0.4f);
		this.textFuel.text = ((int)((float)this.owner.owner.c_charges / (float)this.owner.owner.trait.MaxFuel * 100f)).ToString() + "%";
		int price = this.GetPrice();
		this.itemCost.transform.parent.SetActive(price != 0);
		if (price == 0)
		{
			return;
		}
		this.itemCost.text1.SetText(Lang._currency(this.GetPrice(), false, 14), (ELayer.pc.GetCurrency(this.owner.IDCurrency) >= price) ? FontColor.Good : FontColor.Bad);
		this.itemCost.image1.sprite = SpriteSheet.Get("icon_" + this.owner.IDCurrency);
	}

	public bool IsAllGridSet()
	{
		for (int i = 0; i < this.owner.numDragGrid; i++)
		{
			if (this.buttons[i].card == null)
			{
				return false;
			}
		}
		return true;
	}

	public override void ClearButtons()
	{
		for (int i = 0; i < this.owner.numDragGrid; i++)
		{
			this.buttons[i].SetCardGrid(null, null);
		}
		this.RefreshCurrentGrid();
		this.uiIngredients.Refresh();
	}

	public override void RefreshCurrentGrid()
	{
		int num = -1;
		for (int i = 0; i < this.buttons.Count; i++)
		{
			bool flag = this.buttons[i].Card != null;
			if (num == -1 && (!flag || i == this.owner.numDragGrid - 1))
			{
				num = i;
			}
			this.buttons[i].interactable = (flag || num == i);
			this.buttons[i].GetComponent<CanvasGroup>().alpha = (this.buttons[i].interactable ? 1f : 0.5f);
		}
		this.currentIndex = num;
		bool flag2 = num >= 0 && num < this.owner.numDragGrid;
		if (flag2)
		{
			this.transIndex.position = this.buttons[num].transform.position;
		}
		this.transIndex.SetActive(flag2);
		this.Redraw();
	}

	public override List<Thing> GetTargets()
	{
		List<Thing> list = new List<Thing>();
		TraitCrafter traitCrafter = this.owner.owner.trait as TraitCrafter;
		for (int i = 0; i < traitCrafter.numIng; i++)
		{
			Card card = this.buttons[i].card;
			list.Add(((card != null) ? card.Thing : null) ?? null);
		}
		return list;
	}

	private void Update()
	{
		this.Validate();
	}

	public void Validate()
	{
		Card card = this.buttonOwner.card;
		if (card.Num != this.lastNum)
		{
			this.lastNum = card.Num;
			this.buttonOwner.SetCardGrid(this.owner.owner, null);
		}
		if (card == null || card.isDestroyed)
		{
			this.Close();
		}
	}

	public void Redraw()
	{
		foreach (ButtonGrid buttonGrid in this.buttons)
		{
			if (buttonGrid.gameObject.activeSelf)
			{
				buttonGrid.Redraw();
			}
		}
		LayerInventory.SetDirtyAll(true);
	}

	public override void OnKill()
	{
		EInput.haltInput = false;
		LayerInventory.CloseAllyInv();
		InvOwner.Trader = null;
		if (!this.wasInventoryOpen && ELayer.ui.IsInventoryOpen)
		{
			ELayer.ui.ToggleInventory(false);
		}
		LayerInventory.SetDirtyAll(false);
		if (ELayer.pc.ai is AI_UseCrafter && ELayer.pc.ai.IsRunning)
		{
			ELayer.pc.ai.Cancel();
		}
	}

	public override void OnUpdateInput()
	{
		if (EInput.action == EAction.MenuInventory || Input.GetKeyDown(KeyCode.Tab))
		{
			this.Close();
			EInput.WaitReleaseKey();
			return;
		}
		base.OnUpdateInput();
	}

	public override void OnRightClick()
	{
		if (InputModuleEX.GetComponentOf<ButtonGrid>() == null)
		{
			this.Close();
		}
	}

	public static LayerDragGrid Create(InvOwnerDraglet owner, bool refuelFromLayerDragGrid = false)
	{
		return ELayer.ui.AddLayer<LayerDragGrid>("LayerInventory/LayerDragGrid").SetInv(owner, refuelFromLayerDragGrid);
	}

	public static LayerDragGrid CreateOffering(TraitAltar altar)
	{
		Msg.Say("offer_what");
		return LayerDragGrid.Create(new InvOwnerOffering(altar.owner, null, CurrencyType.Money)
		{
			altar = altar
		}, false);
	}

	public static LayerDragGrid CreateDeliver(InvOwnerDeliver.Mode mode = InvOwnerDeliver.Mode.Default, Card owner = null)
	{
		if (mode == InvOwnerDeliver.Mode.Tax)
		{
			Msg.Say("bills", ELayer.player.taxBills.ToString() ?? "", null, null, null);
		}
		Msg.Say("deliver_what");
		return LayerDragGrid.Create(new InvOwnerDeliver(owner, null, CurrencyType.Money)
		{
			mode = mode
		}, false);
	}

	public static LayerDragGrid CreateGive(Chara c)
	{
		return LayerDragGrid.Create(new InvOwnerGive(c, null, CurrencyType.Money)
		{
			chara = c
		}, false);
	}

	public static LayerDragGrid CreateCraft(TraitCrafter crafter)
	{
		return LayerDragGrid.Create(new InvOwnerCraft(crafter.owner, null, CurrencyType.None)
		{
			crafter = crafter
		}, false);
	}

	public static LayerDragGrid CreateRecycle(TraitRecycle recycle)
	{
		Msg.Say("recycle_what");
		return LayerDragGrid.Create(new InvOwnerRecycle(recycle.owner, null, CurrencyType.Ecopo)
		{
			recycle = recycle
		}, false);
	}

	public static LayerDragGrid CreateGacha(TraitGacha gacha)
	{
		Msg.Say("target_what");
		return LayerDragGrid.Create(new InvOwnerGacha(gacha.owner, null, CurrencyType.None)
		{
			gacha = gacha
		}, false);
	}

	public static LayerDragGrid TryProc(Chara cc, InvOwnerEffect owner)
	{
		owner.cc = cc;
		if (cc.IsPC)
		{
			return LayerDragGrid.Create(owner, false);
		}
		int num = (owner.count == -1) ? 1 : owner.count;
		int i = 0;
		Func<Thing, bool> <>9__0;
		while (i < num)
		{
			ThingContainer things = cc.things;
			Func<Thing, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = ((Thing t) => owner.ShouldShowGuide(t)));
			}
			List<Thing> list = things.List(func, false);
			if (list.Count > 0)
			{
				Thing t2 = list.RandomItem<Thing>();
				owner._OnProcess(t2);
				i++;
			}
			else
			{
				if (i == 0)
				{
					cc.SayNothingHappans();
					break;
				}
				break;
			}
		}
		return null;
	}

	public static LayerDragGrid CreateIdentify(Chara cc, bool superior = false, BlessedState state = BlessedState.Normal, int price = 0, int count = 1)
	{
		return LayerDragGrid.TryProc(cc, new InvOwnerIdentify
		{
			state = state,
			price = price,
			count = count,
			superior = superior
		});
	}

	public static LayerDragGrid CreateEnchant(Chara cc, bool armor, bool superior = false, BlessedState state = BlessedState.Normal, int count = 1)
	{
		return LayerDragGrid.TryProc(cc, new InvOwnerEnchant
		{
			armor = armor,
			state = state,
			superior = superior,
			count = count
		});
	}

	public static LayerDragGrid CreateChangeMaterial(Chara cc, Thing consume, SourceMaterial.Row mat, EffectId idEffect, BlessedState state = BlessedState.Normal, int price = 0, int count = 1)
	{
		return LayerDragGrid.TryProc(cc, new InvOwnerChangeMaterial
		{
			consume = consume,
			mat = mat,
			state = state,
			price = price,
			count = count,
			idEffect = idEffect
		});
	}

	public static LayerDragGrid CreateUncurse(Chara cc, BlessedState state = BlessedState.Normal, int price = 0, int count = 1)
	{
		return LayerDragGrid.TryProc(cc, new InvOwnerUncurse
		{
			state = state,
			price = price,
			count = count
		});
	}

	public static LayerDragGrid CreateLighten(Chara cc, BlessedState state = BlessedState.Normal, int price = 0, int count = 1)
	{
		return LayerDragGrid.TryProc(cc, new InvOwnerLighten
		{
			state = state,
			price = price,
			count = count
		});
	}

	public static LayerDragGrid CreateReconstruction(Chara cc, BlessedState state = BlessedState.Normal, int price = 0, int count = 1)
	{
		return LayerDragGrid.TryProc(cc, new InvOwnerReconstruction
		{
			state = state,
			price = price,
			count = count
		});
	}

	public static LayerDragGrid Instance;

	public UIItem itemCost;

	public Image imageOwner;

	public UIText textTitle;

	public UIText textFuel;

	public Transform transIndex;

	public List<ButtonGrid> buttons;

	public ButtonGrid buttonOwner;

	public UIButton buttonRefuel;

	public UIButton buttonAutoRefuel;

	public UIButton buttonAlly;

	public UIButton buttonStock;

	public InvOwnerDraglet owner;

	public UICurrency uiCurrency;

	public int currentIndex;

	public UIDragGridInfo info;

	public UIList listAlly;

	public UIDragGridIngredients uiIngredients;

	private bool wasInventoryOpen;

	private int lastNum;
}
