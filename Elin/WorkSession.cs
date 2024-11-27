using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

public class WorkSession : EClass
{
	public int id
	{
		get
		{
			return this.ints[1];
		}
		set
		{
			this.ints[1] = value;
		}
	}

	public int efficiency
	{
		get
		{
			return this.ints[2];
		}
		set
		{
			this.ints[2] = value;
		}
	}

	[OnSerializing]
	private void _OnSerializing(StreamingContext context)
	{
		this.ints[0] = this._bits.ToInt();
	}

	[OnDeserialized]
	private void _OnDeserialized(StreamingContext context)
	{
		this._bits.SetInt(this.ints[0]);
	}

	public void AddOutcome(WorkOutcome o)
	{
		if (this.outcomes == null)
		{
			this.outcomes = new List<WorkOutcome>();
		}
		this.outcomes.Add(o);
	}

	[JsonProperty]
	public List<WorkOutcome> outcomes;

	[JsonProperty]
	public int[] ints = new int[5];

	public BitArray32 _bits;
}
