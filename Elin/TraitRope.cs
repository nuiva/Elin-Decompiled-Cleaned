public class TraitRope : TraitItem
{
	public override bool OnUse(Chara c)
	{
		Dialog.YesNo("dialog_rope", delegate
		{
			EClass.player.EndTurn();
			EClass.pc.DamageHP(99999, AttackSource.Hang);
		});
		return false;
	}
}
