using System;

public class HotItemActionAudoDump : HotAction
{
	public override string Id
	{
		get
		{
			return "AutoDump";
		}
	}

	public override void Perform()
	{
		TaskDump.TryPerform();
	}
}
