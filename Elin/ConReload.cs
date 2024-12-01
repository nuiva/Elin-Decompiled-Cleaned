public class ConReload : Condition
{
	public override int GetPhase()
	{
		return 0;
	}

	public override void OnRemoved()
	{
		if (owner.IsPC)
		{
			WidgetCurrentTool.RefreshCurrentHotItem();
		}
	}
}
