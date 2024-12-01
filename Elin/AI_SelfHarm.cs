public class AI_SelfHarm : AI_PassTime
{
	public override Type type => Type.selfHarm;

	public override bool CancelWhenDamaged => false;

	public override int exp => 50;
}
