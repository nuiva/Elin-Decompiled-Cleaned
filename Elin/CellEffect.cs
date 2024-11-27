using System;
using Newtonsoft.Json;

public class CellEffect : EClass
{
	public int id
	{
		get
		{
			return this.ints[0];
		}
		set
		{
			this.ints[0] = value;
		}
	}

	public int amount
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

	public EffectId idEffect
	{
		get
		{
			return this.ints[2].ToEnum<EffectId>();
		}
		set
		{
			this.ints[2] = (int)value;
		}
	}

	public int power
	{
		get
		{
			return this.ints[3];
		}
		set
		{
			this.ints[3] = value;
		}
	}

	public int color
	{
		get
		{
			return this.ints[4];
		}
		set
		{
			this.ints[4] = value;
		}
	}

	public bool isHostileAct
	{
		get
		{
			return (this.ints[5] & 2) != 0;
		}
		set
		{
			this.ints[5] = (value ? (this.ints[5] | 2) : (this.ints[5] & -3));
		}
	}

	public bool isBlessed
	{
		get
		{
			return (this.ints[5] & 4) != 0;
		}
		set
		{
			this.ints[5] = (value ? (this.ints[5] | 4) : (this.ints[5] & -5));
		}
	}

	public bool isCursed
	{
		get
		{
			return (this.ints[5] & 8) != 0;
		}
		set
		{
			this.ints[5] = (value ? (this.ints[5] | 8) : (this.ints[5] & -9));
		}
	}

	public int idEle
	{
		get
		{
			return this.ints[6];
		}
		set
		{
			this.ints[6] = value;
		}
	}

	public string n1
	{
		get
		{
			return this.strs[0];
		}
		set
		{
			this.strs[0] = value;
		}
	}

	public SourceCellEffect.Row source
	{
		get
		{
			SourceCellEffect.Row result;
			if ((result = this._source) == null)
			{
				result = (this._source = EClass.sources.cellEffects.rows[this.id]);
			}
			return result;
		}
	}

	public bool IsFire
	{
		get
		{
			return this.id == 3;
		}
	}

	public bool IsLiquid
	{
		get
		{
			return this.id == 1 || this.id == 2 || this.id == 4;
		}
	}

	public int FireAmount
	{
		get
		{
			if (this.id != 3)
			{
				return 0;
			}
			return this.amount;
		}
	}

	public int LiquidAmount
	{
		get
		{
			if (!this.IsLiquid)
			{
				return 0;
			}
			return this.amount;
		}
	}

	public bool WillFade
	{
		get
		{
			return this.id >= 5;
		}
	}

	[JsonProperty]
	public int[] ints = new int[8];

	[JsonProperty]
	public string[] strs = new string[1];

	public SourceCellEffect.Row _source;
}
