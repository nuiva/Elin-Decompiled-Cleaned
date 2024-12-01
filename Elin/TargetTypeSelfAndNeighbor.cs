public class TargetTypeSelfAndNeighbor : TargetType
{
	public override TargetRange Range => TargetRange.Neighbor;

	public override int LimitDist => 2;

	public override bool ShowMapHighlight => false;

	public override bool RequireLos => false;
}
