public class HomeResourceRate : BaseHomeResource
{
	public override bool IsAvailable => value > 0;

	public override ResourceGroup Group => ResourceGroup.Rate;

	public override void Refresh()
	{
		lastValue = value;
		value = GetDestValue();
	}

	public override void OnAdvanceDay()
	{
	}

	public virtual int GetDestValue()
	{
		return 0;
	}
}
