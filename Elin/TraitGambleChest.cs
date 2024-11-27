using System;

public class TraitGambleChest : TraitItem
{
	public override void OnCreate(int lv)
	{
		this.owner.c_lockLv = lv;
		this.owner.c_revealLock = true;
	}

	public override bool OnUse(Chara c)
	{
		EClass.pc.SetAIImmediate(new AI_OpenGambleChest
		{
			target = this.owner.Thing
		});
		return false;
	}

	public override bool CanStackTo(Thing to)
	{
		return this.owner.c_lockLv == to.c_lockLv;
	}

	public override int GetValue()
	{
		return this.owner.sourceCard.value + this.owner.c_lockLv * 50;
	}
}
