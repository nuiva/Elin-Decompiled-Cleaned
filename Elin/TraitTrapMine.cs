public class TraitTrapMine : TraitTrap
{
	public override int DestroyChanceOnActivateTrap => 100;

	public override void OnStepped(Chara c)
	{
		if (!(c.id == "dog_mine"))
		{
			base.OnStepped(c);
		}
	}
}
