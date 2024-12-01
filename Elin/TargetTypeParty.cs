public class TargetTypeParty : TargetTypeSelf
{
	public override TargetRange Range => TargetRange.Party;

	public override int LimitDist => 999;

	public override bool ForceParty => true;
}
