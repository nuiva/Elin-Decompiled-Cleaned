using System;
using Newtonsoft.Json;
using UnityEngine;

public class Meeting : EClass
{
	public virtual bool IsGlobalChara
	{
		get
		{
			return false;
		}
	}

	public virtual string IdChara
	{
		get
		{
			return "begger";
		}
	}

	public void SetOwner(FactionBranch _branch)
	{
		this.branch = _branch;
	}

	public void SetChara(Point pos)
	{
		if (this.IsGlobalChara)
		{
			this.chara = EClass.game.cards.globalCharas.Find(this.IdChara);
			if (this.chara == null)
			{
				Debug.Log("creating chara for meeting:" + this.IdChara);
				this.chara = CharaGen.Create(this.IdChara, -1);
				this.chara.SetGlobal();
				EClass.game.spatials.Find("somewhere").AddCard(this.chara);
			}
		}
		else
		{
			this.chara = EClass._zone.AddCard(CharaGen.Create(this.IdChara, -1), pos).Chara;
		}
		if (this.chara.currentZone != EClass.game.activeZone)
		{
			this.chara.MoveZone(EClass.game.activeZone, ZoneTransition.EnterState.Auto);
		}
		this.chara.MoveImmediate(pos, true, true);
	}

	public void Start()
	{
		EClass.pc.LookAt(this.chara);
		this.chara.LookAt(EClass.pc);
		this.PlayDrama();
		LayerDrama.Instance.SetOnKill(new Action(this.OnEndDrama));
	}

	public virtual void PlayDrama()
	{
	}

	public virtual void OnEndDrama()
	{
		if (this.chara.IsGlobal)
		{
			this.chara.MoveZone(EClass.game.spatials.Find("somewhere"), ZoneTransition.EnterState.Auto);
		}
		else
		{
			this.chara.Destroy();
		}
		EClass.Branch.meetings.CallNext();
	}

	[JsonProperty]
	public int dateExipire;

	public FactionBranch branch;

	public Chara chara;
}
