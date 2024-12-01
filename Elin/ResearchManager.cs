using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class ResearchManager : EClass
{
	[JsonProperty]
	public List<ResearchPlan> plans = new List<ResearchPlan>();

	[JsonProperty]
	public List<ResearchPlan> finished = new List<ResearchPlan>();

	[JsonProperty]
	public List<ResearchPlan> newPlans = new List<ResearchPlan>();

	[JsonProperty]
	public ResearchPlan focused;

	public FactionBranch branch;

	public void SetOwner(FactionBranch _branch)
	{
		branch = _branch;
		foreach (ResearchPlan plan in plans)
		{
			plan.SetOwner(_branch);
		}
		foreach (ResearchPlan item in finished)
		{
			item.SetOwner(_branch);
		}
	}

	public void OnSimulateDay()
	{
		newPlans.Clear();
	}

	public void TryAddPlans(string idResource, int lv)
	{
		foreach (SourceResearch.Row item in EClass.sources.researches.rows.Where((SourceResearch.Row r) => r.resource.Length > 1 && r.resource[0] == idResource && r.resource[1].ToInt() == lv))
		{
			AddPlan(item.id);
		}
	}

	public bool IsListBarter(string idPlan)
	{
		SourceResearch.Row row = EClass.sources.researches.map[idPlan];
		if (row.money > 0)
		{
			return !branch.researches.HasPlan(row.id);
		}
		return false;
	}

	public bool HasPlan(string idPlan)
	{
		foreach (ResearchPlan item in plans.Concat(finished))
		{
			if (item.id == idPlan)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsComplete(string id, int rank = -1)
	{
		foreach (ResearchPlan item in finished)
		{
			if (item.source.id == id)
			{
				return true;
			}
		}
		if (rank != -1)
		{
			foreach (ResearchPlan plan in plans)
			{
				if (plan.source.id == id && plan.rank >= rank)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void AddPlan(string id)
	{
		AddPlan(ResearchPlan.Create(id));
	}

	public void AddPlan(ResearchPlan p)
	{
		p.SetOwner(branch);
		plans.Add(p);
		newPlans.Add(p);
		WidgetPopText.Say("rewardPlan".lang(p.Name));
	}

	public bool CanCompletePlan(ResearchPlan p)
	{
		if (branch.resources.knowledge.value < p.source.tech)
		{
			return false;
		}
		return true;
	}

	public void CompletePlan(ResearchPlan p)
	{
		if (focused == p)
		{
			focused = null;
		}
		p.OnComplete();
		WidgetPopText.Say("completePlan".lang(p.Name), FontColor.Great);
		if (p.source.maxLv > p.rank)
		{
			p.rank++;
			p.exp = (p.lastExp = 0);
			SE.Play("good");
		}
		else
		{
			plans.Remove(p);
			finished.Add(p);
			SE.Play("good");
		}
	}

	public void ShowNewPlans(Action onComplete = null)
	{
		Rand.SetSeed(EClass.game.seed + branch.seedPlan);
		List<ResearchPlan> list = new List<ResearchPlan>();
		foreach (SourceResearch.Row row in EClass.sources.researches.rows)
		{
			list.Add(ResearchPlan.Create(row.id));
		}
		Rand.SetSeed();
		foreach (ResearchPlan item in list)
		{
			_ = item;
		}
		EClass.core.ui.AddLayer<LayerList>().SetSize().SetList2(list, (ResearchPlan p) => p.Name, delegate(ResearchPlan p, ItemGeneral b)
		{
			branch.seedPlan++;
			branch.researches.AddPlan(p);
			onComplete?.Invoke();
		}, delegate
		{
		});
	}
}
