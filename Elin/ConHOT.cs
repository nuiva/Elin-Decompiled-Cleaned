using System;
using System.Collections.Generic;

public class ConHOT : Timebuff
{
	public override void Tick()
	{
		Dice dice = Dice.Create("SpHOT", base.power, null, null);
		this.owner.HealHP(dice.Roll(), HealSource.HOT);
		base.Mod(-1, false);
	}

	public override void OnWriteNote(List<string> list)
	{
		Dice dice = Dice.Create("SpHOT", base.power, null, null);
		list.Add("hintHOT".lang(dice.ToString(), null, null, null, null));
	}
}
