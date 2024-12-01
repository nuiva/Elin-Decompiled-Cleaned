public class AM_ADV_Target : AM_Adv
{
	public void Activate(Act act)
	{
		Activate();
	}

	public override void OnActivate()
	{
	}

	public override void _OnUpdateInput()
	{
		if (EInput.rightMouse.down)
		{
			Deactivate();
		}
	}
}
