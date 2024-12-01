using UnityEngine;

public class TargetType : EClass
{
	public static TargetTypeAny Any = new TargetTypeAny();

	public static TargetTypeSelf Self = new TargetTypeSelf();

	public static TargetTypeSelfParty SelfParty = new TargetTypeSelfParty();

	public static TargetTypeSelect Select = new TargetTypeSelect();

	public static TargetTypeSelfAndNeighbor SelfAndNeighbor = new TargetTypeSelfAndNeighbor();

	public static TargetTypeGround Ground = new TargetTypeGround();

	public static TargetTypeChara Chara = new TargetTypeChara();

	public static TargetTypeEnemy Enemy = new TargetTypeEnemy();

	public static TargetTypeCard Card = new TargetTypeCard();

	public static TargetTypeParty Party = new TargetTypeParty();

	public virtual TargetRange Range => TargetRange.Self;

	public virtual bool CanOnlyTargetEnemy => false;

	public virtual bool CanTargetGround => false;

	public virtual bool RequireChara => false;

	public virtual bool ShowMapHighlight => true;

	public virtual bool RequireLos => true;

	public virtual bool CanSelectSelf => false;

	public virtual bool CanSelectParty => false;

	public virtual bool ForceParty => false;

	public virtual Sprite IconType => null;

	public virtual int LimitDist => 999;
}
