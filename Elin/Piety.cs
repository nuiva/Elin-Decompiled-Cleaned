using System;

public class Piety : Element
{
	public override bool ShowMsgOnValueChanged
	{
		get
		{
			return false;
		}
	}

	public override bool CanGainExp
	{
		get
		{
			return true;
		}
	}

	public override bool UsePotential
	{
		get
		{
			return false;
		}
	}

	public override bool UseExpMod
	{
		get
		{
			return false;
		}
	}
}
