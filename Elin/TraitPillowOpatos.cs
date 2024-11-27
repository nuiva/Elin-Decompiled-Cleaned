using System;

public class TraitPillowOpatos : TraitPillowGod
{
	public override Religion Deity
	{
		get
		{
			return EClass.game.religions.Earth;
		}
	}
}
