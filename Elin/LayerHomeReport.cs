public class LayerHomeReport : ELayer
{
	public override void OnInit()
	{
		GetComponentInChildren<UIHomeInfo>().Refresh();
	}
}
