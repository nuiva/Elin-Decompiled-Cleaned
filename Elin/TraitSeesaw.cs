public class TraitSeesaw : TraitFloorSwitch
{
	public override bool UseAltTiles => owner.isOn;

	public override void OnActivateTrap(Chara c)
	{
		owner.isOn = !owner.isOn;
		owner.PlaySound("seesaw");
		TraitSwitch.haltMove = false;
	}
}
