using UnityEngine;

public class TargetTypeSelf : TargetType
{
	public override Sprite IconType => EClass.core.refs.icons.targetSelf;

	public override int LimitDist => 1;

	public override bool ShowMapHighlight => false;

	public override bool RequireLos => false;

	public override bool CanSelectSelf => true;
}
