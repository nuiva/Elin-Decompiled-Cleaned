public class HotItemGameAction : HotItem
{
	public virtual Act act => null;

	public override bool IsGameAction => true;

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		hotbar.Select(this);
	}

	public override void OnRightClick(ButtonHotItem b)
	{
		hotbar.Select(this);
	}

	public override void OnMarkMapHighlights()
	{
		if (act != null)
		{
			act.OnMarkMapHighlights();
		}
	}
}
