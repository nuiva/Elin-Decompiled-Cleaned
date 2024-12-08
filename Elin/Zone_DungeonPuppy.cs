public class Zone_DungeonPuppy : Zone_DungeonUnfixed
{
	public int LvPoppy => -2;

	public override bool LockExit => base.lv <= LvPoppy;

	public override void OnGenerateMap()
	{
		if (base.lv <= LvPoppy)
		{
			Quest quest = EClass.game.quests.Get("puppy");
			if (quest != null && quest.phase == 0)
			{
				Chara chara = EClass.game.cards.globalCharas.Find("poppy") ?? CharaGen.Create("poppy");
				chara.AddEditorTag(EditorTag.Invulnerable);
				EClass._zone.AddCard(chara, EClass._map.bounds.GetRandomSpawnPos());
			}
		}
		base.OnGenerateMap();
	}

	public override void OnBeforeSimulate()
	{
		if (base.visitCount == 0)
		{
			Tutorial.Reserve("eq");
		}
	}
}
