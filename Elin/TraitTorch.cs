public class TraitTorch : Trait
{
	public override bool UseExtra
	{
		get
		{
			if (owner.isOn)
			{
				return !owner.Cell.isCurtainClosed;
			}
			return false;
		}
	}

	public override ToggleType ToggleType => ToggleType.Fire;
}
