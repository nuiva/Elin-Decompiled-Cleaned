using System;

public class AM_ADV_Target : AM_Adv
{
	public void Activate(Act act)
	{
		base.Activate(true, false);
	}

	public override void OnActivate()
	{
	}

	public override void _OnUpdateInput()
	{
		if (EInput.rightMouse.down)
		{
			base.Deactivate();
		}
	}
}
