using System;
using UnityEngine;

public class TCCensored : TC
{
	public override Vector3 FixPos
	{
		get
		{
			if (base.owner.IsPCC)
			{
				return TC._setting.censorPos.PlusY(base.owner.IsDeadOrSleeping ? -0.3f : -0.05f);
			}
			return TC._setting.censorPos;
		}
	}
}
