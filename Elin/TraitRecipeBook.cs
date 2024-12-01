public class TraitRecipeBook : TraitScroll
{
	public override string langNote => "traitRecipeBook";

	public override int GetActDuration(Chara c)
	{
		return 10;
	}

	public override void OnRead(Chara c)
	{
	}

	public override void WriteNote(UINote n, bool identified)
	{
	}
}
