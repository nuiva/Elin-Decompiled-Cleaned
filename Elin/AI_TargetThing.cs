public class AI_TargetThing : AI_TargetCard
{
	public new Thing target
	{
		get
		{
			return base.target as Thing;
		}
		set
		{
			base.target = value;
		}
	}
}
