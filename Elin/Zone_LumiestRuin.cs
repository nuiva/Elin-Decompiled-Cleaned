public class Zone_LumiestRuin : Zone
{
	public override bool UseFog => base.lv <= 0;

	public override void OnActivate()
	{
		_ = base.visitCount;
	}
}
