using System;

public class StickyHomeReport : BaseSticky
{
	public override int idIcon
	{
		get
		{
			return 6;
		}
	}

	public override string GetText()
	{
		return string.Concat(new string[]
		{
			"sticky_homeReport".lang(),
			"(",
			EClass.world.date.month.ToString(),
			"/",
			EClass.world.date.day.ToString(),
			")"
		});
	}

	public override bool RemoveOnClick
	{
		get
		{
			return true;
		}
	}

	public override void OnClick()
	{
		EClass.ui.AddLayer<LayerHomeReport>();
	}
}
