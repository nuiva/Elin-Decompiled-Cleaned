public class TraitCurrencyMedal : TraitCurrency
{
	public override void OnStepped(Chara c)
	{
		if (c.IsPC)
		{
			Msg.Say("spotMedalStep");
			owner.SetPlaceState(PlaceState.roaming);
		}
	}
}
