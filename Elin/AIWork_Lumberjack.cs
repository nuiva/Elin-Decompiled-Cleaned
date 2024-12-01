public class AIWork_Lumberjack : AIWork
{
	public override int destDist => 1;

	public override bool FuncWorkPoint(Point p)
	{
		return p.growth?.IsTree ?? false;
	}

	public override AIAct GetWork(Point p)
	{
		GrowSystem growth = p.growth;
		if (growth != null && !growth.IsTree)
		{
			return null;
		}
		return CreateProgress();
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		p.onProgress = delegate
		{
			owner.PlaySound(EClass.sources.things.map["log"].DefaultMaterial.GetSoundImpact());
		};
	}

	public override void OnPerformWork(bool realtime)
	{
	}
}
