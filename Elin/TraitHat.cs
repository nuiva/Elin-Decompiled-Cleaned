public class TraitHat : TraitItem
{
	public override bool OnUse(Chara c)
	{
		SE.Play("mutation");
		EClass._zone.idHat = ((EClass._zone.idHat == owner.id) ? null : owner.id);
		EClass._zone.dateHat = ((EClass._zone.idHat != null) ? EClass.world.date.GetRaw(720) : 0);
		EClass._zone.RefreshHat();
		return false;
	}
}
