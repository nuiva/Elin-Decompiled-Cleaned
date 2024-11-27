using System;

public class StickyDate : BaseSticky
{
	public override int idIcon
	{
		get
		{
			return 4;
		}
	}

	public override bool ForceShowText
	{
		get
		{
			return true;
		}
	}

	public override bool ShouldShow
	{
		get
		{
			return base.widget.extra.showDate;
		}
	}

	public override string GetText()
	{
		return DateTime.Now.ToString("H:mm");
	}

	public override void Refresh()
	{
		this.SetText();
	}
}
