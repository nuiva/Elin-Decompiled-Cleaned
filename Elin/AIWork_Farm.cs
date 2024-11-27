using System;

public class AIWork_Farm : AIWork
{
	public override void OnPerformWork(bool realtime)
	{
		if (!realtime)
		{
			base.SetDestination();
			this.SetDestPos();
		}
	}

	public override AIAct GetWork(Point p)
	{
		return new AI_Farm
		{
			pos = p
		};
	}
}
