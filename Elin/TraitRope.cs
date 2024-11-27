using System;

public class TraitRope : TraitItem
{
	public override bool OnUse(Chara c)
	{
		Dialog.YesNo("dialog_rope", delegate
		{
			EClass.player.EndTurn(true);
			EClass.pc.DamageHP(99999, AttackSource.Hang, null);
		}, null, "yes", "no");
		return false;
	}
}
