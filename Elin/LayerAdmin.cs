public class LayerAdmin : ELayer
{
	public override void OnSwitchContent(Window window)
	{
		switch (window.idTab)
		{
		case 0:
			RefreshResearch();
			break;
		case 1:
			RefreshPolicy();
			break;
		}
	}

	public void RefreshResearch()
	{
	}

	public void RefreshPolicy()
	{
	}
}
