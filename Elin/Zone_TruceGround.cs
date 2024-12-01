public class Zone_TruceGround : Zone
{
	public override bool ShouldRegenerate => Version.Get(base.version).IsBelow(0, 23, 7);

	public override bool UseFog => base.lv <= 0;
}
