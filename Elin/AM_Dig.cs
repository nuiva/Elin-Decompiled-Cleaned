public class AM_Dig : AM_Designation<TaskDig>
{
	public TaskDig.Mode mode;

	public int ramp = 3;

	public override int CostMoney => 10;

	public override string id => "Dig" + mode;

	public override bool AllowAutoClick => true;

	public override bool ForcedInstaComplete(TaskDig t)
	{
		if (t.pos.sourceFloor.tileType.CanInstaComplete)
		{
			return mode == TaskDig.Mode.RemoveFloor;
		}
		return false;
	}

	public override void OnUpdateCursor()
	{
		SetCursorOnMap(CursorSystem.Dig);
	}

	public void Activate(TaskDig.Mode _mode)
	{
		TaskDig.Mode mode = this.mode;
		this.mode = _mode;
		ramp = 3;
		Activate(toggle: false, mode != this.mode);
	}

	public override void OnActivate()
	{
		list = base.Designations.dig;
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
}
