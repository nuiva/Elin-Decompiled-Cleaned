using UnityEngine;

public class TargetTypeSelect : TargetTypeChara
{
	public override Sprite IconType => EClass.core.refs.icons.targetAny;

	public override bool CanSelectSelf => true;

	public override bool CanSelectParty => true;
}
