using System;

public class TraitPillowJure : TraitPillowGod
{
	public override Religion Deity
	{
		get
		{
			return EClass.game.religions.Healing;
		}
	}
}
