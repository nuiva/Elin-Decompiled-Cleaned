using System;

public class StickyDate : BaseSticky
{
	public override int idIcon => 4;

	public override bool ForceShowText => true;

	public override bool ShouldShow => base.widget.extra.showDate;

	public override string GetText()
	{
		return DateTime.Now.ToString("H:mm");
	}

	public override void Refresh()
	{
		SetText();
	}
}
