using System;
using UnityEngine;

public class TargetTypeSelfParty : TargetTypeSelf
{
	public override Sprite IconType
	{
		get
		{
			return EClass.core.refs.icons.targetAny;
		}
	}

	public override bool CanSelectParty
	{
		get
		{
			return true;
		}
	}
}
