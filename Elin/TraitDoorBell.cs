public class TraitDoorBell : Trait
{
	public override void OnOpenDoor(Chara c)
	{
		owner.PlaySound(GetParam(1));
	}
}
