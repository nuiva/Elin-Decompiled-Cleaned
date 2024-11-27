using System;

public class AttbSpecial : Element
{
	public override bool CanLink(ElementContainer owner)
	{
		return !base.IsGlobalElement;
	}

	public override bool ShowPotential
	{
		get
		{
			return false;
		}
	}
}
