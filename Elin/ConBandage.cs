public class ConBandage : ConHOT
{
	public override void OnStart()
	{
		base.OnStart();
		owner.CureCondition<ConBleed>(10);
	}
}
