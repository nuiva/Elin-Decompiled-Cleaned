using System;

public class TraitAdventurerBacker : TraitAdventurer
{
	public override bool UseRandomAlias
	{
		get
		{
			return false;
		}
	}

	public override TraitChara.Adv_Type AdvType
	{
		get
		{
			return TraitChara.Adv_Type.Adv_Backer;
		}
	}
}
