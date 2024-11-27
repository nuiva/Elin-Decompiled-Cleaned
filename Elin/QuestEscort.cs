using System;
using Newtonsoft.Json;

public class QuestEscort : QuestDestZone
{
	public Chara target
	{
		get
		{
			return EClass._map.FindChara(this.uidChara);
		}
	}

	public override Quest.DifficultyType difficultyType
	{
		get
		{
			return Quest.DifficultyType.Escort;
		}
	}

	public override int KarmaOnFail
	{
		get
		{
			return -4;
		}
	}

	public override bool ForbidTeleport
	{
		get
		{
			return true;
		}
	}

	public override void OnStart()
	{
		Chara chara = CharaGen.CreateFromFilter("c_neutral", 10, -1);
		EClass._zone.AddCard(chara, EClass.pc.pos.GetNearestPoint(false, false, true, false));
		chara.MakeMinion(EClass.pc, MinionType.Default);
		this.uidChara = chara.uid;
		chara.Talk("parasite", null, null, true);
	}

	public override int GetExtraMoney()
	{
		return base.DestZone.Dist(base.ClientZone) * 6;
	}

	public override string GetTextProgress()
	{
		return "progressEscort".lang((this.target == null) ? "???" : this.target.NameSimple, base.DestZone.Name, null, null, null);
	}

	public override void OnEnterZone()
	{
		if (this.target == null || this.target.isDead)
		{
			base.Fail();
			return;
		}
		if (EClass._zone == base.DestZone)
		{
			base.Complete();
			this.target.Talk("thanks", null, null, true);
			this.ReleaseEscort();
		}
	}

	public override void OnFail()
	{
		this.ReleaseEscort();
	}

	public void ReleaseEscort()
	{
		if (this.target == null)
		{
			return;
		}
		this.target.ReleaseMinion();
		if (EClass._zone.IsRegion)
		{
			this.target.Destroy();
			return;
		}
		this.target.SetSummon(60);
	}

	[JsonProperty]
	public int uidChara;
}
