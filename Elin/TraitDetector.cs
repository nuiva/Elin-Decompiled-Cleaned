using System;

public class TraitDetector : TraitItem
{
	public override bool IsTool
	{
		get
		{
			return true;
		}
	}

	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public string term
	{
		get
		{
			return this.owner.c_idRefName;
		}
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		if (p.IsSelf)
		{
			p.TrySetAct(this.LangUse, () => this.OnUse(EClass.pc), this.owner, null, 1, false, true, false);
		}
	}

	public override bool OnUse(Chara c)
	{
		Dialog.InputName("dialogDetector", this.term, delegate(bool cancel, string text)
		{
			if (!cancel)
			{
				this.owner.c_idRefName = text;
				this.Search();
			}
		}, Dialog.InputType.Default);
		return false;
	}

	public override void OnHeld()
	{
		this.Search();
	}

	public override void OnTickHeld()
	{
		this.interval--;
		if (this.interval <= 0)
		{
			this.Search();
		}
	}

	public void Search()
	{
		if (this.term.IsEmpty())
		{
			return;
		}
		Card card = null;
		int num = 999;
		foreach (Thing thing in EClass._map.things)
		{
			int num2 = EClass.pc.Dist(thing);
			if ((thing.id.ToLower().Contains(this.term.ToLower()) || thing.Name.ToLower().Contains(this.term.ToLower())) && num2 < num)
			{
				num = num2;
				card = thing;
			}
		}
		this.interval = 10;
		if (card == null)
		{
			EClass.pc.PlaySound("detect_none", 1f, true);
		}
		else
		{
			EClass.pc.PlaySound("detect_" + ((num <= 1) ? "detected" : ((num < 5) ? "near" : ((num < 15) ? "medium" : ((num < 30) ? "far" : ((num < 50) ? "veryFar" : "superFar"))))), 1f, true);
			this.interval = ((num <= 1) ? 1 : ((num < 5) ? 2 : ((num < 15) ? 4 : ((num < 30) ? 7 : 10))));
		}
		this.owner.PlayAnime(AnimeID.HitObj, false);
	}

	public override void SetName(ref string s)
	{
		if (!this.term.IsEmpty())
		{
			s = "_detect".lang(s, this.term, null, null, null);
		}
	}

	public int interval;
}
