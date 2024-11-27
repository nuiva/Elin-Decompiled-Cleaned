using System;
using UnityEngine;

public class AM_Build : AM_Designation<TaskBuild>
{
	public bool _IsRoofEditMode(Card c)
	{
		return Input.GetKey(KeyCode.LeftAlt) && (c == null || !c.trait.CanOnlyCarry);
	}

	public override bool IsRoofEditMode(Card c = null)
	{
		Recipe recipe = this.recipe;
		return this._IsRoofEditMode((recipe != null) ? recipe.Mold : null);
	}

	public override bool IsFillMode()
	{
		return !this.recipe.IsThing && Input.GetKey(KeyCode.LeftControl) && EClass.debug.godBuild;
	}

	public override BuildMenu.Mode buildMenuMode
	{
		get
		{
			return BuildMenu.Mode.Build;
		}
	}

	public override int CostMoney
	{
		get
		{
			if (this.recipe == null || this.recipe.UseStock)
			{
				return 0;
			}
			return 10;
		}
	}

	public override bool IsBuildMode
	{
		get
		{
			return true;
		}
	}

	public override string idSound
	{
		get
		{
			return null;
		}
	}

	public override bool AllowAutoClick
	{
		get
		{
			return this.recipe != null && !(this.recipe is RecipeCard);
		}
	}

	public override bool AllowMiddleClickFunc
	{
		get
		{
			return false;
		}
	}

	public override int SelectorHeight
	{
		get
		{
			return this.bridgeHeight;
		}
	}

	public override int TopHeight(Point p)
	{
		return this.bridgeHeight + (this.recipe.tileType.AltitudeAsDir ? 0 : this.altitude);
	}

	public override HitResult HitResultOnDesignation(Point p)
	{
		CellDetail detail = p.detail;
		if (((detail != null) ? detail.designation : null) is TaskBuild)
		{
			return HitResult.Warning;
		}
		return HitResult.Invalid;
	}

