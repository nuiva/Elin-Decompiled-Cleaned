public class TraitBJTable : TraitGamble
{
	public override string idMsg => "use_card";

	public override bool OnUse(Chara c)
	{
		MiniGame.Activate(MiniGame.Type.Blackjack);
		return false;
	}
}
