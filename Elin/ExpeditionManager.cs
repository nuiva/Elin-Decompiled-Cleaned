using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class ExpeditionManager : EClass
{
	public void SetOwner(FactionBranch _branch)
	{
		this.branch = _branch;
		foreach (Expedition expedition in this.dict.Values)
		{
			expedition.SetOwner(this.branch);
		}
	}

	public void Add(Expedition ex)
	{
		this.dict[ex.uidChara] = ex;
		ex.Start();
	}

	public void OnSimulateHour()
	{
		this.dict.Values.ToList<Expedition>().ForeachReverse(delegate(Expedition e)
		{
			e.OnAdvanceHour();
		});
	}

	[JsonProperty]
	public Dictionary<int, Expedition> dict = new Dictionary<int, Expedition>();

	public FactionBranch branch;
}