	public override BaseTileSelector.HitType hitType
	{
		get
		{
			Recipe recipe = this.recipe;
			if (recipe == null)
			{
				return BaseTileSelector.HitType.None;
			}
			return recipe.tileType.HitType;
		}
	}

	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			Recipe recipe = this.recipe;
			if (recipe == null)
			{
				return BaseTileSelector.SelectType.Single;
			}
			return recipe.tileType.SelectType;
		}
	}

	public override BaseTileSelector.BoxType boxType
	{
		get
		{
			Recipe recipe = this.recipe;
			if (recipe == null)
			{
				return BaseTileSelector.BoxType.Box;
			}
			return recipe.GetBoxType();
		}
	}

	public override int hitW
	{
		get
		{
			Recipe recipe = this.recipe;
			if (recipe == null)
			{
				return 1;
			}
			return recipe.W;
		}
	}

	public override int hitH
	{
		get
		{
			Recipe recipe = this.recipe;
			if (recipe == null)
			{
				return 1;
			}
			return recipe.H;
		}
	}

	public override MeshPass GetGuidePass(Point point)
	{
		if (!this.recipe.IsBlock)
		{
			return EClass.screen.guide.passGuideFloor;
		}
		return EClass.screen.guide.passGuideBlock;
	}

	public override bool ForcedInstaComplete(TaskBuild t)
	{
		return this.recipe.tileType.CanInstaComplete;
	}

	public override void OnUpdateCursor()
	{
		base.SetCursorOnMap(CursorSystem.Build);
	}

	public override void OnActivate()
	{
		this.list = base.Designations.build;
		base.OnActivate();
	}

	public override void OnDeactivate()
	{
		base.Summary.SetRecipe(null);
		this.recipe = (this.mold.recipe = null);
		this.houseBoard = null;
		if (BuildMenu.Instance)
		{
			BuildMenu.Instance.Unselect();
		}
	}

	public override void OnCancel()
	{
		ActionMode.Inspect.Activate(true, false);
	}

	public void StartBuild(Recipe r, Func<ButtonGrid> _button)
	{
		this.recipe = r;
		this.button = _button;
		if (!base.IsActive)
		{
			base.Activate(true, false);
		}
		base.CreateNewMold(false);
		this.bridgeHeight = -1;
		this.SetAltitude(0);
		base.Summary.SetRecipe(r);
		EClass.ui.hud.hint.Refresh();
	}

	public override void OnCreateMold(bool processing = false)
	{
		this.mold.recipe = (processing ? this._recipe : this.recipe);
		this.SetAltitude(this.altitude);
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (start != null)
		{
			this.mold.bridgeHeight = (int)start.cell.bridgeHeight;
		}
		else
		{
			this.mold.bridgeHeight = (int)point.cell.bridgeHeight;
		}
		if (this.recipe == null)
		{
			return HitResult.Default;
		}
		if (this.IsRoofEditMode(null))
		{
			return HitResult.Valid;
		}
		if (this.IsFillMode())
		{
			Cell cell = point.cell;
			if (this.recipe.IsBridge)
			{
				if (cell._bridge != start.cell._bridge)
				{
					return HitResult.NoTarget;
				}
				return HitResult.Valid;
			}
			else if (this.recipe.IsFloor)
			{
				if (cell._floor != start.cell._floor)
				{
					return HitResult.NoTarget;
				}
				return HitResult.Valid;
			}
			else if (this.recipe.IsBlock)
			{
				if (cell._block != start.cell._block)
				{
					return HitResult.NoTarget;
				}
				return HitResult.Valid;
			}
		}
		if (this.recipe.IsBridge && this.recipe.tileType.SelectType == BaseTileSelector.SelectType.Multiple && !point.Equals(start) && (int)point.cell.height > this.bridgeHeight + this.altitude)
		{
			return HitResult.Invalid;
		}
		return base.HitTest(point, start);
	}

	public void FixBridge(Point point, Recipe recipe)
	{
		this.bridgeHeight = -1;
		if (recipe.IsBridge)
		{
			this.bridgeHeight = (int)((byte)((point.cell.bridgeHeight == 0) ? ((int)point.cell.height + recipe.tileType.MinAltitude) : ((int)point.cell.bridgeHeight)));
		}
	}

	public unsafe override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (this.recipe == null)
		{
			return;
		}
		if (this.recipe is RecipeBridgePillar)
		{
			base.OnRenderTile(point, result, dir);
			return;
		}
		if (EClass.screen.tileSelector.start == null)
		{
			this.FixBridge(point, this.recipe);
		}
		if (this.recipe.IsFloorOrBridge && !this.recipe.IsBridge && point.cell.bridgeHeight != 0)
		{
			base.OnRenderTileFloor(point, result);
			return;
		}
		if (result != HitResult.Valid && result != HitResult.Warning)
		{
			base.OnRenderTile(point, result, dir);
			return;
		}
		if (this.bridgeHeight != -1)
		{
			Vector3 vector = *point.Position();
			EClass.screen.guide.passGuideFloor.Add(vector.x, vector.y, vector.z - 0.01f, (float)result, 0f);
		}
		int desiredDir = this.recipe.tileType.GetDesiredDir(point, this.recipe._dir);
		if (desiredDir != -1)
		{
			dir = (this.recipe._dir = desiredDir);
		}
		bool main = !base.tileSelector.multisize || (base.tileSelector.firstInMulti && base.Summary.count == base.Summary.countValid);
		this.recipe.OnRenderMarker(point, false, result, main, dir, this.bridgeHeight + ((this.recipe.tileType.AltitudeAsDir && !this.IsRoofEditMode(null)) ? 0 : this.altitude));
	}

	public override void OnBeforeProcessTiles()
	{
		this._recipe = this.recipe;
		this._recipe._dir = this.recipe._dir;
		this.mold.dir = this.recipe._dir;
		base.OnBeforeProcessTiles();
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		if (dir != -1)
		{
			this.mold.dir = dir;
		}
		else
		{
			this.mold.dir = this.recipe._dir;
		}
		if (point.HasDesignation)
		{
			TaskBuild taskBuild = point.detail.designation as TaskBuild;
			if (taskBuild != null && this.recipe.IsWallOrFence && taskBuild.recipe.IsWallOrFence && taskBuild.dir != dir)
			{
				dir = (this.mold.dir = 2);
			}
			CellDetail detail = point.detail;
			if (detail != null)
			{
				detail.designation.Destroy();
			}
		}
		this.mold.bridgeHeight = this.bridgeHeight;
		this.SetAltitude(this.altitude);
		base.OnProcessTiles(point, dir);
	}

	public override void OnFinishProcessTiles()
	{
		base.OnFinishProcessTiles();
		ButtonGrid buttonGrid = this.button();
		if (this.recipe.UseStock)
		{
			Thing thing = this.recipe.ingredients[0].thing;
			if (thing == null || thing.isDestroyed || thing.ExistsOnMap || thing.Num <= 0)
			{
				ActionMode.ignoreSound = true;
				if (buttonGrid)
				{
					buttonGrid.selected = false;
					buttonGrid.DoNormalTransition(true);
				}
				BuildMenu.Instance.Unselect();
				BuildMenu.Instance.RefreshCategory(EClass.player.pref.lastBuildCategory);
				ActionMode.Inspect.Activate(true, false);
				BuildMenu.Instance.Unselect();
				return;
			}
		}
		if (buttonGrid)
		{
			buttonGrid.SetRecipe();
		}
		if (this.recipe.IsBridge)
		{
			this.SetAltitude(0);
		}
	}

	public override void RotateUnderMouse()
	{
		if (this.recipe != null && this.recipe.CanRotate)
		{
			SE.Rotate();
			this.recipe.Rotate();
			return;
		}
	}

	public int MaxAltitude
	{
		get
		{
			if (!this.IsRoofEditMode(null) || !(this.recipe.renderRow is SourceBlock.Row))
			{
				return this.recipe.MaxAltitude;
			}
			return 63;
		}
	}

	public override void InputWheel(int wheel)
	{
		if (EInput.isAltDown || EInput.isCtrlDown)
		{
			return;
		}
		if (this.recipe.MaxAltitude > 0)
		{
			this.ModAltitude(wheel);
			EClass.screen.tileSelector.RefreshMouseInfo(true);
			return;
		}
		base.InputWheel(wheel);
	}

	public void ModAltitude(int a)
	{
		this.altitude += a;
		if (this.altitude < (this.recipe.IsBridge ? -10 : 0))
		{
			this.altitude = this.MaxAltitude;
		}
		if (this.altitude > this.MaxAltitude)
		{
			this.altitude = (this.recipe.IsBridge ? -10 : 0);
		}
		this.SetAltitude(this.altitude);
	}

	public void SetAltitude(int a)
	{
		this.mold.altitude = a;
		this.altitude = a;
		this.recipe.OnChangeAltitude(a);
		if (this.recipe.tileType.AltitudeAsDir && !this.IsRoofEditMode(null))
		{
			this.mold.dir = (this.recipe._dir = this.altitude);
		}
	}

	public int bridgeHeight;

	public int altitude;

	public Recipe recipe;

	public Recipe _recipe;

	public Func<ButtonGrid> button;

	public TraitHouseBoard houseBoard;
}
