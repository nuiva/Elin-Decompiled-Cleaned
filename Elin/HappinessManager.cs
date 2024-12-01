using System.Collections.Generic;
using Newtonsoft.Json;

public class HappinessManager : EClass
{
	[JsonProperty]
	public List<Happiness> list = new List<Happiness>();

	public FactionBranch owner;

	public Happiness residents => list[0];

	public Happiness livestocks => list[1];

	public void SetOwner(FactionBranch _owner)
	{
		owner = _owner;
		if (list.Count == 0)
		{
			for (int i = 0; i < 4; i++)
			{
				list.Add(new Happiness
				{
					value = 50,
					lastValue = 50
				});
			}
		}
		for (int j = 0; j < 4; j++)
		{
			list[j].SetOwner(owner, j.ToEnum<FactionMemberType>());
		}
	}

	public void OnSimulateDay()
	{
		foreach (Happiness item in list)
		{
			item.OnAdvanceDay();
		}
	}
}
