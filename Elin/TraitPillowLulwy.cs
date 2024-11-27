using System;

public class TraitPillowLulwy : TraitPillowGod
{
	public override Religion Deity
	{
		get
		{
			return EClass.game.religions.Wind;
		}
	}
}
