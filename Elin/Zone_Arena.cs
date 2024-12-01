public class Zone_Arena : Zone
{
	public override bool RestrictBuild => true;

	public override bool AllowCriminal => true;

	public override bool ScaleMonsterLevel => base._dangerLv >= 51;

	public override bool MakeTownProperties => true;

	public override bool UseFog => true;
}
