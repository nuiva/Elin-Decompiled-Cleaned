using System;
using UnityEngine;

public class TargetTypeSelect : TargetTypeChara
{
	public override Sprite IconType
	{
		get
		{
			return EClass.core.refs.icons.targetAny;
		}
	}

	public override bool CanSelectSelf
	{
		get
		{
			return true;
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
