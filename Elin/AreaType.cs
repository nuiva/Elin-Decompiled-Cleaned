using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class AreaType : EClass
{
	public virtual bool CanAssign
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsWork
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsPublicArea
	{
		get
		{
			return true;
		}
	}

	public virtual bool IsPrison
	{
		get
		{
			return false;
		}
	}

	public virtual string RandomNameGroup
	{
		get
		{
			return "";
		}
	}

	public string langHeader
	{
		get
		{
			return "listAssign".lang(this.owner.Name, null, null, null, null);
		}
	}

	public SourceArea.Row source
	{
		get
		{
			SourceArea.Row result;
			if ((result = this._source) == null)
			{
				result = (this._source = (EClass.sources.areas.map.TryGetValue(this.id, null) ?? EClass.sources.areas.map["Room"]));
			}
			return result;
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

	[JsonProperty]
	public HashSet<int> uidCharas = new HashSet<int>();

	[JsonProperty]
	public string id = "Public";

	public BaseArea owner;

	public SourceArea.Row _source;
}
