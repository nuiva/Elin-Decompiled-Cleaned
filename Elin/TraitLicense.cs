public class TraitLicense : TraitScroll
{
	public override void OnRead(Chara c)
	{
		EClass.player.ModKeyItem(owner.id);
		owner.ModNum(-1);
	}
}
