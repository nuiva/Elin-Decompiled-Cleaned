using UnityEngine;

public class HotItemIcon : HotItem
{
	public virtual int defaultIcon => 0;

	public override Sprite GetSprite()
	{
		return EClass.core.refs.icon_HotItem[defaultIcon];
	}
}
