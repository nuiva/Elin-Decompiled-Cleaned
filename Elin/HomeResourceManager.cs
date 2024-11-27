using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class HomeResourceManager : EClass
{
	public void SetOwner(FactionBranch _owner)
	{
		this.owner = _owner;
		if (this.worth == null)
		{
			this.food = new HomeResource().Create<HomeResource>(HomeResourceType.food, 0);
			this.money = new HomeResource().Create<HomeResource>(HomeResourceType.money, 0);
			this.knowledge = new HomeResource().Create<HomeResource>(HomeResourceType.knowledge, 0);
			this.fun = new HomeResourceFun().Create<HomeResourceFun>(HomeResourceType.fun, 50);
			this.culture = new HomeResourceCulture().Create<HomeResourceCulture>(HomeResourceType.culture, 50);
			this.medicine = new HomeResourceMedicine().Create<HomeResourceMedicine>(HomeResourceType.medicine, 50);
			this.safety = new HomeResourceSafety().Create<HomeResourceSafety>(HomeResourceType.safety, 50);
			this.industry = new HomeResourceIndustry().Create<HomeResourceIndustry>(HomeResourceType.industry, 0);
			this.education = new HomeResourceEducation().Create<HomeResourceEducation>(HomeResourceType.education, 0);
			this.nature = new HomeResourceNature().Create<HomeResourceNature>(HomeResourceType.nature, 0);
			this.worth = new HomeResourceWorth().Create<HomeResourceWorth>(HomeResourceType.worth, 0);
			this.reknown = new HomeResourceReknown().Create<HomeResourceReknown>(HomeResourceType.reknown, 0);
			this.karma = new HomeResourceKarma().Create<HomeResourceKarma>(HomeResourceType.karma, 0);
		}
		this.list = new List<BaseHomeResource>
		{
			this.money,
			this.food,
			this.knowledge,
			this.fun,
			this.culture,
			this.medicine,
			this.safety,
			this.industry,
			this.education,
			this.nature,
			this.worth,
			this.reknown,
			this.karma
		};
		foreach (BaseHomeResource baseHomeResource in this.list)
		{
			baseHomeResource.branch = this.owner;
		}
	}

	public void SetDirty()
	{
		this.isDirty = true;
	}

	public void OnSimulateDay()
	{
		foreach (BaseHomeResource baseHomeResource in this.list)
		{
			baseHomeResource.OnAdvanceDay();
		}
	}

	public void Refresh()
	{
		foreach (BaseHomeResource baseHomeResource in this.list)
		{
			baseHomeResource.Refresh();
		}
		this.isDirty = false;
	}

	public BaseHomeResource Get(string id)
	{
		return this.GetField(id);
	}

	public T Get<T>(string id) where T : BaseHomeResource
	{
		return this.GetField(id);
	}

	[JsonProperty]
	public HomeResource food;

	[JsonProperty]
	public HomeResource money;

	[JsonProperty]
	public HomeResource knowledge;

	[JsonProperty]
	public HomeResource influence;

	[JsonProperty]
	public HomeResourceFun fun;

	[JsonProperty]
	public HomeResourceSafety safety;

	[JsonProperty]
	public HomeResourceNature nature;

	[JsonProperty]
	public HomeResourceEducation education;

	[JsonProperty]
	public HomeResourceCulture culture;

	[JsonProperty]
	public HomeResourceIndustry industry;

	[JsonProperty]
	public HomeResourceMedicine medicine;

	[JsonProperty]
	public HomeResourceWorth worth;

	[JsonProperty]
	public HomeResourceKarma karma;

	[JsonProperty]
	public HomeResourceReknown reknown;

	private FactionBranch owner;

	public bool isDirty = true;

	public List<BaseHomeResource> list;
}
