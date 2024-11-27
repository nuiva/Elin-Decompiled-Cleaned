using System;

public class InvOwnerHotbar : InvOwner
{
	public override bool AllowAutouse
	{
		get
		{
			return InvOwner.Trader != null && InvOwner.Trader.UseGuide;
		}
	}

	public override int destInvY
	{
		get
		{
			return 1;
		}
	}

	public InvOwnerHotbar(Card owner, Card container = null, CurrencyType _currency = CurrencyType.None) : base(owner, container, _currency, PriceType.Default)
	{
	}

	public override void OnClick(ButtonGrid button)
	{
		if (EClass.ui.layerFloat.GetLayer<LayerInventory>(false))
		{
			base.OnClick(button);
			return;
		}
		SE.SelectHotitem();
		WidgetCurrentTool.Instance.Select(this.index % 10, false);
	}

	public override void OnRightClick(ButtonGrid button)
	{
		if (this.AllowAutouse)
		{
			base.OnRightClick(button);
			return;
		}
		SE.SelectHotitem();
		WidgetCurrentTool.Instance.Select(this.index % 10, false);
	}

	public override void OnRightPressed(ButtonGrid button)
	{
	}

	public override void OnProcess(Thing t)
	{
		if (WidgetCurrentTool.Instance.selected == this.index)
		{
			WidgetCurrentTool.Instance.Reselect();
		}
	}

	public int index;
}
