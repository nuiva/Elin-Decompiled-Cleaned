using UnityEngine;

public class TargetTypeSelfParty : TargetTypeSelf
{
	public override Sprite IconType => EClass.core.refs.icons.targetAny;

	public override bool CanSelectParty => true;
}
