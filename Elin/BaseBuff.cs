public class BaseBuff : BaseDebuff
{
	public override bool CanManualRemove => true;

	public override int GetPhase()
	{
		return 0;
	}
}
