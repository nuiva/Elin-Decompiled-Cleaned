using System;
using UnityEngine;

public class HotItemHeld : HotItemThing
{
	public static bool CanChangeHeightByWheel()
	{
		return (!EClass.core.config.input.altChangeHeight || EClass.scene.actionMode.IsBuildMode || EInput.isShiftDown) && (!EClass._zone.IsRegion && EClass.pc.held != null && EClass.pc.held.trait.CanChangeHeight && HotItemHeld.taskBuild != null && HotItemHeld.recipe.MaxAltitude != 0) && HotItemHeld.taskBuild.CanPerform();
	}

	public static bool CanRotate()
	{
		return !EClass._zone.IsRegion && EClass.pc.held != null && (EClass._zone is Zone_Tent || EClass._zone.IsPCFaction || !EClass.pc.held.trait.CanBeOnlyBuiltInHome) && (!EClass._zone.RestrictBuild || EClass.pc.held.trait.CanBuildInTown) && (EClass.pc.held.trait is TraitTile || (HotItemHeld.taskBuild != null && (HotItemHeld.taskBuild.CanPerform() || !ActionMode.Adv.planAll.HasAct)));
	}

	public override Act act
	{
		get
		{
			return this.GetAct();
		}
	}

	public HotItemHeld()
	{
	}

	public HotItemHeld(Thing t)
	{
		this.thing = t;
	}

	public override void OnSetCurrentItem()
	{
		ActionMode.Build.altitude = 0;
		HotItemHeld.disableTool = false;
		if (EClass.pc.held != this.thing)
		{
			EClass.pc.HoldCard(this.thing, -1);
		}
		HotItemHeld.taskBuild = null;
		RecipeManager.BuildList();
		HotItemHeld.recipe = this.thing.trait.GetRecipe();
		if (this.thing.trait is TraitCatalyst)
		{
			this._act = (this.thing.trait as TraitCatalyst).CreateAct();
		}
		if (HotItemHeld.lastHeld != this.thing)
		{
			this.thing.trait.OnHeld();
		}
		HotItemHeld.lastHeld = this.thing;
		if (this.thing.trait is TraitToolRange)
		{
			EClass.pc.ranged = this.thing;
		}
	}

	public override void OnUnselect()
	{
		HotItemHeld.taskBuild = null;
		HotItemHeld.recipe = null;
		if (EClass.pc.held == this.thing)
		{
			EClass.pc.PickHeld(false);
		}
	}

	public Act GetAct()
	{
		if (this.thing.trait is TraitRod)
		{
			return new ActZap();
		}
		if (this.thing.trait is TraitToolRange)
		{
			return ACT.Ranged;
		}
		if (this.thing.HasElement(241, 1))
		{
			return new AI_PlayMusic();
		}
		if (this.thing.HasElement(225, 1))
		{
			return new TaskCut();
		}
		if (this.thing.HasElement(220, 1))
		{
			return new TaskMine();
		}
		if (this.thing.HasElement(230, 1))
		{
			return new TaskDig
			{
				mode = TaskDig.Mode.RemoveFloor
			};
		}
		if (this.thing.HasElement(286, 1))
		{
			return new TaskPlow();
		}
		if (this.thing.HasElement(245, 1))
		{
			return new AI_Fish();
		}
		if (this.thing.HasElement(237, 1))
		{
			return new AI_TendAnimal();
		}
		return this._act;
	}

	public Act GetSelfAct()
	{
		if (this.lost)
		{
			return null;
		}
		if (this.thing.trait.CanDrink(EClass.pc))
		{
			return new AI_Drink
			{
				target = this.thing
			};
		}
		if (this.thing.trait.CanEat(EClass.pc))
		{
			return new AI_Eat
			{
				cook = false
			};
		}
		if (this.thing.trait.CanRead(EClass.pc))
		{
			return new AI_Read
			{
				target = this.thing
			};
		}
		return null;
	}

