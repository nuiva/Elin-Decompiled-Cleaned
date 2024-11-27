using System;

public class AM_Dig : AM_Designation<TaskDig>
{
	public override int CostMoney
	{
		get
		{
			return 10;
		}
	}

	public override string id
	{
		get
		{
			return "Dig" + this.mode.ToString();
		}
	}

	public override bool AllowAutoClick
	{
		get
		{
			return true;
		}
	}

	public override bool ForcedInstaComplete(TaskDig t)
	{
		return t.pos.sourceFloor.tileType.CanInstaComplete && this.mode == TaskDig.Mode.RemoveFloor;
	}

	public override void OnUpdateCursor()
	{
		base.SetCursorOnMap(CursorSystem.Dig);
	}

	public void Activate(TaskDig.Mode _mode)
	{
		TaskDig.Mode mode = this.mode;
		this.mode = _mode;
		this.ramp = 3;
		base.Activate(false, mode != this.mode);
	}

	public override void OnActivate()
	{
		this.list = base.Designations.dig;
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

	public TaskDig.Mode mode;

	public int ramp = 3;
}
