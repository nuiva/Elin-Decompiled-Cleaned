public class TraitRide : TraitFloorSwitch
{
	public override void OnActivateTrap(Chara c)
	{
		TraitSwitch.haltMove = false;
	}
}
