using System;

public class AM_BaseSim : AM_BaseGameMode
{
	public override AreaHighlightMode AreaHihlight
	{
		get
		{
			return AreaHighlightMode.Sim;
		}
	}

	public override bool ShouldPauseGame
	{
		get
		{
			return false;
		}
	}
}
