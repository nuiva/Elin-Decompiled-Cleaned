using UnityEngine;

public class TaskHarvest : BaseTaskHarvest
{
	public bool wasReapSeed;

	public bool wasCrime;

	public HarvestType mode = HarvestType.Obj;

	public bool IsObj => mode == HarvestType.Obj;

	public bool IsReapSeed
	{
		get
		{
			if (IsObj && CanReapSeed && owner.Tool != null)
			{
				if (!(owner.Tool.trait is TraitToolSickle))
				{
					return owner.Tool.category.IsChildOf("scythe");
				}
				return true;
			}
			return false;
		}
	}

	public override HarvestType harvestType => mode;

	public override int RightHand => 1005;

	public override int destDist => 1;

	public override bool IsGrowth => pos.growth != null;

	public override bool IsHostileAct
	{
		get
		{
			if (!wasCrime)
			{
				if (mode == HarvestType.Disassemble)
				{
					Thing thing = target;
					if (thing != null && thing.isNPCProperty)
					{
						goto IL_007b;
					}
				}
				if (mode == HarvestType.Obj)
				{
					if (!pos.sourceObj.ContainsTag("crime"))
					{
						if (pos.growth != null && pos.growth.IsCrimeToHarvest)
						{
							return !(EClass._zone is Zone_Harvest);
						}
						return false;
					}
					return true;
				}
				return false;
			}
			goto IL_007b;
			IL_007b:
			return true;
		}
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override string GetBaseText(string str)
	{
		if (!IsReapSeed)
		{
			if (mode != HarvestType.Disassemble)
			{
				if (!base.IsHarvest)
				{
					return base.GetBaseText(str);
				}
				return "actHarvest".lang();
			}
			return (HaveHarvestThing() ? "TaskDisassemble" : "TaskDisassemble_destroy").lang();
		}
		return "TaskHarvestSeed".lang();
	}

	public override string GetTextSmall(Card c)
	{
		if (IsObj)
		{
			return base.GetTextSmall(c);
		}
		if (target == null)
		{
			return "";
		}
		return target.Name;
	}

	public static TaskHarvest TryGetAct(Chara c, Point p)
	{
		Thing t = c.Tool;
		bool hasTool = t != null && (t.HasElement(225) || t.HasElement(220));
		bool hasDiggingTool = t != null && t.HasElement(230);
		if (t != null)
		{
			if (t.trait is TraitToolShears)
			{
				return null;
			}
			if (t.trait is TraitToolWaterCan)
			{
				return null;
			}
			if (t.trait is TraitToolMusic)
			{
				return null;
			}
			if (t.trait is TraitToolSickle && !p.cell.CanReapSeed())
			{
				return null;
			}
		}
		if (p.HasObj && IsValidTarget(p.sourceObj.reqHarvest))
		{
			return new TaskHarvest
			{
				pos = p.Copy()
			};
		}
		if (p.HasThing)
		{
			for (int num = p.Things.Count - 1; num >= 0; num--)
			{
				t = p.Things[num];
				if (t.trait.ReqHarvest != null && IsValidTarget(t.trait.ReqHarvest.Split(',')))
				{
					return new TaskHarvest
					{
						pos = p.Copy(),
						mode = HarvestType.Thing,
						target = t
					};
				}
			}
			for (int num2 = p.Things.Count - 1; num2 >= 0; num2--)
			{
				t = p.Things[num2];
				if (!t.isHidden && !t.isMasked && t.trait.CanBeDisassembled && c.Tool?.trait is TraitToolHammer)
				{
					return new TaskHarvest
					{
						pos = p.Copy(),
						mode = HarvestType.Disassemble,
						target = t
					};
				}
			}
		}
		return null;
		bool IsValidTarget(string[] raw)
		{
			if (raw[0] == "digging")
			{
				return hasDiggingTool;
			}
			bool num3 = p.cell.CanHarvest();
			int num4 = (num3 ? 250 : EClass.sources.elements.alias[raw[0]].id);
			bool flag = ((!num3 && num4 != 250) ? true : false);
			if (!flag && t != null && !t.trait.CanHarvest)
			{
				return false;
			}
			return !flag || hasTool;
		}
	}

	public override bool CanProgress()
	{
		if (tool != null && tool.trait is TraitToolSickle && !pos.cell.CanReapSeed())
		{
			return false;
		}
		if (IsObj)
		{
			SetTarget(owner ?? EClass.pc);
			if (base.CanProgress())
			{
				return pos.HasObj;
			}
			return false;
		}
		if (target == null || !target.ExistsOnMap)
		{
			return false;
		}
		return base.CanProgress();
	}

	public override HitResult GetHitResult()
	{
		if (IsObj)
		{
			if (base.IsHarvest)
			{
				return HitResult.Valid;
			}
			if (pos.HasObj)
			{
				return HitResult.Valid;
			}
			if (pos.HasDecal && EClass.debug.godBuild && (bool)BuildMenu.Instance)
			{
				return HitResult.Valid;
			}
			return HitResult.Default;
		}
		if (target == null || !target.ExistsOnMap)
		{
			return HitResult.Default;
		}
		return HitResult.Valid;
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		SetTarget(owner);
		string n = (IsObj ? pos.cell.GetObjName() : target.Name);
		SourceMaterial.Row mat = (IsObj ? pos.cell.matObj : target.material);
		GrowSystem growth = pos.growth;
		float num = (base.IsHarvest ? 0.5f : ((!IsObj) ? 1f : ((growth != null) ? growth.MtpProgress : 1f)));
		int exp = 50;
		wasReapSeed = IsReapSeed;
		wasCrime = IsHostileAct;
		p.textHint = n;
		p.maxProgress = (int)((float)(maxProgress * 150) * num / 100f);
		p.interval = 1;
		p.onProgressBegin = delegate
		{
			if (base.IsTooHard)
			{
				owner.Say((mode == HarvestType.Disassemble) ? "tooHardToDisassemble" : "tooHardToHarvest", owner, n);
				p.Cancel();
			}
			else if (mode == HarvestType.Disassemble)
			{
				owner.Say("disassemble_start", owner, owner.Tool, n);
			}
			else if (owner.Tool == null)
			{
				owner.Say("harvestHand_start", owner, n);
			}
			else
			{
				owner.Say("harvest_start", owner, owner.Tool, n);
			}
		};
		p.onProgress = delegate(Progress_Custom _p)
		{
			owner.LookAt(pos);
			owner.renderer.NextFrame();
			if (_p.progress % 2 == 0)
			{
				if (IsObj)
				{
					if (base.IsHarvest && growth != null)
					{
						owner.PlaySound(growth.GetSoundProgress());
						if (growth.AnimeProgress != 0)
						{
							pos.Animate(growth.AnimeProgress);
						}
						return;
					}
					pos.Animate(AnimeID.HitObj);
				}
				else
				{
					target.PlayAnime(AnimeID.HitObj);
				}
				pos.PlaySound(mat.GetSoundImpact());
				mat.PlayHitEffect(pos);
				mat.AddBlood(pos);
				effectFrame += maxProgress / 4 + 1;
				if (EClass._zone.IsCrime(owner, this))
				{
					owner.pos.TryWitnessCrime(owner);
				}
			}
		};
		p.onProgressComplete = delegate
		{
			string idRecipe = (IsObj ? pos.sourceObj.RecipeID : ((target != null) ? target.source.RecipeID : ""));
			SourceBacker.Row backerObj = EClass._map.GetBackerObj(pos);
			int num2 = ((EClass.rnd(3) != 0) ? 1 : 0);
			if (IsObj)
			{
				SourceObj.Row sourceObj = pos.sourceObj;
				bool flag = false;
				if (difficulty >= 0 && EClass.rnd(6) == 0)
				{
					flag = true;
				}
				if (difficulty >= 2 && EClass.rnd(3) == 0)
				{
					flag = true;
				}
				if (flag && growth != null)
				{
					growth.OnHitFail(owner);
				}
				if (EClass._zone is Zone_Harvest && !base.IsHarvest && pos.IsFarmField)
				{
					EClass._map.DestroyObj(pos);
					pos.SetObj();
				}
				else
				{
					if (base.IsHarvest && !IsReapSeed)
					{
						pos.growth.Harvest(owner);
					}
					else if (growth != null && !IsReapSeed)
					{
						growth.OnProgressComplete(owner);
					}
					else
					{
						EClass._map.MineObj(pos, this);
					}
					if (sourceObj.alias == "mound")
					{
						if (EClass.rnd(7) == 0)
						{
							EClass._zone.AddThing("plat", pos);
						}
						else if (EClass.rnd(3) == 0)
						{
							EClass._zone.AddCard(ThingGen.CreateFromCategory("junk"), pos);
						}
						else
						{
							EClass._zone.AddThing("bone", pos);
						}
					}
				}
			}
			else
			{
				exp = target.Num * 5;
				num2 = target.Num / 3 + EClass.rnd(target.Num / 3 + 2);
				HarvestThing();
			}
			if (EClass._zone.IsCrime(owner, this) && EClass.rnd(3) != 0)
			{
				EClass.player.ModKarma(-1);
			}
			if (backerObj != null)
			{
				if (backerObj != null && !backerObj.loot.IsEmpty() && !EClass.player.doneBackers.Contains(backerObj.id))
				{
					if (EClass.sources.cards.map.ContainsKey(backerObj.loot))
					{
						Thing thing = ThingGen.Create(backerObj.loot);
						switch (backerObj.id)
						{
						case 490:
						case 867:
						case 5160:
							thing.c_charges = 0;
							thing.c_priceFix = -100;
							break;
						case 2531:
							thing.MakeFoodFrom("cloud_electric");
							break;
						case 1027:
							thing.ChangeMaterial(25);
							thing.SetBlessedState(BlessedState.Doomed);
							thing.ChangeRarity(Rarity.Legendary);
							break;
						case 4565:
							EClass._zone.AddThing("rod", pos);
							EClass._zone.AddThing("money", pos).SetNum(121);
							break;
						case 5367:
							thing.Dye("obsidian");
							break;
						case 471:
						case 1828:
						case 5765:
							thing.SetBlessedState(BlessedState.Cursed);
							break;
						case 5529:
							thing.ChangeMaterial(25);
							break;
						case 1854:
							thing.decay = 10000;
							break;
						case 4788:
							thing.SetEncLv(1);
							break;
						case 4615:
							thing.ChangeMaterial("meat");
							break;
						}
						EClass._zone.AddCard(thing, pos);
					}
					else
					{
						Debug.LogError("exception: Backer Loot not valid:" + backerObj.id + "/" + backerObj.loot);
					}
					Debug.Log(backerObj.id + "/" + backerObj.Name + "/" + backerObj.loot);
				}
				EClass.player.doneBackers.Add(backerObj.id);
				if (!backerObj.Text.IsEmpty() && EClass.core.config.backer.Show(backerObj))
				{
					bool num3 = backerObj.type == 1;
					Msg.Say(num3 ? "backerRemain_read" : "backerTree_read");
					Msg.Say(num3 ? "backerRemain" : "backerTree", backerObj.Text);
				}
			}
			if (owner.IsPC)
			{
				EClass.pc.CalculateFOV();
			}
			owner.elements.ModExp(idEle, exp);
			if (wasReapSeed)
			{
				owner.ModExp(286, 20);
			}
			owner.stamina.Mod(-num2);
			if (owner != null && owner.IsPC)
			{
				EClass.player.recipes.ComeUpWithRecipe(idRecipe, 30);
			}
		};
	}

	public bool HaveHarvestThing()
	{
		string text = target.source.components[0].Split('|')[0].Split('/')[0];
		if (target.IsEquipmentOrRanged || target.IsAmmo)
		{
			text = target.material.thing;
		}
		if (target.trait is TraitGrave)
		{
			return false;
		}
		if (text.Contains("$") || text.Contains("#") || text.Contains("@") || text.Contains("-"))
		{
			return false;
		}
		if (text == target.id || !EClass.sources.cards.map.ContainsKey(text))
		{
			return false;
		}
		if (target.source.components.IsEmpty())
		{
			return false;
		}
		return true;
	}

	public void HarvestThing()
	{
		string text = target.source.components[0].Split('|')[0].Split('/')[0];
		if (target.IsEquipmentOrRanged || target.IsAmmo)
		{
			text = target.material.thing;
		}
		float num = target.Num;
		float num2 = 1.0999999f;
		if (text == "log" || text == "rock")
		{
			num2 = 2.1999998f;
		}
		if (target.trait is TraitAmmo)
		{
			num2 = 50f;
		}
		float num3 = num % num2;
		num /= num2;
		Debug.Log("num:" + num + " div:" + num3 + " chance:" + num2 + " check:" + (num2 - num3 + 1f));
		if (num3 > 0f && EClass.rndf(num2 - num3 + 1f) < 1f)
		{
			num += 1f;
		}
		if (target.sockets != null)
		{
			target.EjectSockets();
		}
		int decay = target.decay;
		int lV = target.LV;
		target.Die();
		if (target.trait is TraitGrave || text.Contains("$") || text.Contains("#") || text.Contains("@") || text.Contains("-") || text == target.id || !EClass.sources.cards.map.ContainsKey(text) || (int)num <= 0 || target.source.components.IsEmpty())
		{
			return;
		}
		if (target.isCopy)
		{
			text = "ash3";
		}
		CardBlueprint.Set(new CardBlueprint
		{
			fixedQuality = true
		});
		Thing thing = ThingGen.Create(text, 1, Mathf.Max(1, lV * 2 / 3));
		if (thing != null)
		{
			thing.SetNum((int)num);
			thing.ChangeMaterial(target.material);
			thing.decay = decay;
			if (thing.IsDecayed && thing.IsFood)
			{
				thing.elements.SetBase(73, -10);
			}
			EClass._map.TrySmoothPick(pos.IsBlocked ? owner.pos : pos, thing, EClass.pc);
		}
	}
}
