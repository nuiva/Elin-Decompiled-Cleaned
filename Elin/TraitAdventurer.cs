using System;

public class TraitAdventurer : TraitChara
{
	public override bool UseGlobalGoal
	{
		get
		{
			return base.owner.IsGlobal;
		}
	}

	public override bool UseRandomAbility
	{
		get
		{
			return true;
		}
	}

	public override bool UseRandomAlias
	{
		get
		{
			return true;
		}
	}

	public override bool ShowAdvRank
	{
		get
		{
			return true;
		}
	}

	public override bool HaveNews
	{
		get
		{
			return true;
		}
	}

	public override bool CanBout
	{
		get
		{
			return true;
		}
	}

	public override TraitChara.Adv_Type AdvType
	{
		get
		{
			if (!(base.owner.id == "adv"))
			{
				return TraitChara.Adv_Type.Adv_Fairy;
			}
			return TraitChara.Adv_Type.Adv;
		}
	}
}
