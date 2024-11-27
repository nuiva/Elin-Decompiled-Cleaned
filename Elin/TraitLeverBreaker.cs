using System;

public class TraitLeverBreaker : TraitLever
{
	public override bool UseAltTiles
	{
		get
		{
			return EClass._map.isBreakerDown;
		}
	}

	public override void OnToggle()
	{
		if (EClass._zone.IsTown)
		{
			return;
		}
		EClass._map.isBreakerDown = !EClass._map.isBreakerDown;
		bool isBreakerDown = EClass._map.isBreakerDown;
		this.owner.PlaySound(isBreakerDown ? "electricity_off" : "electricity_on", 1f, true);
		EClass._zone.RefreshElectricity();
	}
}
