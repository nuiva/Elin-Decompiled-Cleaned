using System;

public class HotItemGameAction : HotItem
{
	public virtual Act act
	{
		get
		{
			return null;
		}
	}

	public override bool IsGameAction
	{
		get
		{
			return true;
		}
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		this.hotbar.Select(this);
	}

	public override void OnRightClick(ButtonHotItem b)
	{
		this.hotbar.Select(this);
	}

	public override void OnMarkMapHighlights()
	{
		if (this.act == null)
		{
			return;
		}
		this.act.OnMarkMapHighlights();
	}
}
