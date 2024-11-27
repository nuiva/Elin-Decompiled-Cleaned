using System;

public class TraitRecipeBook : TraitScroll
{
	public override int GetActDuration(Chara c)
	{
		return 10;
	}

	public override string langNote
	{
		get
		{
			return "traitRecipeBook";
		}
	}

	public override void OnRead(Chara c)
	{
	}

	public override void WriteNote(UINote n, bool identified)
	{
	}
}
