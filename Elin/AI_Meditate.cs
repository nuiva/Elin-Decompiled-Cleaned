public class AI_Meditate : AI_PassTime
{
	public override Type type => Type.meditate;

	public override int turns => 300;

	public override int exp => 10;
}
