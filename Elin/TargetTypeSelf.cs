using System;
using UnityEngine;

public class TargetTypeSelf : TargetType
{
	public override Sprite IconType
	{
		get
		{
			return EClass.core.refs.icons.targetSelf;
		}
	}

	public override int LimitDist
	{
		get
		{
			return 1;
		}
	}

	public override bool ShowMapHighlight
	{
		get
		{
			return false;
		}
	}

	public override bool RequireLos
	{
		get
		{
			return false;
		}
	}

	public override bool CanSelectSelf
	{
		get
		{
			return true;
		}
	}
}
