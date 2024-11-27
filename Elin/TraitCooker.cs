using System;

public class TraitCooker : TraitFactory
{
	public override Emo Icon
	{
		get
		{
			return Emo.cook;
		}
	}

	public virtual string DestFoodID
	{
		get
		{
			return null;
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "invCook";
		}
	}

	public override AnimeID IdAnimeProgress
	{
		get
		{
			return AnimeID.Jump;
		}
	}

	public override string idSoundProgress
	{
		get
		{
			return "cook";
		}
	}
}
