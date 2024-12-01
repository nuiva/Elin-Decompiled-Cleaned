public class ConWeapon : BaseBuff
{
	public override bool IsElemental => true;

	public override int P2 => owner.CHA;

	public override void Tick()
	{
	}
}
