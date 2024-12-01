public class TraitRecipeCat : TraitScroll
{
	public string Cat => GetParam(1);

	public override int GetValue()
	{
		return base.GetValue() * (100 + (owner.LV - 5) * 10) / 100;
	}

	public override void OnRead(Chara c)
	{
		string randomRecipe = RecipeManager.GetRandomRecipe(owner.LV, Cat, onlyUnlearned: true);
		if (randomRecipe.IsEmpty())
		{
			Msg.SayNothingHappen();
			return;
		}
		EClass.player.recipes.Add(randomRecipe);
		owner.ModNum(-1);
	}

	public override void SetName(ref string s)
	{
		s = s + " Lv." + owner.LV;
	}

	public override bool CanStackTo(Thing to)
	{
		if (base.CanStackTo(to))
		{
			return owner.LV == to.LV;
		}
		return false;
	}
}
