using System;
using UnityEngine;

public class HotItemIcon : HotItem
{
	public virtual int defaultIcon
	{
		get
		{
			return 0;
		}
	}

	public override Sprite GetSprite()
	{
		return EClass.core.refs.icon_HotItem[this.defaultIcon];
	}
}
