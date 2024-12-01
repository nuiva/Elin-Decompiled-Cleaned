public class Zone_Civilized : Zone
{
	public override bool ShouldRegenerate => true;

	public override bool HasLaw => true;

	public override bool AllowCriminal => true;
}
