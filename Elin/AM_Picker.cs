using System;
using System.Collections.Generic;

public class AM_Picker : AM_BaseTileSelect
{
	public override bool IsBuildMode
	{
		get
		{
			return true;
		}
	}

	public override BaseTileSelector.HitType hitType
	{
		get
		{
			return EClass.core.screen.tileSelector.inspectHitType;
		}
	}

	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			return BaseTileSelector.SelectType.Single;
		}
	}

	public override void OnUpdateCursor()
	{
		base.SetCursorOnMap(CursorSystem.Picker);
	}

	public BuildMenu BM
	{
		get
		{
			return BuildMenu.Instance;
		}
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (!this.Test(point, false).IsValid)
		{
			return HitResult.Default;
		}
		return HitResult.Valid;
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		this.Test(point, true);
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (point.cell.HasWallOrFence && (result == HitResult.Valid || result == HitResult.Invalid))
		{
			EClass.screen.guide.DrawWall(point, (result == HitResult.Valid) ? EClass.Colors.blockColors.Valid : EClass.Colors.blockColors.Warning, false, 0f);
			return;
		}
		base.OnRenderTile(point, result, dir);
	}

	public AM_Picker.Result TestBlock(Point point)
	{
		return new AM_Picker.Result
		{
			mat = point.matBlock,
			source = this.TryGetRecipe(point.sourceBlock.RecipeID)
		};
	}

	public AM_Picker.Result TestFloor(Point point)
	{
		AM_Picker.Result result = default(AM_Picker.Result);
		if (point.cell._bridge == 0)
		{
			result.mat = point.matFloor;
			result.source = this.TryGetRecipe(point.sourceFloor.RecipeID);
		}
		else
		{
			result.mat = point.cell.matBridge;
			result.source = this.TryGetRecipe(point.cell.sourceBridge.RecipeID + "-b");
		}
		return result;
	}

	public AM_Picker.Result TestThing(Thing t)
	{
		AM_Picker.Result result = default(AM_Picker.Result);
		if (t.trait.IsGround)
		{
			return result;
		}
		result.mat = t.material;
		result.source = this.TryGetRecipe(t.sourceCard.RecipeID);
		result.thing = t;
		return result;
	}

	public AM_Picker.Result Test(Point point, bool select = false)
	{
		List<Card> list = point.ListCards(true);
		for (int i = list.Count - 1; i >= 0; i--)
		{
			Card card = list[i];
			if (card.isThing)
			{
				AM_Picker.Result result = this.TestThing(card.Thing);
				if (result.IsValid)
				{
					if (select)
					{
						this.Select(result);
					}
					TraitHouseBoard traitHouseBoard = card.trait as TraitHouseBoard;
					if (traitHouseBoard != null)
					{
						ActionMode.Build.houseBoard = traitHouseBoard;
					}
					return result;
				}
			}
		}
		if (point.HasBlock)
		{
			AM_Picker.Result result2 = this.TestBlock(point);
			if (result2.IsValid)
			{
				if (select)
				{
					this.Select(result2);
				}
				return result2;
			}
		}
		if (point.HasFloor)
		{
			AM_Picker.Result result3 = this.TestFloor(point);
			if (result3.IsValid)
			{
				if (select)
				{
					this.Select(result3);
				}
				return result3;
			}
		}
		return default(AM_Picker.Result);
	}

	public RecipeSource TryGetRecipe(string id)
	{
		return EClass.player.recipes.GetSource(id);
	}

	public new bool Select(AM_Picker.Result r)
	{
		ActionMode.ignoreSound = true;
		this.BM.Select(r);
		return true;
	}

	public bool CanActivate
	{
		get
		{
			return !base.IsActive && (EClass.debug.godBuild || (EClass.Branch != null && EClass.Branch.elements.Has(4005)));
		}
	}

	public struct Result
	{
		public bool IsValid
		{
			get
			{
				return this.source != null;
			}
		}

		public string GetText()
		{
			return string.Concat(new string[]
			{
				this.source.Name,
				EClass.debug.showExtra ? (":" + this.source.id) : "",
				"(",
				this.mat.GetName(),
				EClass.debug.showExtra ? (":" + this.mat.id.ToString()) : "",
				")",
				EClass.player.recipes.IsKnown(this.source.id) ? "" : (Environment.NewLine + "noRecipe".lang())
			});
		}

		public string category
		{
			get
			{
				return this.source.recipeCat;
			}
		}

		public RecipeSource source;

		public SourceMaterial.Row mat;

		public Thing thing;
	}
}
