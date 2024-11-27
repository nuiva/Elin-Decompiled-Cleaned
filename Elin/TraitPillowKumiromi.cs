using System;

public class TraitPillowKumiromi : TraitPillowGod
{
	public override Religion Deity
	{
		get
		{
			return EClass.game.religions.Harvest;
		}
	}
}
