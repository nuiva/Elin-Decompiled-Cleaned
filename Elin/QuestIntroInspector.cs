using System;

public class QuestIntroInspector : QuestProgression
{
	public override void OnComplete()
	{
		Chara chara = CharaGen.Create("loytel", -1);
		chara.SetInt(100, 1);
		EClass._zone.AddCard(chara, EClass.pc.pos.GetNearestPoint(false, true, true, false));
		EClass.Branch.AddMemeber(chara);
		EClass.game.quests.globalList.Add(Quest.Create("shippingChest", null, null).SetClient(chara, false));
		EClass.game.quests.globalList.Add(Quest.Create("exploration", null, null).SetClient(EClass.game.cards.globalCharas.Find("ashland"), false));
	}
}
