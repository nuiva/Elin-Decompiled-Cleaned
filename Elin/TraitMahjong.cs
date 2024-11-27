using System;

public class TraitMahjong : TraitGamble
{
	public override int IdleUseChance
	{
		get
		{
			return 1;
		}
	}

	public override string idMsg
	{
		get
		{
			return "use_mahjong";
		}
	}

	public override string idSound
	{
		get
		{
			return "mahjong";
		}
	}

	public override string idTalk
	{
		get
		{
			return "mahjong";
		}
	}
}
