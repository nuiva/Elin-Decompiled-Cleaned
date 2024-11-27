using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class RelationManager : EClass
{
	public void UpdateRelations()
	{
	}

	public Relation GetRelation(Chara c1, Chara c2)
	{
		string key = (c1.uid < c2.uid) ? (c1.uid.ToString() + "x" + c2.uid.ToString()) : (c2.uid.ToString() + "x" + c1.uid.ToString());
		Relation relation = this.dict.TryGetValue(key, null);
		if (relation == null)
		{
			relation = new Relation();
			this.dict.Add(key, relation);
		}
		return relation;
	}

	[JsonProperty]
	public Dictionary<string, Relation> dict = new Dictionary<string, Relation>();
}
