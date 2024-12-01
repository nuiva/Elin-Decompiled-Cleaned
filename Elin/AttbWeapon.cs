public class AttbWeapon : Element
{
	public override bool CanLink(ElementContainer owner)
	{
		if (!owner.IsMeleeWeapon)
		{
			return !base.IsGlobalElement;
		}
		return false;
	}
}
