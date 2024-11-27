using System;

public class TraitDoorAuto : TraitDoorSwing
{
	public override bool UseLowblock
	{
		get
		{
			return true;
		}
	}

	public override string idSound
	{
		get
		{
			return "door2";
		}
	}
}
