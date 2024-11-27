using System;

public class AttbWeapon : Element
{
	public override bool CanLink(ElementContainer owner)
	{
		return !owner.IsMeleeWeapon && !base.IsGlobalElement;
	}
}
