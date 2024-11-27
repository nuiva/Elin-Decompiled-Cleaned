using System;

public class TraitPillowMani : TraitPillowGod
{
	public override Religion Deity
	{
		get
		{
			return EClass.game.religions.Machine;
		}
	}
}