	public override bool TrySetAct(ActPlan p)
	{
		HotItemHeld.taskBuild = null;
		if (!HotItemHeld.disableTool)
		{
			if (p.IsSelf && this.thing.trait.CanUse(EClass.pc))
			{
				return p.TrySetAct(this.thing.trait.LangUse, () => this.thing.trait.OnUse(p.cc), this.thing, null, -1, false, true, false);
			}
			if (EClass.scene.mouseTarget.target is Card && this.thing.trait.CanUse(EClass.pc, EClass.scene.mouseTarget.target as Card))
			{
				return p.TrySetAct(this.thing.trait.LangUse, () => this.thing.trait.OnUse(p.cc, EClass.scene.mouseTarget.target as Card), this.thing, null, -1, false, true, false);
			}
			if (this.thing.trait.CanUse(EClass.pc, p.pos))
			{
				return p.TrySetAct(this.thing.trait.LangUse, () => this.thing.trait.OnUse(p.cc, p.pos), this.thing, null, -1, false, true, false);
			}
			if (this.thing.trait.IsTool)
			{
				if (this.TrySetToolAct(p))
				{
					return true;
				}
			}
			else
			{
				this.thing.trait.TrySetHeldAct(p);
			}
			if (p.HasAct || this.thing.trait.IsTool)
			{
				return true;
			}
		}
		if (p.pos.Equals(EClass.pc.pos))
		{
			Act selfAct = this.GetSelfAct();
			if (selfAct != null)
			{
				p.TrySetAct(selfAct, this.thing);
				return true;
			}
		}
		bool flag = true;
		if (this.thing.trait is TraitThrown && !HotItemHeld.disableTool)
		{
			flag = false;
		}
		if (flag)
		{
			if (p.IsSelfOrNeighbor)
			{
				Thing installed = p.pos.Installed;
				if (installed != null)
				{
					Trait trait = installed.trait;
				}
				Chara tg = p.pos.FirstVisibleChara();
				if (tg != null && tg != EClass.pc && !tg.IsDisabled && tg.IsNeutralOrAbove() && EClass.pc.held != null && tg.CanAcceptGift(EClass.pc, EClass.pc.held))
				{
					string lang = "actGive";
					if (tg.Evalue(1232) > 0 && EClass.pc.held.trait is TraitDrinkMilkMother)
					{
						lang = "actMilk";
					}
					p.TrySetAct(lang, delegate()
					{
						if (!tg.IsValidGiftWeight(EClass.pc.held, 1))
						{
							tg.Talk("tooHeavy", null, null, false);
							return true;
						}
						if (EClass.core.config.game.confirmGive)
						{
							Dialog.YesNo("dialogGive".lang(EClass.pc.held.GetName(NameStyle.Full, 1), null, null, null, null), new Action(base.<TrySetAct>g__func|4), null, "yes", "no");
						}
						else
						{
							base.<TrySetAct>g__func|4();
						}
						return true;
					}, this.thing, null, 1, false, true, false);
				}
			}
			if (p.HasAct)
			{
				return true;
			}
			if (!this.thing.c_isImportant && (p.IsSelfOrNeighbor || this.thing.trait.CanExtendBuild))
			{
				Chara chara = p.pos.FirstVisibleChara();
				if ((chara == null || chara == EClass.pc || chara.IsNeutralOrAbove()) && HotItemHeld.recipe != null && (!this.thing.trait.IsThrowMainAction || EInput.isShiftDown || HotItemHeld.disableTool))
				{
					HotItemHeld.taskBuild = new TaskBuild
					{
						recipe = HotItemHeld.recipe,
						held = EClass.pc.held,
						pos = p.pos.Copy()
					};
					AM_Build build = ActionMode.Build;
					build.bridgeHeight = -1;
					build.recipe = HotItemHeld.taskBuild.recipe;
					build.mold = HotItemHeld.taskBuild;
					build.SetAltitude(HotItemHeld.recipe.tileType.AltitudeAsDir ? HotItemHeld.recipe._dir : build.altitude);
					if (HotItemHeld.recipe.IsBlock && this.thing.trait is TraitBlock && p.pos.HasBlock && !this.thing.trait.IsDoor)
					{
						p.TrySetAct("actRotateWall", delegate()
						{
							SE.Rotate();
							p.pos.cell.RotateBlock(1);
							return false;
						}, null, 1);
					}
					else
					{
						p.TrySetAct(HotItemHeld.taskBuild, null);
					}
				}
			}
		}
		if (p.HasAct)
		{
			return true;
		}
		if (ActThrow.CanThrow(EClass.pc, this.thing, null, p.pos))
		{
			ActPlan p2 = p;
			ActThrow actThrow = new ActThrow();
			actThrow.target = this.thing;
			Card card = p.pos.FindAttackTarget();
			actThrow.pcTarget = ((card != null) ? card.Chara : null);
			if (p2.TrySetAct(actThrow, this.thing))
			{
				return true;
			}
		}
		return false;
	}

