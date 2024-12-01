public class TraitFridge : TraitContainer
{
	public override bool IsFridge => true;

	public override bool UseAltTiles => owner.isOn;

	public override int DecaySpeedChild
	{
		get
		{
			if (!owner.isOn)
			{
				return base.DecaySpeedChild;
			}
			return 0;
		}
	}
}
