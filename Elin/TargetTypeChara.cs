public class TargetTypeChara : TargetType
{
	public override TargetRange Range => TargetRange.Chara;

	public override bool RequireChara => true;
}
