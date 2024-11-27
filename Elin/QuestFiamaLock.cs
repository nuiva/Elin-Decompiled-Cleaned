using System;

public class QuestFiamaLock : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		return this.phase == 0;
	}

	public override void OnStart()
	{
		EClass.player.DropReward(ThingGen.Create("lockpick", -1, -1), false).c_charges = 12;
		Thing thing = EClass.player.DropReward(ThingGen.Create("1171", -1, -1), false);
		thing.ChangeMaterial("willow");
		thing.c_lockLv = 50;
		thing.c_lockedHard = true;
		thing.things.DestroyAll(null);
		thing.AddCard(ThingGen.Create("1172", -1, -1));
		thing.AddCard(ThingGen.Create("amulet_begger", -1, -1));
		thing.AddCard(ThingGen.Create("_meat", -1, -1).MakeFoodFrom("begger")).MaxDecay++;
		thing.Install();
		EClass._zone.events.Add(new ZoneEventBeggerChest
		{
			uidChest = thing.uid
		}, false);
	}
}
