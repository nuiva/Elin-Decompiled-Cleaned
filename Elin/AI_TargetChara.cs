public class AI_TargetChara : AI_TargetCard
{
	public new Chara target
	{
		get
		{
			return base.target as Chara;
		}
		set
		{
			base.target = value;
		}
	}
}
