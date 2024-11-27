using System;
using UnityEngine;

public class TraitAltar : Trait
{
	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override bool IsAltar
	{
		get
		{
			return true;
		}
	}

	public bool IsBranchAltar
	{
		get
		{
			return this is TraitAltarAncient;
		}
	}

	public override bool CanOnlyCarry
	{
		get
		{
			return this.IsBranchAltar;
		}
	}

	public virtual string idDeity
	{
		get
		{
			return this.owner.c_idDeity.IsEmpty("eyth");
		}
	}

	public virtual Religion Deity
	{
		get
		{
			return EClass.game.religions.Find(this.idDeity) ?? EClass.game.religions.Eyth;
		}
	}

	public string StrDeity
	{
		get
		{
			return this.Deity.NameDomain;
		}
	}

	public bool IsEyth
	{
		get
		{
			return this.idDeity == "eyth";
		}
	}

	public override void OnCreate(int lv)
	{
		this.SetDeity(base.GetParam(1, null) ?? EClass.game.religions.GetRandomReligion(true, false).id);
	}

	public override void OnImportMap()
	{
		if (this.owner.c_idDeity.IsEmpty())
		{
			this.SetDeity(base.GetParam(1, null) ?? EClass.game.religions.GetRandomReligion(true, false).id);
		}
	}

	public void SetDeity(string id)
	{
		this.owner.c_idDeity = id;
		if (this.owner.id == "altar")
		{
			this.owner.ChangeMaterial(this.Deity.source.idMaterial);
		}
	}

