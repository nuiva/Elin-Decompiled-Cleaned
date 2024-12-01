using Newtonsoft.Json;
using UnityEngine;

public class Meeting : EClass
{
	[JsonProperty]
	public int dateExipire;

	public FactionBranch branch;

	public Chara chara;

	public virtual bool IsGlobalChara => false;

	public virtual string IdChara => "begger";

	public void SetOwner(FactionBranch _branch)
	{
		branch = _branch;
	}

	public void SetChara(Point pos)
	{
		if (IsGlobalChara)
		{
			chara = EClass.game.cards.globalCharas.Find(IdChara);
			if (chara == null)
			{
				Debug.Log("creating chara for meeting:" + IdChara);
				chara = CharaGen.Create(IdChara);
				chara.SetGlobal();
				EClass.game.spatials.Find("somewhere").AddCard(chara);
			}
		}
		else
		{
			chara = EClass._zone.AddCard(CharaGen.Create(IdChara), pos).Chara;
		}
		if (chara.currentZone != EClass.game.activeZone)
		{
			chara.MoveZone(EClass.game.activeZone);
		}
		chara.MoveImmediate(pos);
	}

	public void Start()
	{
		EClass.pc.LookAt(chara);
		chara.LookAt(EClass.pc);
		PlayDrama();
		LayerDrama.Instance.SetOnKill(OnEndDrama);
	}

	public virtual void PlayDrama()
	{
	}

	public virtual void OnEndDrama()
	{
		if (chara.IsGlobal)
		{
			chara.MoveZone(EClass.game.spatials.Find("somewhere"));
		}
		else
		{
			chara.Destroy();
		}
		EClass.Branch.meetings.CallNext();
	}
}
