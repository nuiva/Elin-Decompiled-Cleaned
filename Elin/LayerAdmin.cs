using System;

public class LayerAdmin : ELayer
{
	public override void OnSwitchContent(Window window)
	{
		int idTab = window.idTab;
		if (idTab == 0)
		{
			this.RefreshResearch();
			return;
		}
		if (idTab != 1)
		{
			return;
		}
		this.RefreshPolicy();
	}

	public void RefreshResearch()
	{
	}

	public void RefreshPolicy()
	{
	}
}
