using System;

public class TraitDeposit : TraitContainer
{
	public override bool IsSpecialContainer
	{
		get
		{
			return true;
		}
	}

	public override void Prespawn(int lv)
	{
	}
}
