using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class SourcePref : EClass, ISerializationCallbackReceiver
{
	public int test
	{
		get
		{
			this.Validate();
			return this.ints[0];
		}
	}

	public PrefFlag flags
	{
		get
		{
			return this.ints[1].ToEnum<PrefFlag>();
		}
		set
		{
			this.ints[1] = (int)value;
		}
	}

	public int shadow
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

	public float height
	{
		get
		{
			return (float)this.ints[3] * 0.01f;
		}
		set
		{
			this.ints[3] = (int)(value * 100f);
		}
	}

	public int liquidMod
	{
		get
		{
			return this.ints[16];
		}
		set
		{
			this.ints[16] = value;
		}
	}

	public int liquidModMax
	{
		get
		{
			return this.ints[21];
		}
		set
		{
			this.ints[21] = value;
		}
	}

	public float hatY
	{
		get
		{
			return (float)this.ints[23] * 0.01f;
		}
		set
		{
			this.ints[23] = (int)(value * 100f);
		}
	}

	public float stackX
	{
		get
		{
			return (float)this.ints[17] * 0.01f;
		}
		set
		{
			this.ints[17] = (int)(value * 100f);
		}
	}

	public float x
	{
		get
		{
			return (float)this.ints[4] * 0.01f;
		}
		set
		{
			this.ints[4] = (int)(value * 100f);
		}
	}

	public float y
	{
		get
		{
			return (float)this.ints[5] * 0.01f;
		}
		set
		{
			this.ints[5] = (int)(value * 100f);
		}
	}

	public float z
	{
		get
		{
			return (float)this.ints[2] * 0.01f;
		}
		set
		{
			this.ints[2] = (int)(value * 100f);
		}
	}

	public int pivotX
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

	public int pivotY
	{
		get
		{
			return this.ints[7];
		}
		set
		{
			this.ints[7] = value;
		}
	}

	public int shadowX
	{
		get
		{
			return this.ints[8];
		}
		set
		{
			this.ints[8] = value;
		}
	}

	public int shadowY
	{
		get
		{
			return this.ints[9];
		}
		set
		{
			this.ints[9] = value;
		}
	}

	public int shadowRX
	{
		get
		{
			return this.ints[10];
		}
		set
		{
			this.ints[10] = value;
		}
	}

	public int shadowRY
	{
		get
		{
			return this.ints[11];
		}
		set
		{
			this.ints[11] = value;
		}
	}

	public int shadowBX
	{
		get
		{
			return this.ints[12];
		}
		set
		{
			this.ints[12] = value;
		}
	}

	public int shadowBY
	{
		get
		{
			return this.ints[13];
		}
		set
		{
			this.ints[13] = value;
		}
	}

	public int shadowBRX
	{
		get
		{
			return this.ints[14];
		}
		set
		{
			this.ints[14] = value;
		}
	}

	public int shadowBRY
	{
		get
		{
			return this.ints[15];
		}
		set
		{
			this.ints[15] = value;
		}
	}

	public int equipX
	{
		get
		{
			return this.ints[18];
		}
		set
		{
			this.ints[18] = value;
		}
	}

	public int equipY
	{
		get
		{
			return this.ints[19];
		}
		set
		{
			this.ints[19] = value;
		}
	}

	public int scaleIcon
	{
		get
		{
			return this.ints[20];
		}
		set
		{
			this.ints[20] = value;
		}
	}

	public bool bypassShadow
	{
		get
		{
			return this._bits1[0];
		}
		set
		{
			this._bits1[0] = value;
		}
	}

	public bool UsePref
	{
		get
		{
			return (this.ints[1] & 1) != 0;
		}
	}

	public bool Float
	{
		get
		{
			return (this.ints[1] & 4) != 0;
		}
	}

	public bool Surface
	{
		get
		{
			return (this.ints[1] & 8) != 0;
		}
	}

	public void OnAfterDeserialize()
	{
		if (this.ints.Length >= 25)
		{
			this._bits1.SetInt(this.ints[22]);
			return;
		}
		this.Validate();
	}

	public void OnBeforeSerialize()
	{
		this.ints[22] = this._bits1.ToInt();
	}

	public void Validate()
	{
		if (this.ints.Length < 25)
		{
			Array.Resize<int>(ref this.ints, 25);
		}
	}

	[JsonProperty]
	public int[] ints = new int[25];

	public BitArray32 _bits1;
}
