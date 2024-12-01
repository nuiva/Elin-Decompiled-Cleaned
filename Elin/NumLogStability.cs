public class NumLogStability : NumLog
{
	public override string Name => Lang.Get("stability");

	public override Gross gross => EClass.Branch.stability;
}
