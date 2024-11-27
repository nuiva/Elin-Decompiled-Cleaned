using System;

public class ConExcommunication : BaseDebuff
{
	public override bool ShouldRefresh
	{
		get
		{
			return true;
		}
	}

	public override void OnRefresh()
	{
		this.owner.RefreshFaithElement();
	}

	public override void OnRemoved()
	{
		this.owner.RefreshFaithElement();
	}
}
