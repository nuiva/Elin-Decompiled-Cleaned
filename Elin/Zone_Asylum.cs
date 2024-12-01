public class Zone_Asylum : Zone_Civilized
{
	public override bool UseFog => base.lv <= 0;
}
