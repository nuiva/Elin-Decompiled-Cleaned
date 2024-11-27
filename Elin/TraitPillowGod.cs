using System;

public class TraitPillowGod : TraitPillow
{
	public virtual Religion Deity
	{
		get
		{
			return EClass.game.religions.Eyth;
		}
	}
}
