public class AttbSpecial : Element
{
	public override bool ShowPotential => false;

	public override bool CanLink(ElementContainer owner)
	{
		return !base.IsGlobalElement;
	}
}
