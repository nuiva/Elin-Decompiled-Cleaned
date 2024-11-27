using System;

public class TraitPolicyBoard : TraitItem
{
	public override string LangUse
	{
		get
		{
			return "actRead";
		}
	}

	public override bool OnUse(Chara c)
	{
		if (!EClass._zone.IsPCFaction)
		{
			Msg.SayNothingHappen();
			return false;
		}
		EClass.ui.AddLayer<LayerPolicy>();
		return false;
	}
}
