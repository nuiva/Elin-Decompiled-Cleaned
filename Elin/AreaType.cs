using System.Collections.Generic;
using Newtonsoft.Json;

public class AreaType : EClass
{
	[JsonProperty]
	public HashSet<int> uidCharas = new HashSet<int>();

	[JsonProperty]
	public string id = "Public";

	public BaseArea owner;

	public SourceArea.Row _source;

	public virtual bool CanAssign => false;

	public virtual bool IsWork => false;

	public virtual bool IsPublicArea => true;

	public virtual bool IsPrison => false;

	public virtual string RandomNameGroup => "";

	public string langHeader => "listAssign".lang(owner.Name);

	public SourceArea.Row source
	{
		get
		{
			SourceArea.Row row = _source;
			if (row == null)
			{
				SourceArea.Row obj = EClass.sources.areas.map.TryGetValue(id) ?? EClass.sources.areas.map["Room"];
				SourceArea.Row row2 = obj;
				_source = obj;
				row = row2;
			}
			return row;
		}
	}

	public virtual AIAct GetAI()
	{
		return new AI_Idle();
	}

	public virtual int GetEfficiency()
	{
		return 80;
	}
}
