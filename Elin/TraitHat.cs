using System;

public class TraitHat : TraitItem
{
	public override bool OnUse(Chara c)
	{
		SE.Play("mutation");
		EClass._zone.idHat = ((EClass._zone.idHat == this.owner.id) ? null : this.owner.id);
		EClass._zone.dateHat = ((EClass._zone.idHat == null) ? 0 : EClass.world.date.GetRaw(720));
		EClass._zone.RefreshHat();
		return false;
	}
}
