using System;

public class TraitPillowEhekatl : TraitPillowGod
{
	public override Religion Deity
	{
		get
		{
			return EClass.game.religions.Luck;
		}
	}
}
