using System;
using Newtonsoft.Json;

public class ZoneEvent : EClass
{
	public SourceQuest.Row source
	{
		get
		{
			return EClass.sources.quests.map[this.id];
		}
	}

	public virtual string id
	{
		get
		{
			return "";
		}
	}

	public virtual string TextWidgetDate
	{
		get
		{
			return "";
		}
	}

	public virtual float roundInterval
	{
		get
		{
			return 1f;
		}
	}

	public virtual Playlist playlist
	{
		get
		{
			return null;
		}
	}

	public virtual bool debugSkip
	{
		get
		{
			return false;
		}
	}

	public virtual string RefStr2
	{
		get
		{
			return null;
		}
	}

	public virtual string RefStr3
	{
		get
		{
			return null;
		}
	}

	public virtual int hoursToKill
	{
		get
		{
			return 0;
		}
	}

	public virtual bool HasReport
	{
		get
		{
			return false;
		}
	}

	public virtual string GetText()
	{
		return Lang.ParseRaw(this.source.GetDetail().Split('|', StringSplitOptions.None)[1], "", this.RefStr2, this.RefStr3, null, null);
	}

	public string Name
	{
		get
		{
			return this.source.GetName();
		}
	}

	public void Tick(float delta)
	{
		if (this.firstTick)
		{
			this.firstTick = false;
			this.OnFirstTick();
		}
		this.OnTick();
		this.time += delta;
		if (this.time > this.roundInterval * (float)this.rounds)
		{
			this.rounds++;
			this.OnTickRound();
		}
	}

	public void Init()
	{
		this.OnInit();
	}

	public void OnSimulateHour()
	{
		this.hoursElapsed++;
		if (this.hoursToKill != 0 && this.hoursElapsed >= this.hoursToKill)
		{
			this.Kill();
			return;
		}
	}

	public virtual void OnVisit()
	{
	}

	public virtual void OnInit()
	{
	}

	public virtual void OnFirstTick()
	{
	}

	public virtual void OnTick()
	{
	}

	public virtual void OnTickRound()
	{
	}

	public virtual void OnKill()
	{
	}

	public virtual void OnCharaDie(Chara c)
	{
	}

	public virtual void OnLeaveZone()
	{
	}

	public void OnLoad(Zone _zone)
	{
		this.zone = _zone;
	}

	public void Kill()
	{
		this.zone.events.list.Remove(this);
		this.OnKill();
	}

	[JsonProperty]
	public float time;

	[JsonProperty]
	public int rounds;

	[JsonProperty]
	public int hoursElapsed;

	[JsonProperty]
	public int minElapsed;

	public Zone zone;

	public bool firstTick = true;
}
