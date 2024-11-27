using System;

public class TraitToolShears : TraitTool
{
	public override bool DisableAutoCombat
	{
		get
		{
			return true;
		}
	}

	public override Emo2 GetHeldEmo(Chara c)
	{
		if (!c.CanBeSheared())
		{
			return Emo2.none;
		}
		if (c.c_fur < 40)
		{
			return Emo2.fur;
		}
		return Emo2.fur2;
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		foreach (Chara chara in p.pos.Charas)
		{
			if (chara.CanBeSheared())
			{
				p.TrySetAct(new AI_Shear
				{
					target = chara
				}, chara);
			}
		}
	}
}
