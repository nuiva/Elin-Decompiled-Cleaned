using System;

public class AM_Cut : AM_Designation<TaskCut>
{
	public override int CostMoney
	{
		get
		{
			return 10;
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
		base.SetCursorOnMap(CursorSystem.Cut);
	}

	public override void OnActivate()
	{
		this.list = base.Designations.cut;
		base.OnActivate();
	}
}
