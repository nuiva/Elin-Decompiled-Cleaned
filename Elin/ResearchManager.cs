using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class ResearchManager : EClass
{
	public void SetOwner(FactionBranch _branch)
	{
		this.branch = _branch;
		foreach (ResearchPlan researchPlan in this.plans)
		{
			researchPlan.SetOwner(_branch);
		}
		foreach (ResearchPlan researchPlan2 in this.finished)
		{
			researchPlan2.SetOwner(_branch);
		}
	}

	public void OnSimulateDay()
	{
		this.newPlans.Clear();
	}

	public void TryAddPlans(string idResource, int lv)
	{
		IEnumerable<SourceResearch.Row> rows = EClass.sources.researches.rows;
		Func<SourceResearch.Row, bool> <>9__0;
		Func<SourceResearch.Row, bool> predicate;
		if ((predicate = <>9__0) == null)
		{
			predicate = (<>9__0 = ((SourceResearch.Row r) => r.resource.Length > 1 && r.resource[0] == idResource && r.resource[1].ToInt() == lv));
		}
		foreach (SourceResearch.Row row in rows.Where(predicate))
		{
			this.AddPlan(row.id);
		}
	}

	public bool IsListBarter(string idPlan)
	{
		SourceResearch.Row row = EClass.sources.researches.map[idPlan];
		return row.money > 0 && !this.branch.researches.HasPlan(row.id);
	}

	public bool HasPlan(string idPlan)
	{
		using (IEnumerator<ResearchPlan> enumerator = this.plans.Concat(this.finished).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.id == idPlan)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsComplete(string id, int rank = -1)
	{
		using (List<ResearchPlan>.Enumerator enumerator = this.finished.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.source.id == id)
				{
					return true;
				}
			}
		}
		if (rank != -1)
		{
			foreach (ResearchPlan researchPlan in this.plans)
			{
				if (researchPlan.source.id == id && researchPlan.rank >= rank)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void AddPlan(string id)
	{
		this.AddPlan(ResearchPlan.Create(id));
	}

	public void AddPlan(ResearchPlan p)
	{
		p.SetOwner(this.branch);
		this.plans.Add(p);
		this.newPlans.Add(p);
		WidgetPopText.Say("rewardPlan".lang(p.Name, null, null, null, null), FontColor.Default, null);
	}

	public bool CanCompletePlan(ResearchPlan p)
	{
		return this.branch.resources.knowledge.value >= p.source.tech;
	}

	public void CompletePlan(ResearchPlan p)
	{
		if (this.focused == p)
		{
			this.focused = null;
		}
		p.OnComplete();
		WidgetPopText.Say("completePlan".lang(p.Name, null, null, null, null), FontColor.Great, null);
		if (p.source.maxLv > p.rank)
		{
			p.rank++;
			p.exp = (p.lastExp = 0);
			SE.Play("good");
			return;
		}
		this.plans.Remove(p);
		this.finished.Add(p);
		SE.Play("good");
	}

	public void ShowNewPlans(Action onComplete = null)
	{
		Rand.SetSeed(EClass.game.seed + this.branch.seedPlan);
		List<ResearchPlan> list = new List<ResearchPlan>();
		foreach (SourceResearch.Row row in EClass.sources.researches.rows)
		{
			list.Add(ResearchPlan.Create(row.id));
		}
		Rand.SetSeed(-1);
		foreach (ResearchPlan researchPlan in list)
		{
		}
		EClass.core.ui.AddLayer<LayerList>().SetSize(450f, -1f).SetList2<ResearchPlan>(list, (ResearchPlan p) => p.Name, delegate(ResearchPlan p, ItemGeneral b)
		{
			this.branch.seedPlan++;
			this.branch.researches.AddPlan(p);
			Action onComplete2 = onComplete;
			if (onComplete2 == null)
			{
				return;
			}
			onComplete2();
		}, delegate(ResearchPlan p, ItemGeneral b)
		{
		}, true);
	}

	[JsonProperty]
	public List<ResearchPlan> plans = new List<ResearchPlan>();

	[JsonProperty]
	public List<ResearchPlan> finished = new List<ResearchPlan>();

	[JsonProperty]
	public List<ResearchPlan> newPlans = new List<ResearchPlan>();

	[JsonProperty]
	public ResearchPlan focused;

	public FactionBranch branch;
}
