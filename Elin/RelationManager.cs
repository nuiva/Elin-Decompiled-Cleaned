using System.Collections.Generic;
using Newtonsoft.Json;

public class RelationManager : EClass
{
	[JsonProperty]
	public Dictionary<string, Relation> dict = new Dictionary<string, Relation>();

	public void UpdateRelations()
	{
	}

	public Relation GetRelation(Chara c1, Chara c2)
	{
		string key = ((c1.uid < c2.uid) ? (c1.uid + "x" + c2.uid) : (c2.uid + "x" + c1.uid));
		Relation relation = dict.TryGetValue(key);
		if (relation == null)
		{
			relation = new Relation();
			dict.Add(key, relation);
		}
		return relation;
	}
}
