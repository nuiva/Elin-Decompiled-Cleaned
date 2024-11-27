using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class QuestSupplyCat : QuestSupply
{
	public SourceCategory.Row Cat
	{
		get
		{
			return EClass.sources.categories.map[this.idCat];
		}
	}

	public override string RefDrama2
	{
		get
		{
			return this.Cat.GetName();
		}
	}

	public override bool IsDestThing(Thing t)
	{
		return !t.c_isImportant && !t.isEquipped && t.category.IsChildOf(this.idCat) && t.things.Count == 0;
	}

	public override string NameDeliver
	{
		get
		{
			return this.Cat.GetName();
		}
	}

	public override void SetIdThing()
	{
		List<SourceCategory.Row> source = (from c in EClass.sources.categories.rows
		where c._parent == "meal"
		select c).ToList<SourceCategory.Row>();
		this.idCat = source.RandomItem<SourceCategory.Row>().id;
	}

	public override string GetTextProgress()
	{
		string @ref = (this.GetDestThing() != null) ? "supplyInInv".lang().TagColor(FontColor.Good, null) : "supplyNotInInv".lang();
		return "progressSupply".lang(this.Cat.GetName() + Lang.space + this.TextExtra2.IsEmpty(""), @ref, null, null, null);
	}

	[JsonProperty]
	public string idCat;
}
