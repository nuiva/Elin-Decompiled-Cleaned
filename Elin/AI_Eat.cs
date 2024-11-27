using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_Eat : AIAct
{
	public bool IsValidTarget(Card c)
	{
		return c != null && c.trait.CanEat(this.owner);
	}

	public override bool LocalAct
	{
		get
		{
			return false;
		}
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override bool IsHostileAct
	{
		get
		{
			return this.target != null && this.target.isNPCProperty;
		}
	}

	public override void OnStart()
	{
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.target != null && (this.target.GetRootCard() == this.owner || this.target.parent == null))
		{
			this.owner.HoldCard(this.target, 1);
		}
		else if (this.target != null)
		{
			yield return base.DoGrab(this.target, 1, false, null);
		}
		else
		{
			if (!this.IsValidTarget(this.owner.held))
			{
				yield return base.DoGrab<TraitFood>();
				if (!this.IsValidTarget(this.owner.held))
				{
					yield return this.Cancel();
				}
			}
			if (this.cook)
			{
				yield return base.Do(new AI_Cook(), new Func<AIAct.Status>(base.KeepRunning));
				if (!this.IsValidTarget(this.owner.held))
				{
					yield return this.Cancel();
				}
				yield return base.DoGotoSpot<TraitHearth>(new Func<AIAct.Status>(base.KeepRunning), false);
			}
		}
		this.target = this.owner.held;
		if (this.target == null)
		{
			yield return this.Cancel();
		}
		if (EClass._zone.IsPCFaction && !this.owner.IsPCParty && this.owner.memberType != FactionMemberType.Livestock && !this.owner.noMove)
		{
			yield return base.DoGotoSpot<TraitSpotDining>(new Func<AIAct.Status>(base.KeepRunning), false);
		}
		int max = (this.target.SelfWeight < 100) ? 1 : (2 + (int)Mathf.Sqrt((float)(this.target.SelfWeight * 2 / 3)));
		int turn = 0;
		Progress_Custom seq = new Progress_Custom
		{
			cancelWhenMoved = false,
			canProgress = (() => this.IsValidTarget(this.target) && this.owner.held == this.target),
			onProgressBegin = delegate()
			{
				this.owner.Say("eat_start", this.owner, this.target.GetName(NameStyle.Full, 1), null);
				this.owner.PlaySound("eat", 1f, true);
			},
			onProgress = delegate(Progress_Custom p)
			{
				this.target.PlayAnime(AnimeID.Eat, false);
				int turn;
				if (turn == 1 && this.owner.IsPC && this.owner.hunger.GetPhase() == 0 && !EClass.debug.godFood)
				{
					this.owner.Say("eat_full", null, null);
					p.Cancel();
				}
				if (turn == 1)
				{
					foreach (Element element in this.target.elements.dict.Values)
					{
						if (!element.source.foodEffect.IsEmpty())
						{
							string[] foodEffect = element.source.foodEffect;
							if (foodEffect[0] == "poison" || foodEffect[0] == "love")
							{
								this.owner.Talk("eatWeird", null, null, false);
								break;
							}
						}
					}
					CardRow refCard = this.target.refCard;
					if (refCard != null && refCard.id == "mammoth")
					{
						EClass.player.forceTalk = true;
						this.owner.Talk("eatammoth", null, null, false);
					}
				}
				turn = turn;
				turn++;
			},
			onProgressComplete = delegate()
			{
				if (this.owner.IsPC && this.owner.hunger.GetPhase() == 0 && !EClass.debug.godFood)
				{
					this.owner.Say("eat_full", null, null);
					return;
				}
				this.owner.Say("eat_end", this.owner, this.target.GetName(NameStyle.Full, 1), null);
				this.owner.ShowEmo(Emo.happy, 0f, true);
				FoodEffect.Proc(this.owner, this.target.Thing);
				this.target.ModNum(-1, true);
			}
		}.SetDuration(max, 5);
		yield return base.Do(seq, null);
		yield break;
	}

	public Card target;

	public bool cook = true;
}
