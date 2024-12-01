public class AIWork_Research : AIWork
{
	public override int destDist => 1;

	public override AIAct GetWork(Point p)
	{
		return CreateProgress();
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		p.interval = 5;
		p.onProgressBegin = delegate
		{
			owner.PlaySound("read_paper");
		};
	}
}
