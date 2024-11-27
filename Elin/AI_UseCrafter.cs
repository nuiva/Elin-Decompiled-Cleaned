using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AI_UseCrafter : AIAct
{
	public override int LeftHand
	{
		get
		{
			return 1001;
		}
	}

	public override int RightHand
	{
		get
		{
			return 1002;
		}
	}

	public override bool CanManualCancel()
	{
		return this.layer && this.layer.CanCancelAI;
	}

	public override void OnStart()
	{
		if (this.crafter.Icon != Emo.none)
		{
			this.owner.ShowEmo(this.crafter.Icon, 0f, true);
		}
	}

	public override void OnSuccess()
	{
		this.OnEnd();
	}

	public override void OnCancel()
	{
		this.OnEnd();
		if (this.layer)
		{
			this.layer.Close();
		}
	}

	public void OnEnd()
	{
		foreach (Thing thing in this.ings)
		{
			if (thing != null && thing.ExistsOnMap)
			{
				thing.isHidden = false;
				EClass.pc.Pick(thing, true, true);
			}
		}
		if (this.crafter.AutoTurnOff && this.crafter.owner.isOn)
		{
			this.crafter.Toggle(false, false);
		}
		if (!this.crafter.idSoundBG.IsEmpty())
		{
			EClass.Sound.Stop(this.crafter.idSoundBG, 0f);
		}
		if (this.layer)
		{
			this.layer.OnEndCraft();
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		for (;;)
		{
			AI_UseCrafter.<>c__DisplayClass14_0 CS$<>8__locals1 = new AI_UseCrafter.<>c__DisplayClass14_0();
			CS$<>8__locals1.<>4__this = this;
			if (this.crafter.owner.isDestroyed || !this.layer)
			{
				yield return base.Success(null);
			}
			if (!this.crafter.idSoundBG.IsEmpty())
			{
				SE.Play(this.crafter.idSoundBG);
			}
			List<Thing> targets = this.layer.GetTargets();
			CS$<>8__locals1.blessed = BlessedState.Normal;
			int num;
			for (int i = 0; i < targets.Count; i = num + 1)
			{
				Thing t = targets[i];
				if (!this.<Run>g__IsIngValid|14_0(t, i))
				{
					if (i == 0)
					{
						this.layer.ClearButtons();
					}
					else
					{
						this.layer.RefreshCurrentGrid();
					}
					yield return base.Success(null);
				}
				num = i;
			}
			if (!this.crafter.IsFuelEnough(this.num, targets, true))
			{
				Msg.Say("notEnoughFuel");
				this.layer.RefreshCurrentGrid();
				yield return base.Success(null);
			}
			this.ings = new List<Thing>();
			for (int j = 0; j < targets.Count; j++)
			{
				Thing thing = targets[j].Split(this.layer.GetReqIngredient(j));
				this.ings.Add(thing);
				if (thing.blessedState <= BlessedState.Cursed && CS$<>8__locals1.blessed > thing.blessedState)
				{
					CS$<>8__locals1.blessed = thing.blessedState;
				}
				if (thing.blessedState > BlessedState.Normal && CS$<>8__locals1.blessed == BlessedState.Normal)
				{
					CS$<>8__locals1.blessed = thing.blessedState;
				}
				if (this.crafter.IsConsumeIng)
				{
					Card card = EClass._zone.AddCard(thing, this.crafter.owner.ExistsOnMap ? this.crafter.owner.pos : this.owner.pos);
					card.altitude = (this.crafter.owner.ExistsOnMap ? 0 : 1);
					if (this.crafter.animeType == TraitCrafter.AnimeType.Microwave)
					{
						card.isHidden = true;
					}
				}
			}
			if (LayerDragGrid.Instance)
			{
				LayerDragGrid.Instance.Redraw();
			}
			CS$<>8__locals1.requireOn = (this.crafter.IsRequireFuel || this.crafter.ToggleType > ToggleType.None);
			if (CS$<>8__locals1.requireOn && !this.crafter.owner.isOn)
			{
				this.crafter.Toggle(true, false);
			}
			CS$<>8__locals1.costSP = this.crafter.GetCostSp(this);
			CS$<>8__locals1.duration = this.crafter.GetDuration(this, CS$<>8__locals1.costSP);
			Progress_Custom progress = new Progress_Custom
			{
				canProgress = delegate()
				{
					if (CS$<>8__locals1.requireOn && !CS$<>8__locals1.<>4__this.crafter.owner.isOn)
					{
						return false;
					}
					foreach (Thing thing2 in CS$<>8__locals1.<>4__this.ings)
					{
						if (thing2.isDestroyed || (CS$<>8__locals1.<>4__this.crafter.IsConsumeIng && !thing2.ExistsOnMap))
						{
							return false;
						}
					}
					if (LayerDragGrid.Instance)
					{
						InvOwnerDraglet owner = LayerDragGrid.Instance.owner;
						for (int k = 0; k < owner.numDragGrid; k++)
						{
							if (owner.buttons[k].Card == null)
							{
								return false;
							}
						}
						if (owner.numDragGrid == 2 && CS$<>8__locals1.<>4__this.ings[0] == CS$<>8__locals1.<>4__this.ings[1] && CS$<>8__locals1.<>4__this.ings[0].Num == 1)
						{
							return false;
						}
					}
					return !CS$<>8__locals1.<>4__this.crafter.owner.isDestroyed;
				},
				onProgress = delegate(Progress_Custom p)
				{
					if (this.crafter.owner.ExistsOnMap && !this.owner.pos.Equals(this.crafter.owner.pos))
					{
						this.owner.LookAt(this.crafter.owner);
					}
					this.owner.PlaySound(this.crafter.idSoundProgress, 1f, true);
					if (this.crafter.owner.ExistsOnMap)
					{
						TraitCrafter.AnimeType animeType = this.crafter.animeType;
						if (animeType - TraitCrafter.AnimeType.Microwave <= 1)
						{
							this.crafter.owner.renderer.PlayAnime(this.crafter.IdAnimeProgress, default(Vector3), false);
						}
					}
					foreach (Thing thing2 in this.ings)
					{
						thing2.renderer.PlayAnime(this.crafter.IdAnimeProgress, default(Vector3), false);
					}
				},
				onProgressComplete = delegate()
				{
					if (CS$<>8__locals1.<>4__this.crafter.StopSoundProgress)
					{
						EClass.Sound.Stop(CS$<>8__locals1.<>4__this.crafter.idSoundProgress, 0f);
					}
					CS$<>8__locals1.<>4__this.owner.PlaySound(CS$<>8__locals1.<>4__this.crafter.idSoundComplete, 1f, true);
					ElementContainer elements = CS$<>8__locals1.<>4__this.owner.elements;
					TraitCrafter traitCrafter = CS$<>8__locals1.<>4__this.crafter;
					Recipe recipe = CS$<>8__locals1.<>4__this.recipe;
					Element orCreateElement = elements.GetOrCreateElement(traitCrafter.IDReqEle(((recipe != null) ? recipe.source : null) ?? null));
					if (CS$<>8__locals1.<>4__this.recipe != null)
					{
						for (int k = 0; k < CS$<>8__locals1.<>4__this.num; k++)
						{
							CS$<>8__locals1.<>4__this.recipe.Craft(CS$<>8__locals1.blessed, k == 0, CS$<>8__locals1.<>4__this.ings, false);
						}
						EClass.Sound.Play("craft");
						Point from = CS$<>8__locals1.<>4__this.crafter.owner.ExistsOnMap ? CS$<>8__locals1.<>4__this.crafter.owner.pos : CS$<>8__locals1.<>4__this.owner.pos;
						Effect.Get("smoke").Play(from, 0f, null, null);
						Effect.Get("mine").Play(from, 0f, null, null).SetParticleColor(CS$<>8__locals1.<>4__this.recipe.GetColorMaterial().GetColor()).Emit(10 + EClass.rnd(10));
						CS$<>8__locals1.<>4__this.owner.renderer.PlayAnime(AnimeID.JumpSmall, default(Vector3), false);
					}
					else
					{
						Thing thing2 = CS$<>8__locals1.<>4__this.crafter.Craft(CS$<>8__locals1.<>4__this);
						if (thing2 != null)
						{
							if (thing2.category.ignoreBless == 0)
							{
								thing2.SetBlessedState(CS$<>8__locals1.blessed);
							}
							thing2.PlaySoundDrop(false);
							EClass._zone.AddCard(thing2, EClass.pc.pos);
							thing2.Identify(false, IDTSource.Identify);
							CS$<>8__locals1.<>4__this.owner.Pick(thing2, true, true);
						}
					}
					for (int l = 0; l < CS$<>8__locals1.<>4__this.ings.Count; l++)
					{
						if (CS$<>8__locals1.<>4__this.crafter.ShouldConsumeIng(CS$<>8__locals1.<>4__this.crafter.GetSource(CS$<>8__locals1.<>4__this), l))
						{
							CS$<>8__locals1.<>4__this.ings[l].Destroy();
						}
					}
					foreach (Thing thing3 in CS$<>8__locals1.<>4__this.ings)
					{
						if (thing3.ExistsOnMap)
						{
							CS$<>8__locals1.<>4__this.owner.Pick(thing3, true, true);
						}
					}
					if (CS$<>8__locals1.<>4__this.crafter.IsRequireFuel)
					{
						CS$<>8__locals1.<>4__this.crafter.owner.ModCharge(-CS$<>8__locals1.<>4__this.crafter.FuelCost * CS$<>8__locals1.<>4__this.num, false);
						if (CS$<>8__locals1.<>4__this.crafter.owner.c_charges <= 0)
						{
							CS$<>8__locals1.<>4__this.crafter.owner.c_charges = 0;
							CS$<>8__locals1.<>4__this.crafter.Toggle(false, false);
						}
					}
					for (int m = 0; m < CS$<>8__locals1.<>4__this.num; m++)
					{
						CS$<>8__locals1.<>4__this.owner.RemoveCondition<ConInvulnerable>();
						CS$<>8__locals1.<>4__this.owner.elements.ModExp(orCreateElement.id, CS$<>8__locals1.costSP * 12 * (100 + CS$<>8__locals1.duration * 2) / 100, false);
						CS$<>8__locals1.<>4__this.owner.stamina.Mod(-CS$<>8__locals1.costSP);
						if (CS$<>8__locals1.<>4__this.owner == null || CS$<>8__locals1.<>4__this.owner.isDead)
						{
							break;
						}
					}
				}
			}.SetDuration(CS$<>8__locals1.duration, 5);
			this.owner.SetTempHand(-1, -1);
			if (EClass.debug.godCraft)
			{
				progress.SetDuration(1, 1);
			}
			yield return base.Do(progress, null);
			if (progress.status == AIAct.Status.Fail)
			{
				yield return this.Cancel();
			}
			if (this.crafter.CloseOnComplete)
			{
				yield return this.Cancel();
			}
			if (!this.crafter.IsConsumeIng)
			{
				break;
			}
			if (!this.layer || !this.layer.RepeatAI)
			{
				goto IL_4BF;
			}
			CS$<>8__locals1 = null;
			targets = null;
			progress = null;
		}
		this.layer.ClearButtons();
		IL_4BF:
		yield break;
	}

	[CompilerGenerated]
	private bool <Run>g__IsIngValid|14_0(Thing t, int i)
	{
		if (t == null || t.isDestroyed)
		{
			return false;
		}
		Card rootCard = t.GetRootCard();
		return (rootCard == null || !rootCard.isChara || rootCard.IsPC) && (this.crafter.IsFactory || this.crafter.IsCraftIngredient(t, i));
	}

	public LayerBaseCraft layer;

	public TraitCrafter crafter;

	public Recipe recipe;

	public int num = 1;

	public List<Thing> ings = new List<Thing>();
}
