using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class CodexCreature : EClass
{
	public string Name
	{
		get
		{
			return this.source.GetName(null, true);
		}
	}

	public SourceChara.Row source
	{
		get
		{
			return EClass.sources.charas.map[this.id];
		}
	}

	public int numCard
	{
		get
		{
			return this._ints[0];
		}
		set
		{
			this._ints[0] = value;
		}
	}

	public int weakspot
	{
		get
		{
			return this._ints[2];
		}
		set
		{
			this._ints[2] = value;
		}
	}

	public int kills
	{
		get
		{
			return this._ints[3];
		}
		set
		{
			this._ints[3] = value;
		}
	}

	public int spawns
	{
		get
		{
			return this._ints[4];
		}
		set
		{
			this._ints[4] = value;
		}
	}

	public int BonusDropLv
	{
		get
		{
			if (this.numCard > 1)
			{
				return (int)Mathf.Sqrt((float)this.numCard * 1.5f);
			}
			return 0;
		}
	}

	public bool droppedCard
	{
		get
		{
			return (this._ints[1] & 2) != 0;
		}
		set
		{
			this._ints[1] = (value ? (this._ints[1] | 2) : (this._ints[1] & -3));
		}
	}

	public void SetImage(Image image, bool nativeSize = false)
	{
		this.source.SetImage(image, null, 0, nativeSize, 0, 0);
		image.SetActive(true);
	}

	public string GetTextBonus()
	{
		string result = "noItem".lang();
		if (this.BonusDropLv > 0)
		{
			result = "codexBonus1".lang(this.BonusDropLv.ToString() ?? "", null, null, null, null);
		}
		return result;
	}

	public string id;

	[JsonProperty]
	public int[] _ints = new int[5];
}
