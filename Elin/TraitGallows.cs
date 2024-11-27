using System;
using UnityEngine;

public class TraitGallows : TraitShackle
{
	public override Vector3 GetRestrainPos
	{
		get
		{
			return EClass.setting.render.posGallows;
		}
	}
}
