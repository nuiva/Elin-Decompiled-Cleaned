using System;
using Newtonsoft.Json;

public class RefReligion : EClass
{
	public Religion Instance
	{
		get
		{
			Religion result;
			if ((result = this._religion) == null)
			{
				result = (this._religion = EClass.game.religions.dictAll[this.uid]);
			}
			return result;
		}
	}

	public RefReligion()
	{
	}

	public RefReligion(Religion religion)
	{
		this._religion = religion;
		this.uid = religion.id;
	}

	[JsonProperty]
	public string uid;

	private Religion _religion;
}
