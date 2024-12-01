using System.Collections.Generic;

public class ConHOT : Timebuff
{
	public override void Tick()
	{
		Dice dice = Dice.Create("SpHOT", base.power);
		owner.HealHP(dice.Roll(), HealSource.HOT);
		Mod(-1);
	}

	public override void OnWriteNote(List<string> list)
	{
		Dice dice = Dice.Create("SpHOT", base.power);
		list.Add("hintHOT".lang(dice.ToString()));
	}
}
