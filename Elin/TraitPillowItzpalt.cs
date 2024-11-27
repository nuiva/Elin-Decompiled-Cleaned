using System;

public class TraitPillowItzpalt : TraitPillowGod
{
	public override Religion Deity
	{
		get
		{
			return EClass.game.religions.Element;
		}
	}
}
