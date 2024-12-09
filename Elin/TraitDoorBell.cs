public class TraitDoorBell : Trait
{
	public override TileMode tileMode => TileMode.SignalAnime;

	public override void OnOpenDoor(Chara c)
	{
		owner.PlaySound(GetParam(1));
		owner.animeCounter = 0.01f;
	}
}
