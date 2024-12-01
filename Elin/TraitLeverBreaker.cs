public class TraitLeverBreaker : TraitLever
{
	public override bool UseAltTiles => EClass._map.isBreakerDown;

	public override void OnToggle()
	{
		if (!EClass._zone.IsTown)
		{
			EClass._map.isBreakerDown = !EClass._map.isBreakerDown;
			bool isBreakerDown = EClass._map.isBreakerDown;
			owner.PlaySound(isBreakerDown ? "electricity_off" : "electricity_on");
			EClass._zone.RefreshElectricity();
		}
	}
}
