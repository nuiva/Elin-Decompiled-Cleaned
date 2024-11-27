using System;
using UnityEngine;

public class AM_Mine : AM_Designation<TaskMine>
{
	public override bool IsRoofEditMode(Card c = null)
	{
		return Input.GetKey(KeyCode.LeftAlt);
	}

	public override int CostMoney
	{
		get
		{
			return 10;
		}
	}

	public override BaseTileSelector.HitType hitType
	{
		get
		{
			return BaseTileSelector.HitType.Block;
		}
	}

	public override string id
	{
		get
		{
			return "Mine" + this.mode.ToString();
		}
	}

	public override bool AllowAutoClick
	{
		get
		{
			return true;
		}
	}

	public override void OnUpdateCursor()
	{
		base.SetCursorOnMap(CursorSystem.Mine);
	}

	public override bool ForcedInstaComplete(TaskMine t)
	{
		return t.pos.sourceBlock.tileType.CanInstaComplete;
	}

	public override bool CanTargetFog
	{
		get
		{
			return false;
		}
	}

	public override MeshPass GetGuidePass(Point point)
	{
		if ((this.IsRoofEditMode(null) && point.cell._roofBlock != 0) || (!this.IsRoofEditMode(null) && point.HasWallOrFence))
		{
			return EClass.screen.guide.passGuideBlock;
		}
		return base.GetGuidePass(point);
	}

	public void Activate(TaskMine.Mode _mode)
	{
		this.mode = _mode;
		this.ramp = 3;
		base.Activate(true, false);
	}

	public override void OnActivate()
	{
		this.list = base.Designations.mine;
		base.OnActivate();
	}

	public override void OnSelectStart(Point point)
	{
		EClass.ui.hud.hint.groupRadio.ToggleInteractable(false);
	}

	public override void OnSelectEnd(bool cancel)
	{
		EClass.ui.hud.hint.groupRadio.ToggleInteractable(true);
	}

	public override void OnCreateMold(bool processing = false)
	{
		this.mold.mode = this.mode;
		this.mold.ramp = this.ramp;
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (!this.IsRoofEditMode(null) && point.cell.HasWallOrFence && (result == HitResult.Valid || result == HitResult.Invalid))
		{
			EClass.screen.guide.DrawWall(point, (result == HitResult.Valid) ? EClass.Colors.blockColors.Valid : EClass.Colors.blockColors.Warning, false, 0f);
			return;
		}
		base.OnRenderTile(point, result, dir);
	}

	public TaskMine.Mode mode;

	public int ramp = 3;
}