	public override void SetName(ref string s)
	{
		if (!this.owner.c_idDeity.IsEmpty())
		{
			s = "_of".lang(this.StrDeity, s, null, null, null);
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (this.IsBranchAltar)
		{
			return;
		}
		if ((this.IsBranchAltar && EClass.Branch.rank != 0) || !this.IsBranchAltar)
		{
			p.TrySetAct("actOffer", delegate()
			{
				LayerDragGrid.CreateOffering(this);
				return false;
			}, this.owner, null, 1, false, true, false);
		}
		if (!this.IsBranchAltar && this.Deity != EClass.pc.faith && this.Deity.CanJoin)
		{
			p.TrySetAct("actWorship", delegate()
			{
				LayerDrama.currentReligion = this.Deity;
				LayerDrama.Activate("_adv", "god", "worship", null, null, "");
				return false;
			}, this.owner, null, 1, false, true, false);
		}
	}

	public override bool CanOffer(Card c)
	{
		return (c != null && c.HasTag(CTAG.godArtifact) && c.c_idDeity == this.Deity.id) || (base.CanOffer(c) && (EClass.pc.faith.GetOfferingValue(c as Thing, -1) > 0 || c.id == "water") && !c.isCopy);
	}

	public void OnOffer(Chara c, Thing t)
	{
		if (t == null)
		{
			return;
		}
		if (t.id == "water")
		{
			if (this.Deity != EClass.pc.faith)
			{
				if (t.blessedState == BlessedState.Cursed)
				{
					Msg.SayNothingHappen();
					return;
				}
				Msg.Say("waterCurse", t, null, null, null);
				EClass.pc.PlayEffect("curse", true, 0f, default(Vector3));
				EClass.pc.PlaySound("curse3", 1f, true);
				t.SetBlessedState(BlessedState.Cursed);
				return;
			}
			else
			{
				if (t.blessedState == BlessedState.Blessed)
				{
					Msg.SayNothingHappen();
					return;
				}
				Msg.Say("waterBless", t, null, null, null);
				EClass.pc.PlayEffect("revive", true, 0f, default(Vector3));
				EClass.pc.PlaySound("revive", 1f, true);
				t.SetBlessedState(BlessedState.Blessed);
				return;
			}
		}
		else
		{
			if (!this.IsBranchAltar && EClass.pc.IsEyth)
			{
				EClass.pc.Say("god_offerEyth", this.owner, t, null, null);
				return;
			}
			EClass.pc.Say("god_offer", this.owner, t, this.Deity.Name, null);
			if (!this.CanOffer(t))
			{
				EClass.pc.Say("nothingHappens", this.owner, t, null, null);
				return;
			}
			Effect.Get("debuff").Play(this.owner.pos, 0f, null, null);
			EClass.pc.PlaySound("offering", 1f, true);
			if (this.IsBranchAltar)
			{
				Msg.Say("nothingHappens");
			}
			else if (this.IsEyth)
			{
				if (EClass.pc.IsEyth)
				{
					Msg.Say("nothingHappens");
				}
				else
				{
					Msg.Say("takeover_empty", EClass.pc.faith.Name, null, null, null);
					this.TakeOver();
					this._OnOffer(c, t, 2);
				}
			}
			else
			{
				if (t.HasTag(CTAG.godArtifact) && t.c_idDeity == this.Deity.id)
				{
					t.Destroy();
					Religion.Reforge(t.id, null, true);
					return;
				}
				if (EClass.pc.IsEyth)
				{
					Msg.Say("nothingHappens");
					return;
				}
				if (this.Deity != EClass.pc.faith)
				{
					bool flag = EClass.rnd(EClass.pc.faith.GetOfferingValue(t, t.Num)) > EClass.rnd(200);
					if (base.GetParam(1, null) != null)
					{
						Msg.Say("nothingHappens");
						return;
					}
					Msg.Say("takeover_versus", EClass.pc.faith.Name, this.Deity.Name, null, null);
					if (flag)
					{
						Msg.Say("takeover_success", EClass.pc.faith.TextGodGender, null, null, null);
						Msg.Say("takeover_success2", EClass.pc.faith.Name, null, null, null);
						this.TakeOver();
						this._OnOffer(c, t, 5);
					}
					else
					{
						Msg.Say("takeover_fail", this.Deity.Name, null, null, null);
						this.Deity.PunishTakeOver(EClass.pc);
					}
				}
				else
				{
					this._OnOffer(c, t, 0);
				}
			}
			t.Destroy();
			return;
		}
	}

	public void _OnOffer(Chara c, Thing t, int takeoverMod = 0)
	{
		bool @bool = t.GetBool(115);
		int num = this.Deity.GetOfferingValue(t, t.Num);
		num = num * (c.HasElement(1228, 1) ? 130 : 100) / 100;
		if (takeoverMod == 0)
		{
			if (num >= 200)
			{
				Msg.Say("god_offer1", t, null, null, null);
				EClass.pc.faith.Talk("offer", null, null);
			}
			else if (num >= 100)
			{
				Msg.Say("god_offer2", t, null, null, null);
			}
			else if (num >= 50)
			{
				Msg.Say("god_offer3", t, null, null, null);
			}
			else
			{
				Msg.Say("god_offer4", t, null, null, null);
			}
		}
		else
		{
			Msg.Say("god_offer1", t, null, null, null);
			num += this.Deity.GetOfferingValue(t, 1) * takeoverMod;
		}
		int num2 = Mathf.Max(c.Evalue(306), 1);
		Element orCreateElement = c.elements.GetOrCreateElement(85);
		int value = orCreateElement.Value;
		c.elements.ModExp(orCreateElement.id, num * 2 / 3, false);
		int num3 = 4;
		if (orCreateElement.vBase >= num2)
		{
			c.elements.SetBase(orCreateElement.id, num2, 0);
		}
		else
		{
			num3 = Mathf.Clamp(orCreateElement.vBase * 100 / num2 / 25, 0, 3);
		}
		if (num3 == 4 || orCreateElement.Value != value)
		{
			Msg.Say("piety" + num3.ToString(), c, c.faith.TextGodGender, null, null);
		}
		Debug.Log(string.Concat(new string[]
		{
			num.ToString(),
			"/",
			orCreateElement.Value.ToString(),
			"/",
			orCreateElement.vExp.ToString()
		}));
		if (orCreateElement.Value > num2 * 8 / 10)
		{
			c.elements.ModExp(306, num / 5, false);
		}
		c.RefreshFaithElement();
		if (@bool)
		{
			EClass.player.ModKarma(-1);
		}
	}

	public void TakeOver()
	{
		this.SetDeity(EClass.pc.faith.id);
		EClass.pc.faith.Talk("takeover", null, null);
		EClass.pc.PlayEffect("revive", true, 0f, default(Vector3));
		this.owner.PlayEffect("aura_heaven", true, 0f, default(Vector3));
	}
}
