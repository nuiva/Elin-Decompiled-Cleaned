using System;

public class TraitTile : Trait
{
	public virtual TileRow source
	{
		get
		{
			return null;
		}
	}

	public virtual string suffix
	{
		get
		{
			return "";
		}
	}

	public override bool CanExtendBuild
	{
		get
		{
			return true;
		}
	}

	public override bool CanBuildInTown
	{
		get
		{
			return false;
		}
	}

	public override void SetName(ref string s)
	{
		s = this.source.GetName();
	}

	public override Recipe GetRecipe()
	{
		return Recipe.Create(RecipeManager.dict[this.source.RecipeID + this.suffix], this.owner.material.id, null);
	}

	public override Recipe GetBuildModeRecipe()
	{
		return Recipe.Create(RecipeManager.dict[this.source.RecipeID + this.suffix], -1, this.owner.Thing);
	}
}
