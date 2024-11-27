using System;

public class QuestShippingChest : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		if (this.phase == 0)
		{
			foreach (Thing thing in EClass._map.things)
			{
				if (thing.IsInstalled && thing.id == "container_shipping" && thing.material.alias == "palm")
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	public override void OnComplete()
	{
		EClass.game.quests.Add("loytel_farm", "loytel").startDate = EClass.world.date.GetRaw(0) + 1440;
	}
}
