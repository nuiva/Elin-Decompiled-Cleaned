public class TargetTypeGround : TargetType
{
	public override TargetRange Range => TargetRange.Ground;

	public override bool CanTargetGround => true;
}
