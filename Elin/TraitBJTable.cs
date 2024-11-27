using System;

public class TraitBJTable : TraitGamble
{
	public override string idMsg
	{
		get
		{
			return "use_card";
		}
	}

	public override bool OnUse(Chara c)
	{
		MiniGame.Activate(MiniGame.Type.Blackjack);
		return false;
	}
}
