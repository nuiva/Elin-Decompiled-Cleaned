using System;

public class ConReload : Condition
{
	public override int GetPhase()
	{
		return 0;
	}

	public override void OnRemoved()
	{
		if (this.owner.IsPC)
		{
			WidgetCurrentTool.RefreshCurrentHotItem();
		}
	}
}
