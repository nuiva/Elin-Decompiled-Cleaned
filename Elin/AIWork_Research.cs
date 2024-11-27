using System;

public class AIWork_Research : AIWork
{
	public override int destDist
	{
		get
		{
			return 1;
		}
	}

	public override AIAct GetWork(Point p)
	{
		return this.CreateProgress();
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		p.interval = 5;
		p.onProgressBegin = delegate()
		{
			this.owner.PlaySound("read_paper", 1f, true);
		};
	}
}
