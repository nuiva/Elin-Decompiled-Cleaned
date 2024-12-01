public class PropsStocked : Props
{
	public override PlaceState state => PlaceState.stocked;

	public override bool IsStocked => true;
}
