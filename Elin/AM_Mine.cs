using UnityEngine;

public class AM_Mine : AM_Designation<TaskMine>
{
	public TaskMine.Mode mode;

	public int ramp = 3;

	public override int CostMoney => 10;

	public override BaseTileSelector.HitType hitType => BaseTileSelector.HitType.Block;

	public override string id => "Mine" + mode;

	public override bool AllowAutoClick => true;

	public override bool CanTargetFog => false;

	public override bool IsRoofEditMode(Card c = null)
	{
		return Input.GetKey(KeyCode.LeftAlt);
	}

	public override void OnUpdateCursor()
	{
		SetCursorOnMap(CursorSystem.Mine);
	}

	public override bool ForcedInstaComplete(TaskMine t)
	{
		return t.pos.sourceBlock.tileType.CanInstaComplete;
	}

	public override MeshPass GetGuidePass(Point point)
	{
		if ((IsRoofEditMode() && point.cell._roofBlock != 0) || (!IsRoofEditMode() && point.HasWallOrFence))
		{
			return EClass.screen.guide.passGuideBlock;
		}
		return base.GetGuidePass(point);
	}

	public void Activate(TaskMine.Mode _mode)
	{
		mode = _mode;
		ramp = 3;
		Activate();
	}

	public override void OnActivate()
	{
		list = base.Designations.mine;
		base.OnActivate();
	}

	public override void OnSelectStart(Point point)
	{
		EClass.ui.hud.hint.groupRadio.ToggleInteractable(enable: false);
	}

	public override void OnSelectEnd(bool cancel)
	{
		EClass.ui.hud.hint.groupRadio.ToggleInteractable(enable: true);
	}

	public override void OnCreateMold(bool processing = false)
	{
		mold.mode = mode;
		mold.ramp = ramp;
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (!IsRoofEditMode() && point.cell.HasWallOrFence && (result == HitResult.Valid || result == HitResult.Invalid))
		{
			EClass.screen.guide.DrawWall(point, (result == HitResult.Valid) ? EClass.Colors.blockColors.Valid : EClass.Colors.blockColors.Warning);
		}
		else
		{
			base.OnRenderTile(point, result, dir);
		}
	}
}
