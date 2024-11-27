using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

public class ConSleep : BadCondition
{
	public override Emo2 EmoIcon
	{
		get
		{
			return Emo2.speeing;
		}
	}

	public override int GetPhase()
	{
		return 0;
	}

	public override bool ConsumeTurn
	{
		get
		{
			return true;
		}
	}

	public override bool CancelAI
	{
		get
		{
			return false;
		}
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner, false);
		this.owner.conSleep = this;
		if (this.owner.renderer != null)
		{
			this.owner.renderer.RefreshSprite();
		}
	}

	public override void OnBeforeStart()
	{
		TraitPillow traitPillow = this.owner.pos.FindThing<TraitPillow>();
		if (traitPillow != null)
		{
			this.owner.Say("pillow", this.owner, traitPillow.owner, null, null);
		}
	}

	public override void Tick()
	{
		if (!this.succubusChecked && EClass._zone.isStarted && (base.value > 50 || this.pcSleep != 0))
		{
			ConSleep.SuccubusVisit(this.owner);
			this.succubusChecked = true;
		}
		if (this.owner.IsPC && this.pcSleep > 0)
		{
			this.pcSleep--;
			if (this.pcSleep == 0)
			{
				int phase = EClass.pc.sleepiness.GetPhase();
				int num = (phase >= 3) ? 12 : ((phase >= 2) ? 10 : ((phase >= 1) ? 7 : 5));
				phase = EClass.pc.stamina.GetPhase();
				num += ((phase == 0) ? 4 : ((phase == 1) ? 3 : ((phase == 2) ? 2 : 0)));
				num += EClass.rnd(3) - EClass.rnd(2);
				ConSleep.SuccubusSleep(EClass.pc);
				if (EClass.pc.isDead)
				{
					return;
				}
				base.value = 1;
				this.slept = true;
				EClass.ui.AddLayer<LayerSleep>().Sleep(num, this.pcBed, this.pcPillow);
			}
			return;
		}
		if (EClass.rnd(50) == 0 && !this.owner.IsPC)
		{
			if (EClass.rnd(10) == 0)
			{
				this.owner.Talk("sleepFav", (EClass.rnd(2) == 0) ? this.owner.GetFavCat().GetName().ToLower() : this.owner.GetFavFood().GetName(), null, false);
				if (!this.owner.knowFav && this.owner.isSynced)
				{
					Msg.Say("noteFav", this.owner, null, null, null);
					this.owner.knowFav = true;
				}
			}
			else
			{
				this.owner.Talk("sleep", null, null, false);
			}
		}
		if (this.owner.ai is GoalSleep && this.owner.CurrentSpan == TimeTable.Span.Sleep)
		{
			return;
		}
		this.owner.HealHP(EClass.rnd(4) + 1, HealSource.None);
		this.owner.mana.Mod(EClass.rnd(3) + 1);
		if (!this.owner.IsPC)
		{
			this.owner.stamina.Mod(EClass.rnd(2) + 1);
		}
		base.Mod(-1, false);
	}

	public static void SuccubusVisit(Chara tg)
	{
		if (tg.bio.IsUnderAge)
		{
			return;
		}
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara != tg && !chara.IsPC && EClass.rnd(3) == 0 && !chara.IsDisabled && chara.IsIdle)
			{
				Thing thing = chara.things.Find<TraitDreamBug>();
				if ((chara.HasElement(1216, 1) || thing != null) && chara.host == null && (!tg.IsPC || thing != null || EClass.rnd(200) == 0))
				{
					chara.Teleport(tg.pos, false, false);
					if (chara.Dist(tg.pos) <= 2)
					{
						if (thing != null)
						{
							tg.Say("dreambug_teleport", chara, tg, null, null);
							thing.ModCharge(-1, false);
							if (thing.c_charges <= 0)
							{
								tg.Say("dreambug_destroy", chara, thing, null, null);
								thing.Destroy();
							}
						}
						chara.SetAI(new AI_Fuck
						{
							target = tg,
							succubus = true
						});
					}
				}
			}
		}
	}

	public static void SuccubusSleep(Chara tg)
	{
		EClass._map.charas.ForeachReverse(delegate(Chara c)
		{
			AI_Fuck ai_Fuck = c.ai as AI_Fuck;
			if (ai_Fuck != null && ai_Fuck.IsRunning && ai_Fuck.target == tg)
			{
				ai_Fuck.Finish();
				ai_Fuck.Cancel();
				if (!c.HasCondition<ConSleep>())
				{
					c.AddCondition<ConSleep>(100 + EClass.rnd(100), true);
				}
			}
		});
	}

	public unsafe override void OnRemoved()
	{
		this.owner.conSleep = null;
		this.owner.renderer.RefreshSprite();
		this.owner.renderer.SetFirst(true, *this.owner.pos.PositionAuto());
		if (!this.owner.IsPC)
		{
			this.owner.sleepiness.Set(0);
			return;
		}
		if (!this.owner.isDead)
		{
			Thing thing = this.pcPillow;
			TraitPillow traitPillow = (((thing != null) ? thing.trait : null) as TraitPillow) ?? EClass.pc.pos.FindThing<TraitPillow>();
			if (this.pickup)
			{
				ConSleep.<OnRemoved>g__TryPick|20_0(this.pcBed, this.posBed);
				ConSleep.<OnRemoved>g__TryPick|20_0(this.pcPillow, this.posPillow);
			}
			if (this.slept)
			{
				Thing thing2 = EClass.pc.things.Find<TraitGrimoire>();
				if (thing2 != null && thing2.c_lockLv == 0 && !EClass._zone.IsRegion)
				{
					foreach (Thing thing3 in thing2.things.List((Thing _t) => _t.trait is TraitSpellbook || _t.trait is TraitAncientbook || _t.id == "234", false))
					{
						TraitBaseSpellbook traitBaseSpellbook = thing3.trait as TraitBaseSpellbook;
						if (!(thing3.trait is TraitAncientbook) || !thing3.isOn)
						{
							int c_charges = thing3.c_charges;
							for (int i = 0; i < c_charges; i++)
							{
								if (EClass.pc.isDead)
								{
									return;
								}
								int num = traitBaseSpellbook.GetActDuration(EClass.pc) + 1;
								bool flag = false;
								int j = 0;
								while (j < num)
								{
									if (!traitBaseSpellbook.TryProgress(new AIProgress
									{
										owner = EClass.pc
									}))
									{
										flag = true;
										if (EClass.pc.isDead)
										{
											return;
										}
										break;
									}
									else
									{
										j++;
									}
								}
								if (!flag)
								{
									traitBaseSpellbook.OnRead(EClass.pc);
								}
							}
						}
					}
				}
			}
			if (EClass.pc.isDead)
			{
				return;
			}
			if (this.slept)
			{
				EClass.player.recipes.OnSleep(traitPillow is TraitPillowEhekatl);
				EClass.player.DreamSpell();
				TraitPillowGod traitPillowGod = traitPillow as TraitPillowGod;
				if (traitPillowGod != null)
				{
					traitPillowGod.Deity.Talk("morning", null, null);
				}
			}
		}
	}

	[CompilerGenerated]
	internal static void <OnRemoved>g__TryPick|20_0(Thing t, ItemPosition pos)
	{
		if (t == null)
		{
			return;
		}
		t = EClass._map.FindThing(t.uid);
		if (t == null || !t.ExistsOnMap || t.isNPCProperty)
		{
			return;
		}
		if (pos != null)
		{
			Card card = (pos.uidContainer == EClass.pc.uid) ? EClass.pc : EClass.pc.things.Find(pos.uidContainer);
			if (card != null)
			{
				EClass.pc.PlaySound("pick_thing", 1f, true);
				EClass.pc.Say("pick_thing", EClass.pc, t, null, null);
				t = card.AddThing(t, false, pos.invX, pos.invY);
				t.invX = pos.invX;
				t.invY = pos.invY;
				LayerInventory.SetDirty(t);
				return;
			}
		}
		EClass.pc.Pick(t, true, true);
	}

	public bool succubusChecked;

	[JsonProperty]
	public int pcSleep;

	[JsonProperty]
	public Thing pcBed;

	[JsonProperty]
	public Thing pcPillow;

	[JsonProperty]
	public ItemPosition posBed;

	[JsonProperty]
	public ItemPosition posPillow;

	[JsonProperty]
	public bool pickup;

	[JsonProperty]
	public bool slept;
}
