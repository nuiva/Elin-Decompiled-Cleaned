using System.Collections.Generic;
using Newtonsoft.Json;

public class HomeResourceManager : EClass
{
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

	public void SetOwner(FactionBranch _owner)
	{
		owner = _owner;
		if (worth == null)
		{
			food = new HomeResource().Create<HomeResource>(HomeResourceType.food, 0);
			money = new HomeResource().Create<HomeResource>(HomeResourceType.money, 0);
			knowledge = new HomeResource().Create<HomeResource>(HomeResourceType.knowledge, 0);
			fun = new HomeResourceFun().Create<HomeResourceFun>(HomeResourceType.fun, 50);
			culture = new HomeResourceCulture().Create<HomeResourceCulture>(HomeResourceType.culture, 50);
			medicine = new HomeResourceMedicine().Create<HomeResourceMedicine>(HomeResourceType.medicine, 50);
			safety = new HomeResourceSafety().Create<HomeResourceSafety>(HomeResourceType.safety, 50);
			industry = new HomeResourceIndustry().Create<HomeResourceIndustry>(HomeResourceType.industry, 0);
			education = new HomeResourceEducation().Create<HomeResourceEducation>(HomeResourceType.education, 0);
			nature = new HomeResourceNature().Create<HomeResourceNature>(HomeResourceType.nature, 0);
			worth = new HomeResourceWorth().Create<HomeResourceWorth>(HomeResourceType.worth, 0);
			reknown = new HomeResourceReknown().Create<HomeResourceReknown>(HomeResourceType.reknown, 0);
			karma = new HomeResourceKarma().Create<HomeResourceKarma>(HomeResourceType.karma, 0);
		}
		list = new List<BaseHomeResource>
		{
			money, food, knowledge, fun, culture, medicine, safety, industry, education, nature,
			worth, reknown, karma
		};
		foreach (BaseHomeResource item in list)
		{
			item.branch = owner;
		}
	}

	public void SetDirty()
	{
		isDirty = true;
	}

	public void OnSimulateDay()
	{
		foreach (BaseHomeResource item in list)
		{
			item.OnAdvanceDay();
		}
	}

	public void Refresh()
	{
		foreach (BaseHomeResource item in list)
		{
			item.Refresh();
		}
		isDirty = false;
	}

	public BaseHomeResource Get(string id)
	{
		return this.GetField<BaseHomeResource>(id);
	}

	public T Get<T>(string id) where T : BaseHomeResource
	{
		return this.GetField<T>(id);
	}
}
