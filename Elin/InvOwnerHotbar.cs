public class InvOwnerHotbar : InvOwner
{
	public int index;

	public override bool AllowAutouse
	{
		get
		{
			if (InvOwner.Trader != null)
			{
				return InvOwner.Trader.UseGuide;
			}
			return false;
		}
	}

	public override int destInvY => 1;

	public InvOwnerHotbar(Card owner, Card container = null, CurrencyType _currency = CurrencyType.None)
		: base(owner, container, _currency)
	{
	}

	public override void OnClick(ButtonGrid button)
	{
		if ((bool)EClass.ui.layerFloat.GetLayer<LayerInventory>())
		{
			base.OnClick(button);
			return;
		}
		SE.SelectHotitem();
		WidgetCurrentTool.Instance.Select(index % 10);
	}

	public override void OnRightClick(ButtonGrid button)
	{
		if (AllowAutouse)
		{
			base.OnRightClick(button);
			return;
		}
		SE.SelectHotitem();
		WidgetCurrentTool.Instance.Select(index % 10);
	}

	public override void OnRightPressed(ButtonGrid button)
	{
	}

	public override void OnProcess(Thing t)
	{
		if (WidgetCurrentTool.Instance.selected == index)
		{
			WidgetCurrentTool.Instance.Reselect();
		}
		if (!EClass.player.flags.toggleHotbarHighlightActivated && (bool)WidgetCurrentTool.Instance)
		{
			WidgetCurrentTool.Instance.transHighlightSwitch.SetActive(enable: true);
			EClass.player.flags.toggleHotbarHighlightActivated = true;
			EClass.player.flags.toggleHotbarHighlightDisabled = false;
		}
	}
}