	public bool TrySetToolAct(ActPlan p)
	{
		this.thing.trait.TrySetHeldAct(p);
		if (p.HasAct)
		{
			return true;
		}
		Cell cell = p.pos.cell;
		Point pos = p.pos;
		if (cell.HasBlock && (!cell.HasObj || cell.sourceObj.tileType.IsMountBlock) && TaskMine.CanMine(pos, this.thing) && p.TrySetAct(new TaskMine
		{
			pos = pos.Copy()
		}, null))
		{
			return true;
		}
		if (pos.IsHidden)
		{
			return true;
		}
		if (this.thing.trait is TraitToolRange)
		{
			if (!EClass.pc.CanSeeSimple(pos))
			{
				return true;
			}
			Card tc = null;
			foreach (Chara chara in pos.ListVisibleCharas())
			{
				if (chara.isSynced && chara.IsAliveInCurrentZone)
				{
					tc = chara;
					break;
				}
			}
			EClass.pc.ranged = this.thing;
			p.TrySetAct(ACT.Ranged, tc);
			return true;
		}
		else
		{
			if (cell.IsTopWaterAndNoSnow && this.thing.HasElement(245, 1) && p.TrySetAct(new AI_Fish
			{
				pos = pos.Copy()
			}, null))
			{
				return true;
			}
			if (p.IsNeighborBlocked)
			{
				return true;
			}
			TaskHarvest taskHarvest = TaskHarvest.TryGetAct(EClass.pc, pos);
			if (taskHarvest != null && p.TrySetAct(taskHarvest, null))
			{
				return true;
			}
			TraitToolWaterPot traitToolWaterPot = this.thing.trait as TraitToolWaterPot;
			if (cell.IsTopWaterAndNoSnow)
			{
				if (traitToolWaterPot != null && traitToolWaterPot.owner.c_charges < traitToolWaterPot.MaxCharge && p.TrySetAct(new TaskDrawWater
				{
					pot = traitToolWaterPot,
					pos = pos.Copy()
				}, null))
				{
					return true;
				}
			}
			else
			{
				if (traitToolWaterPot != null && !p.pos.HasBridge && traitToolWaterPot.owner.c_charges > 0 && pos.cell.sourceSurface.tag.Contains("soil") && p.TrySetAct(new TaskPourWater
				{
					pot = traitToolWaterPot,
					pos = pos.Copy()
				}, null))
				{
					return true;
				}
				if (this.thing.HasElement(286, 1) && p.TrySetAct(new TaskPlow
				{
					pos = pos.Copy()
				}, null))
				{
					return true;
				}
				if (this.thing.HasElement(230, 1) && p.TrySetAct(new TaskDig
				{
					pos = pos.Copy(),
					mode = TaskDig.Mode.RemoveFloor
				}, null))
				{
					return true;
				}
				if (TaskMine.CanMine(pos, this.thing) && p.TrySetAct(new TaskMine
				{
					pos = pos.Copy()
				}, null))
				{
					return true;
				}
			}
			if (pos.HasChara)
			{
				Chara firstChara = pos.FirstChara;
				Chara pc = EClass.pc;
			}
			if (this.thing.HasTag(CTAG.throwWeapon) && ActThrow.CanThrow(EClass.pc, this.thing, null, p.pos))
			{
				ActThrow actThrow = new ActThrow();
				actThrow.target = this.thing;
				Card card = p.pos.FindAttackTarget();
				actThrow.pcTarget = ((card != null) ? card.Chara : null);
				if (p.TrySetAct(actThrow, this.thing))
				{
					return true;
				}
			}
			return false;
		}
	}

	public unsafe override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (EClass.game.config.showGuideGrid)
		{
			for (int i = point.z - 1; i < point.z + 2; i++)
			{
				for (int j = point.x - 1; j < point.x + 2; j++)
				{
					EClass.screen.guide.passGuideFloor.Add(Point.shared.Set(j, i), 1f, 0f);
				}
			}
		}
		if (HotItemHeld.taskBuild == null || !HotItemHeld.taskBuild.IsRunning)
		{
			return;
		}
		if (!HotItemHeld.recipe.MultiSize && !HotItemHeld.taskBuild.CanProgress())
		{
			return;
		}
		if (!HotItemHeld.taskBuild.CanPerform(EClass.pc, null, point))
		{
			return;
		}
		EClass.screen.guide.isActive = false;
		ActionMode.Build.OnRenderTile(point, HitResult.Valid, HotItemHeld.taskBuild.recipe._dir);
		EClass.screen.guide.isActive = true;
		if (HotItemHeld.recipe.MultiSize)
		{
			point.ForeachMultiSize(HotItemHeld.recipe.W, HotItemHeld.recipe.H, delegate(Point pos, bool main)
			{
				Vector3 vector = *pos.Position();
				vector.z -= 0.01f;
				EClass.screen.guide.passGuideFloor.Add(ref vector, 0f, 0f);
			});
		}
	}

	public static Thing lastHeld;

	public static TaskBuild taskBuild;

	public static Recipe recipe;

	public static bool disableTool;
}
