using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_Steal : AI_TargetCard
{
	public override TargetType TargetType
	{
		get
		{
			return TargetType.SelfAndNeighbor;
		}
	}

	public override bool IsValidTC(Card c)
	{
		return !EClass._zone.IsUserZone && !(c.isThing & EClass._zone is Zone_LittleGarden) && (c.isNPCProperty || !c.isThing) && c.trait.CanBeStolen && c.c_lockLv <= 0 && (c.isThing || !c.IsPCFaction);
	}

	public override int MaxRadius
	{
		get
		{
			return 2;
		}
	}

	public override bool IsHostileAct
	{
		get
		{
			return true;
		}
	}

	public override bool CanPerform()
	{
		return Act.TC != null;
	}

	public override bool Perform()
	{
		this.target = Act.TC;
		return base.Perform();
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		Chara chara = this.target.Chara;
		if (chara != null)
		{
			this.target = chara.things.FindStealable();
			if (this.target == null && chara.GetInt(30, null) < EClass.world.date.GetRaw(0))
			{
				if (EClass.rnd(2) == 0)
				{
					this.target = chara.AddThing(ThingGen.Create("money", -1, -1).SetNum(1 + EClass.rnd(chara.LV * 10)), true, -1, -1);
				}
				else
				{
					this.target = chara.AddThing(ThingGen.CreateFromFilter("steal", chara.LV), true, -1, -1);
					chara.SetInt(30, EClass.world.date.GetRaw(0) + 1440);
				}
			}
			if (this.target == null)
			{
				this.owner.Say("steal_chara_nothing", this.owner, chara, null, null);
				yield return this.Cancel();
			}
			this.owner.Say("steal_chara", this.owner, chara, null, null);
		}
		else
		{
			if (this.target.things.Count > 0)
			{
				Thing thing = this.target.things.FindStealable();
				if (thing != null)
				{
					this.target = thing;
				}
			}
			this.owner.Say("steal_thing", this.owner, this.target, null, null);
		}
		ICardParent targetParent = this.target.parent;
		Card card = chara;
		if (card == null)
		{
			card = this.target;
		}
		Card root = card.GetRootCard();
		Progress_Custom progress_Custom = new Progress_Custom();
		progress_Custom.canProgress = (() => this.target.parent == targetParent && (chara == null || chara.ExistsOnMap));
		progress_Custom.onProgressBegin = delegate()
		{
		};
		Func<Chara, bool> <>9__4;
		progress_Custom.onProgress = delegate(Progress_Custom p)
		{
			this.owner.LookAt(root);
			this.owner.PlaySound("steal", 1f, true);
			root.renderer.PlayAnime(AnimeID.Shiver, default(Vector3), false);
			if (EClass.debug.godMode)
			{
				return;
			}
			Chara chara;
			if (chara != null && this.owner.Dist(chara) > 1)
			{
				EClass.pc.TryMoveTowards(chara.pos);
				if (this.owner == null)
				{
					p.Cancel();
					return;
				}
				if (chara != null && this.owner.Dist(chara) > 1)
				{
					EClass.pc.Say("targetTooFar", null, null);
					p.Cancel();
					return;
				}
			}
			if (this.target.ChildrenAndSelfWeight > this.owner.Evalue(281) * 200 + this.owner.STR * 100 + 1000)
			{
				EClass.pc.Say("tooHeavy", this.target, null, null);
				p.Cancel();
				return;
			}
			int count = this.owner.pos.ListWitnesses(this.owner, 4, WitnessType.crime, chara).Count;
			Point pos = this.owner.pos;
			Chara owner = this.owner;
			chara = chara;
			int radius = 4;
			Func<Chara, bool> funcWitness;
			if ((funcWitness = <>9__4) == null)
			{
				funcWitness = (<>9__4 = delegate(Chara c)
				{
					int num = c.CanSee(this.owner) ? 0 : 30;
					int num2 = c.PER * 250 / 100;
					if (this.target.isThing && (this.target.Thing.isEquipped || this.target.IsRangedWeapon || this.target.IsThrownWeapon))
					{
						num2 *= 2;
						if (this.target.rarity >= Rarity.Legendary)
						{
							num2 *= 2;
						}
						if (this.target.rarity >= Rarity.Artifact)
						{
							num2 *= 2;
						}
					}
					if (c.IsUnique)
					{
						num2 *= 2;
					}
					return EClass.rnd(num2) > this.owner.Evalue(281) + this.owner.DEX + num;
				});
			}
			if (pos.TryWitnessCrime(owner, chara, radius, funcWitness))
			{
				p.Cancel();
				return;
			}
			this.owner.elements.ModExp(281, Mathf.Min(count * 5 + 5, 25), false);
		};
		progress_Custom.onProgressComplete = delegate()
		{
			if (this.target.isThing && this.target.IsInstalled)
			{
				this.target.SetPlaceState(PlaceState.roaming, false);
			}
			this.owner.Say("steal_end", this.owner, this.target, null, null);
			if (chara != null && (chara.IsPCFaction || chara.OriginalHostility >= Hostility.Friend))
			{
				EClass.player.ModKarma(-1);
			}
			else if (chara == null || chara.hostility > Hostility.Enemy)
			{
				EClass.player.ModKarma(-1);
			}
			this.target.isNPCProperty = false;
			if (!this.target.category.IsChildOf("currency"))
			{
				this.target.isStolen = true;
			}
			this.owner.Pick(this.target.Thing, true, true);
			this.owner.elements.ModExp(281, 50, false);
			if (EClass.rnd(2) == 0)
			{
				EClass.pc.stamina.Mod(-1);
			}
		};
		Progress_Custom seq = progress_Custom.SetDuration(20, 4);
		yield return base.Do(seq, null);
		yield break;
	}
}
