using System;

public class TraitTrainer : TraitCitizen
{
	public override int GuidePriotiy
	{
		get
		{
			return 50;
		}
	}

	public override string IDTrainer
	{
		get
		{
			return base.GetParam(1, null).IsEmpty(TraitTrainer.ids[base.owner.uid % TraitTrainer.ids.Length]);
		}
	}

	public static string[] ids = new string[]
	{
		"combat",
		"weapon",
		"general",
		"craft",
		"labor",
		"mind",
		"stealth"
	};
}
