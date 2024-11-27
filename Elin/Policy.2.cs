using System;
using Newtonsoft.Json;
using UnityEngine;

public class Policy : EClass
{
	public Element Ele
	{
		get
		{
			return this.branch.elements.GetElement(this.id);
		}
	}

	public SourceElement.Row source
	{
		get
		{
			return EClass.sources.elements.map[this.id];
		}
	}

	public string Name
	{
		get
		{
			return this.source.GetName();
		}
	}

	public Sprite Sprite
	{
		get
		{
			return Resources.Load<Sprite>("Media/Graphics/Image/Policy/" + this.id.ToString());
		}
	}

	public int Next
	{
		get
		{
			return 100;
		}
	}

	public int Cost
	{
		get
		{
			return this.source.cost[0];
		}
	}

	public void SetOwner(FactionBranch _branch)
	{
		this.branch = _branch;
	}

	public void OnAdvanceHour(VirtualDate date)
	{
		EClass._zone.elements.ModExp(this.id, 10, false);
	}

	public void RefreshEffect(UINote note = null)
	{
		string alias = this.source.alias;
		if (!(alias == "humanRight"))
		{
			if (!(alias == "inquisition") && !(alias == "legalDrug") && !(alias == "prohibition"))
			{
				if (!(alias == "nocturnalLife"))
				{
					return;
				}
				this.ModHappiness(-20, FactionMemberType.Default, note);
			}
			return;
		}
		this.ModHappiness(20, FactionMemberType.Default, note);
		this.ModHappiness(-10, FactionMemberType.Livestock, note);
	}

	public void ModHappiness(int a, FactionMemberType type, UINote note)
	{
		if (note)
		{
			note.AddText("peHappiness".lang(("member" + type.ToString()).lang(), a.ToString() ?? "", null, null, null).TagColorGoodBad(() => a >= 0, false), FontColor.DontChange);
			return;
		}
		this.branch.happiness.list[(int)type].modPolicy += a;
	}

	public void WriteNote(UINote n)
	{
		if (this.Ele == null)
		{
			Debug.Log(this.id);
			return;
		}
		this.Ele.WriteNote(n, EClass._zone.elements, null);
		if (this.active)
		{
			n.Space(0, 1);
			n.AddText("activeFor".lang(this.days.ToString() ?? "", null, null, null, null), FontColor.DontChange);
		}
	}

	public int GetSortVal(UIList.SortMode m)
	{
		return 0;
	}

	[JsonProperty]
	public int id;

	[JsonProperty]
	public int days;

	[JsonProperty]
	public bool active;

	public FactionBranch branch;
}
