using System;
using UnityEngine;

public class Stats : BaseStats
{
	public override int GetValue()
	{
		return this.value;
	}

	public virtual int value
	{
		get
		{
			return this.raw[this.rawIndex];
		}
		set
		{
			this.raw[this.rawIndex] = value;
		}
	}

	public virtual int max
	{
		get
		{
			return 100;
		}
		set
		{
		}
	}

	public string name
	{
		get
		{
			return base.source.GetText("name", false);
		}
	}

	public virtual bool TrackPhaseChange
	{
		get
		{
			return false;
		}
	}

	public virtual int min
	{
		get
		{
			return 0;
		}
	}

	public override string ToString()
	{
		return string.Concat(new string[]
		{
			this.name,
			" ",
			this.value.ToString(),
			" ",
			this.GetPhase().ToString()
		});
	}

	public override string GetText()
	{
		string text = base.source.GetTextArray("strPhase")[this.GetPhase()];
		if (!(text == "#"))
		{
			return text;
		}
		return "";
	}

	public override Color GetColor(Gradient g)
	{
		return g.Evaluate((float)this.value / (float)this.max);
	}

	public Stats Set(int[] _raw, int _rawIndex, Chara _CC)
	{
		this.raw = _raw;
		this.rawIndex = _rawIndex;
		BaseStats.CC = _CC;
		return this;
	}

	public override int GetPhase()
	{
		return base.source.phase[(int)Mathf.Clamp(10f * (float)this.value / (float)this.max, 0f, 9f)];
	}

	public virtual void Set(int a)
	{
		this.value = a;
		if (this.value < this.min)
		{
			this.value = this.min;
			return;
		}
		if (this.value > this.max)
		{
			this.value = this.max;
		}
	}

	public virtual void Mod(int a)
	{
		if (BaseStats.CC.IsAgent)
		{
			return;
		}
		if (this.TrackPhaseChange)
		{
			int phase = this.GetPhase();
			this.value += a;
			if (this.value < this.min)
			{
				this.value = this.min;
			}
			else if (this.value > this.max)
			{
				this.value = this.max;
			}
			int phase2 = this.GetPhase();
			if (phase2 != phase)
			{
				this.OnChangePhase(phase2, phase);
				return;
			}
		}
		else
		{
			this.value += a;
			if (this.value < this.min)
			{
				this.value = this.min;
				return;
			}
			if (this.value > this.max)
			{
				this.value = this.max;
			}
		}
	}

	public virtual void OnChangePhase(int phase, int lastPhase)
	{
		bool flag = phase > lastPhase;
		if (base.source.invert)
		{
			flag = !flag;
		}
		string[] array = (flag ? base.source.GetText("textPhase", false) : base.source.GetText("textPhase2", false)).Split(Environment.NewLine.ToCharArray());
		if (array.Length <= phase)
		{
			return;
		}
		if (flag)
		{
			Msg.SetColor("negative");
		}
		BaseStats.CC.Say(array[phase].Split('|', StringSplitOptions.None).RandomItem<string>(), BaseStats.CC, null, null);
		if (BaseStats.CC.ShouldShowMsg)
		{
			base.PopText();
		}
	}

	public static StatsHunger Hunger = new StatsHunger
	{
		id = 0
	};

	public static StatsBurden Burden = new StatsBurden
	{
		id = 1
	};

	public static StatsStamina Stamina = new StatsStamina
	{
		id = 3
	};

	public static StatsSleepiness Sleepiness = new StatsSleepiness
	{
		id = 4
	};

	public static Stats Depression = new Stats
	{
		id = 5
	};

	public static Stats Bladder = new Stats
	{
		id = 6
	};

	public static Stats Hygiene = new Stats
	{
		id = 7
	};

	public static StatsMana Mana = new StatsMana
	{
		id = 8
	};

	public static StatsSAN SAN = new StatsSAN
	{
		id = 13
	};

	public int rawIndex;

	public int[] raw;
}
