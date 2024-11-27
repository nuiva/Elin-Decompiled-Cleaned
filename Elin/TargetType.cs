using System;
using UnityEngine;

public class TargetType : EClass
{
	public virtual TargetRange Range
	{
		get
		{
			return TargetRange.Self;
		}
	}

	public virtual bool CanOnlyTargetEnemy
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanTargetGround
	{
		get
		{
			return false;
		}
	}

	public virtual bool RequireChara
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShowMapHighlight
	{
		get
		{
			return true;
		}
	}

	public virtual bool RequireLos
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanSelectSelf
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanSelectParty
	{
		get
		{
			return false;
		}
	}

	public virtual bool ForceParty
	{
		get
		{
			return false;
		}
	}

	public virtual Sprite IconType
	{
		get
		{
			return null;
		}
	}

	public virtual int LimitDist
	{
		get
		{
			return 999;
		}
	}

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
}
