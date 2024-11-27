using System;

public class TraitCurrencyMedal : TraitCurrency
{
	public override void OnStepped(Chara c)
	{
		if (c.IsPC)
		{
			Msg.Say("spotMedalStep");
			this.owner.SetPlaceState(PlaceState.roaming, false);
		}
	}
}
