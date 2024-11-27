using System;

public class StickyMenu : BaseSticky
{
	public override int idIcon
	{
		get
		{
			return 5;
		}
	}

	public override string idLang
	{
		get
		{
			return "";
		}
	}

	public override bool ShouldShow
	{
		get
		{
			return base.widget.extra.showNerun;
		}
	}

	public override bool animate
	{
		get
		{
			return false;
		}
	}

	public override void OnClick()
	{
		LayerDrama.ActivateMain("nerun", "9-1", null, null, "");
	}
}
