using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class InvOwnerDraglet : InvOwner
{
	public virtual InvOwnerDraglet.ProcessType processType
	{
		get
		{
			return InvOwnerDraglet.ProcessType.None;
		}
	}

	public virtual bool ShowFuel
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanTargetAlly
	{
		get
		{
			return false;
		}
	}

	public virtual string langWhat
	{
		get
		{
			return "";
		}
	}

	public virtual bool AllowStockIngredients
	{
		get
		{
			return false;
		}
	}

	public override bool AllowTransfer
	{
		get
		{
			return true;
		}
	}

	public override bool AllowAutouse
	{
		get
		{
			return false;
		}
	}

	public override bool AllowContext
	{
		get
		{
			return false;
		}
	}

	public override bool AllowHold(Thing t)
	{
		return false;
	}

	public override bool UseGuide
	{
		get
		{
			return true;
		}
	}

	public override bool CopyOnTransfer
	{
		get
		{
			return true;
		}
	}

	public override bool InvertSell
	{
		get
		{
			return true;
		}
	}

	public override bool DenyImportant
	{
		get
		{
			return false;
		}
	}

	public LayerDragGrid dragGrid
	{
		get
		{
			return LayerDragGrid.Instance;
		}
	}

	public InvOwnerDraglet(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.Money) : base(owner, container, _currency, PriceType.Default)
	{
	}

	public override void OnInit()
	{
		if (this.price == 0)
		{
			this.currency = CurrencyType.None;
		}
		if (!this.langWhat.IsEmpty())
		{
			Msg.Say(this.langWhat);
		}
	}

	public override void OnClick(ButtonGrid button)
	{
		if (button.card != null)
		{
			if (button.index == 0)
			{
				this.dragGrid.ClearButtons();
			}
			else
			{
				button.SetCard(null, ButtonGrid.Mode.Default, null);
				this.dragGrid.RefreshCurrentGrid();
				this.dragGrid.uiIngredients.Refresh();
			}
			SE.Equip();
			if (EClass.pc.ai is AI_UseCrafter && EClass.pc.ai.IsRunning)
			{
				EClass.pc.ai.Success(null);
			}
		}
	}

	public override void OnRightClick(ButtonGrid button)
	{
		this.OnClick(button);
	}

	public override int GetPrice(Thing t, CurrencyType currency, int num, bool sell)
	{
		return this.price;
	}

	public override string GetTextDetail(Thing t, CurrencyType currency, int num, bool sell)
	{
		int num2 = this.GetPrice(t, currency, num, sell);
		return "invInteraction2".lang(num.ToString() ?? "", this.langTransfer.lang(), null, null, null) + ((num2 == 0) ? "" : "invInteraction3".lang(num2.ToString() ?? "", EClass.sources.things.map[base.IDCurrency].GetName(), null, null, null));
	}

	public sealed override void OnProcess(Thing t)
	{
		if (!this.dragGrid.IsAllGridSet())
		{
			this.dragGrid.RefreshCurrentGrid();
			this.dragGrid.uiIngredients.Refresh();
			t.PlaySoundDrop(false);
			return;
		}
		this._OnProcess(t);
		UIButton.TryShowTip(null, true, true);
		LayerInventory.SetDirty(t);
		if (this.processType == InvOwnerDraglet.ProcessType.Consume)
		{
			EInput.haltInput = true;
			using (List<ButtonGrid>.Enumerator enumerator = this.dragGrid.buttons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ButtonGrid b = enumerator.Current;
					b.icon.rectTransform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(delegate
					{
						if (b.transform != null)
						{
							b.icon.rectTransform.localScale = Vector3.one;
							b.SetCardGrid(null, b.invOwner);
							this.RedrawButton();
						}
						EInput.haltInput = false;
					});
				}
				goto IL_DC;
			}
		}
		this.RedrawButton();
		IL_DC:
		this.dragGrid.uiIngredients.Refresh();
		if (this.count > 0)
		{
			this.count--;
			if (this.count == 0)
			{
				this.dragGrid.Close();
				return;
			}
			if (!this.langWhat.IsEmpty())
			{
				Msg.Say(this.langWhat);
			}
		}
	}

	public virtual void _OnProcess(Thing t)
	{
	}

	public virtual void OnAfterRefuel()
	{
	}

	public void RedrawButton()
	{
		this.dragGrid.RefreshCost();
		this.dragGrid.RefreshCurrentGrid();
	}

	public override void BuildUICurrency(UICurrency uiCurrency, bool canReroll = false)
	{
		base.BuildUICurrency(uiCurrency, false);
		uiCurrency.SetActive(this.price != 0);
	}

	public int count = -1;

	public BlessedState state;

	public int price;

	public enum ProcessType
	{
		None,
		Consume
	}
}
