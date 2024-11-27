using System;

public class AIWork_Lumberjack : AIWork
{
	public override int destDist
	{
		get
		{
			return 1;
		}
	}

	public override bool FuncWorkPoint(Point p)
	{
		GrowSystem growth = p.growth;
		return growth != null && growth.IsTree;
	}

	public override AIAct GetWork(Point p)
	{
		GrowSystem growth = p.growth;
		if (growth != null && !growth.IsTree)
		{
			return null;
		}
		return this.CreateProgress();
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		p.onProgress = delegate(Progress_Custom _p)
		{
			this.owner.PlaySound(EClass.sources.things.map["log"].DefaultMaterial.GetSoundImpact(null), 1f, true);
		};
	}

	public override void OnPerformWork(bool realtime)
	{
	}
}
