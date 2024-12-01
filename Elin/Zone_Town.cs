public class Zone_Town : Zone_Civilized
{
	public override bool IsTown => true;

	public override bool IsExplorable => false;

	public override bool CanDigUnderground => false;

	public override bool CanSpawnAdv => base.lv == 0;

	public override bool AllowCriminal => false;
}
