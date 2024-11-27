using System;

public class TraitCard : TraitFigure
{
	public override bool ShowShadow
	{
		get
		{
			return false;
		}
	}

	public override int GetMatColor()
	{
		return -3;
	}
}
