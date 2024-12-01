public class QuestShippingChest : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		if (phase == 0)
		{
			foreach (Thing thing in EClass._map.things)
			{
				if (thing.IsInstalled && thing.id == "container_shipping" && thing.material.alias == "palm")
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void OnComplete()
	{
		EClass.game.quests.Add("loytel_farm", "loytel").startDate = EClass.world.date.GetRaw() + 1440;
	}
}
