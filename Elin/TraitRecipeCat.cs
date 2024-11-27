using System;

public class TraitRecipeCat : TraitScroll
{
	public string Cat
	{
		get
		{
			return base.GetParam(1, null);
		}
	}

	public override int GetValue()
	{
		return base.GetValue() * (100 + (this.owner.LV - 5) * 10) / 100;
	}

	public override void OnRead(Chara c)
	{
		string randomRecipe = RecipeManager.GetRandomRecipe(this.owner.LV, this.Cat, true);
		if (randomRecipe.IsEmpty())
		{
			Msg.SayNothingHappen();
			return;
		}
		EClass.player.recipes.Add(randomRecipe, true);
		this.owner.ModNum(-1, true);
	}

	public override void SetName(ref string s)
	{
		s = s + " Lv." + this.owner.LV.ToString();
	}

	public override bool CanStackTo(Thing to)
	{
		return base.CanStackTo(to) && this.owner.LV == to.LV;
	}
}
