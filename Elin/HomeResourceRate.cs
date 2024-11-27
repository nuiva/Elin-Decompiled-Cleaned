using System;

public class HomeResourceRate : BaseHomeResource
{
	public override bool IsAvailable
	{
		get
		{
			return this.value > 0;
		}
	}

	public override BaseHomeResource.ResourceGroup Group
	{
		get
		{
			return BaseHomeResource.ResourceGroup.Rate;
		}
	}

	public override void Refresh()
	{
		this.lastValue = this.value;
		this.value = this.GetDestValue();
	}

	public override void OnAdvanceDay()
	{
	}

	public virtual int GetDestValue()
	{
		return 0;
	}
}
