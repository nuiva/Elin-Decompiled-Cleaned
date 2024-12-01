public class TileTypeDoor : TileTypeObj
{
	public override string LangPlaceType => "place_Door";

	public override bool FreeStyle => false;

	public override bool CanStack => false;

	public override bool CanBuiltOnBlock => true;

	public override bool IsDoor => true;

	public override bool CanBeHeld
	{
		get
		{
			if (!EClass._zone.IsPCFaction)
			{
				return EClass._zone is Zone_Tent;
			}
			return true;
		}
	}
}
