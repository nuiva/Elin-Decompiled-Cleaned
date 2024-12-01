public class Zone_SubTown : Zone_Town
{
	public override bool CanBeDeliverDestination => false;

	public override void OnGenerateRooms(BaseMapGen gen)
	{
		base.OnGenerateRooms(gen);
		if (!(id == "foxtown"))
		{
			return;
		}
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsPCFaction)
			{
				chara.SetFaith(EClass.game.religions.Trickery);
			}
		}
	}
}
