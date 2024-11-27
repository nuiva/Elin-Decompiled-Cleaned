using System;

public class LayerHomeReport : ELayer
{
	public override void OnInit()
	{
		base.GetComponentInChildren<UIHomeInfo>().Refresh();
	}
}
