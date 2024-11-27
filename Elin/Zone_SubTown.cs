using System;

public class Zone_SubTown : Zone_Town
{
	public override bool CanBeDeliverDestination
	{
		get
		{
			return false;
		}
	}

	public override void OnGenerateRooms(BaseMapGen gen)
	{
		base.OnGenerateRooms(gen);
		if (this.id == "foxtown")
		{
			foreach (Chara chara in EClass._map.charas)
			{
				if (!chara.IsPCFaction)
				{
					chara.SetFaith(EClass.game.religions.Trickery);
				}
			}
		}
	}
}
