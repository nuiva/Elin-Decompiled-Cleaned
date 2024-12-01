public class TraitGambleChest : TraitItem
{
	public override void OnCreate(int lv)
	{
		owner.c_lockLv = lv;
		owner.c_revealLock = true;
	}

	public override bool OnUse(Chara c)
	{
		EClass.pc.SetAIImmediate(new AI_OpenGambleChest
		{
			target = owner.Thing
		});
		return false;
	}

	public override bool CanStackTo(Thing to)
	{
		return owner.c_lockLv == to.c_lockLv;
	}

	public override int GetValue()
	{
		return owner.sourceCard.value + owner.c_lockLv * 50;
	}
}
