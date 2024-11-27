using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class HappinessManager : EClass
{
	public Happiness residents
	{
		get
		{
			return this.list[0];
		}
	}

	public Happiness livestocks
	{
		get
		{
			return this.list[1];
		}
	}

	public void SetOwner(FactionBranch _owner)
	{
		this.owner = _owner;
		if (this.list.Count == 0)
		{
			for (int i = 0; i < 4; i++)
			{
				this.list.Add(new Happiness
				{
					value = 50,
					lastValue = 50
				});
			}
		}
		for (int j = 0; j < 4; j++)
		{
			this.list[j].SetOwner(this.owner, j.ToEnum<FactionMemberType>());
		}
	}

	public void OnSimulateDay()
	{
		foreach (Happiness happiness in this.list)
		{
			happiness.OnAdvanceDay();
		}
	}

	[JsonProperty]
	public List<Happiness> list = new List<Happiness>();

	public FactionBranch owner;
}
