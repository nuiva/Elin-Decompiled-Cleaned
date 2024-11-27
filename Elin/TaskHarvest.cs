using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TaskHarvest : BaseTaskHarvest
{
	public bool IsObj
	{
		get
		{
			return this.mode == BaseTaskHarvest.HarvestType.Obj;
		}
	}

	public bool IsReapSeed
	{
		get
		{
			return this.IsObj && this.CanReapSeed && this.owner.Tool != null && (this.owner.Tool.trait is TraitToolSickle || this.owner.Tool.category.IsChildOf("scythe"));
		}
	}

	public override BaseTaskHarvest.HarvestType harvestType
	{
		get
		{
			return this.mode;
		}
	}

	public override int RightHand
	{
		get
		{
			return 1005;
		}
	}

	public override int destDist
	{
		get
		{
			return 1;
		}
	}

	public override bool IsGrowth
	{
		get
		{
			return this.pos.growth != null;
		}
	}

	public override bool IsHostileAct
	{
		get
		{
			if (!this.wasCrime)
			{
				if (this.mode == BaseTaskHarvest.HarvestType.Disassemble)
				{
					Thing target = this.target;
					if (target != null && target.isNPCProperty)
					{
						return true;
					}
				}
				return this.mode == BaseTaskHarvest.HarvestType.Obj && (this.pos.sourceObj.ContainsTag("crime") || (this.pos.growth != null && this.pos.growth.IsCrimeToHarvest && !(EClass._zone is Zone_Harvest)));
			}
			return true;
		}
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override string GetBaseText(string str)
	{
		if (this.IsReapSeed)
		{
			return "TaskHarvestSeed".lang();
		}
		if (this.mode == BaseTaskHarvest.HarvestType.Disassemble)
		{
			return (this.HaveHarvestThing() ? "TaskDisassemble" : "TaskDisassemble_destroy").lang();
		}
		if (!base.IsHarvest)
		{
			return base.GetBaseText(str);
		}
		return "actHarvest".lang();
	}

	public override string GetTextSmall(Card c)
	{
		if (this.IsObj)
		{
			return base.GetTextSmall(c);
		}
		if (this.target == null)
		{
			return "";
		}
		return this.target.Name;
	}

	public static TaskHarvest TryGetAct(Chara c, Point p)
	{
		TaskHarvest.<>c__DisplayClass20_0 CS$<>8__locals1;
		CS$<>8__locals1.p = p;
		CS$<>8__locals1.t = c.Tool;
		CS$<>8__locals1.hasTool = (CS$<>8__locals1.t != null && (CS$<>8__locals1.t.HasElement(225, 1) || CS$<>8__locals1.t.HasElement(220, 1)));
		CS$<>8__locals1.hasDiggingTool = (CS$<>8__locals1.t != null && CS$<>8__locals1.t.HasElement(230, 1));
		if (CS$<>8__locals1.t != null)
		{
			if (CS$<>8__locals1.t.trait is TraitToolShears)
			{
				return null;
			}
			if (CS$<>8__locals1.t.trait is TraitToolWaterCan)
			{
				return null;
			}
			if (CS$<>8__locals1.t.trait is TraitToolMusic)
			{
				return null;
			}
			if (CS$<>8__locals1.t.trait is TraitToolSickle && !CS$<>8__locals1.p.cell.CanReapSeed())
			{
				return null;
			}
		}
		if (CS$<>8__locals1.p.HasObj && TaskHarvest.<TryGetAct>g__IsValidTarget|20_0(CS$<>8__locals1.p.sourceObj.reqHarvest, ref CS$<>8__locals1))
		{
			return new TaskHarvest
			{
				pos = CS$<>8__locals1.p.Copy()
			};
		}
		if (CS$<>8__locals1.p.HasThing)
		{
			for (int i = CS$<>8__locals1.p.Things.Count - 1; i >= 0; i--)
			{
				CS$<>8__locals1.t = CS$<>8__locals1.p.Things[i];
				if (CS$<>8__locals1.t.trait.ReqHarvest != null && TaskHarvest.<TryGetAct>g__IsValidTarget|20_0(CS$<>8__locals1.t.trait.ReqHarvest.Split(',', StringSplitOptions.None), ref CS$<>8__locals1))
				{
					return new TaskHarvest
					{
						pos = CS$<>8__locals1.p.Copy(),
						mode = BaseTaskHarvest.HarvestType.Thing,
						target = CS$<>8__locals1.t
					};
				}
			}
			for (int j = CS$<>8__locals1.p.Things.Count - 1; j >= 0; j--)
			{
				CS$<>8__locals1.t = CS$<>8__locals1.p.Things[j];
				if (!CS$<>8__locals1.t.isHidden && !CS$<>8__locals1.t.isMasked && CS$<>8__locals1.t.trait.CanBeDisassembled)
				{
					Thing tool = c.Tool;
					if (((tool != null) ? tool.trait : null) is TraitToolHammer)
					{
						return new TaskHarvest
						{
							pos = CS$<>8__locals1.p.Copy(),
							mode = BaseTaskHarvest.HarvestType.Disassemble,
							target = CS$<>8__locals1.t
						};
					}
				}
			}
		}
		return null;
	}

	public override bool CanProgress()
	{
		if (this.IsObj)
		{
			base.SetTarget(this.owner ?? EClass.pc, null);
			return base.CanProgress() && this.pos.HasObj;
		}
		return this.target != null && this.target.ExistsOnMap && base.CanProgress();
	}

	public override HitResult GetHitResult()
	{
		if (this.IsObj)
		{
			if (base.IsHarvest)
			{
				return HitResult.Valid;
			}
			if (this.pos.HasObj)
			{
				return HitResult.Valid;
			}
			if (this.pos.HasDecal && EClass.debug.godBuild && BuildMenu.Instance)
			{
				return HitResult.Valid;
			}
			return HitResult.Default;
		}
		else
		{
			if (this.target == null || !this.target.ExistsOnMap)
			{
				return HitResult.Default;
			}
			return HitResult.Valid;
		}
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		base.SetTarget(this.owner, null);
		string n = this.IsObj ? this.pos.cell.GetObjName() : this.target.Name;
		SourceMaterial.Row mat = this.IsObj ? this.pos.cell.matObj : this.target.material;
		GrowSystem growth = this.pos.growth;
		float num = base.IsHarvest ? 0.5f : ((!this.IsObj) ? 1f : ((growth != null) ? growth.MtpProgress : 1f));
		int exp = 50;
		this.wasReapSeed = this.IsReapSeed;
		this.wasCrime = this.IsHostileAct;
		p.textHint = n;
		p.maxProgress = (int)((float)(this.maxProgress * 150) * num / 100f);
		p.interval = 1;
		p.onProgressBegin = delegate()
		{
			if (this.IsTooHard)
			{
				this.owner.Say((this.mode == BaseTaskHarvest.HarvestType.Disassemble) ? "tooHardToDisassemble" : "tooHardToHarvest", this.owner, n, null);
				p.Cancel();
				return;
			}
			if (this.mode == BaseTaskHarvest.HarvestType.Disassemble)
			{
				this.owner.Say("disassemble_start", this.owner, this.owner.Tool, n, null);
				return;
			}
			if (this.owner.Tool == null)
			{
				this.owner.Say("harvestHand_start", this.owner, n, null);
				return;
			}
			this.owner.Say("harvest_start", this.owner, this.owner.Tool, n, null);
		};
		p.onProgress = delegate(Progress_Custom _p)
		{
			this.owner.LookAt(this.pos);
			this.owner.renderer.NextFrame();
			if (_p.progress % 2 == 0)
			{
				if (this.IsObj)
				{
					if (this.IsHarvest && growth != null)
					{
						this.owner.PlaySound(growth.GetSoundProgress(), 1f, true);
						if (growth.AnimeProgress != AnimeID.None)
						{
							this.pos.Animate(growth.AnimeProgress, false);
						}
						return;
					}
					this.pos.Animate(AnimeID.HitObj, false);
				}
				else
				{
					this.target.PlayAnime(AnimeID.HitObj, false);
				}
				this.pos.PlaySound(mat.GetSoundImpact(null), true, 1f, true);
				mat.PlayHitEffect(this.pos);
				mat.AddBlood(this.pos, 1);
				this.effectFrame += this.maxProgress / 4 + 1;
				if (EClass._zone.IsCrime(this.owner, this))
				{
					this.owner.pos.TryWitnessCrime(this.owner, null, 4, null);
				}
			}
		};
		p.onProgressComplete = delegate()
		{
			string idRecipe = this.IsObj ? this.pos.sourceObj.RecipeID : ((this.target != null) ? this.target.source.RecipeID : "");
			SourceBacker.Row backerObj = EClass._map.GetBackerObj(this.pos);
			int num2 = (EClass.rnd(3) == 0) ? 0 : 1;
			if (this.IsObj)
			{
				SourceObj.Row sourceObj = this.pos.sourceObj;
				bool flag = false;
				if (this.difficulty >= 0 && EClass.rnd(6) == 0)
				{
					flag = true;
				}
				if (this.difficulty >= 2 && EClass.rnd(3) == 0)
				{
					flag = true;
				}
				if (flag && growth != null)
				{
					growth.OnHitFail(this.owner);
				}
				if (EClass._zone is Zone_Harvest && !this.IsHarvest && this.pos.IsFarmField)
				{
					EClass._map.DestroyObj(this.pos);
					this.pos.SetObj(0, 1, 0);
				}
				else
				{
					if (this.IsHarvest && !this.IsReapSeed)
					{
						this.pos.growth.Harvest(this.owner);
					}
					else if (growth != null && !this.IsReapSeed)
					{
						growth.OnProgressComplete(this.owner);
					}
					else
					{
						EClass._map.MineObj(this.pos, this, null);
					}
					if (sourceObj.alias == "mound")
					{
						if (EClass.rnd(7) == 0)
						{
							EClass._zone.AddThing("plat", this.pos);
						}
						else if (EClass.rnd(3) == 0)
						{
							EClass._zone.AddCard(ThingGen.CreateFromCategory("junk", -1), this.pos);
						}
						else
						{
							EClass._zone.AddThing("bone", this.pos);
						}
					}
				}
			}
			else
			{
				exp = this.target.Num * 5;
				num2 = this.target.Num / 3 + EClass.rnd(this.target.Num / 3 + 2);
				this.HarvestThing();
			}
			if (EClass._zone.IsCrime(this.owner, this) && EClass.rnd(3) != 0)
			{
				EClass.player.ModKarma(-1);
			}
			if (backerObj != null)
			{
				if (backerObj != null && !backerObj.loot.IsEmpty() && !EClass.player.doneBackers.Contains(backerObj.id))
				{
					if (EClass.sources.cards.map.ContainsKey(backerObj.loot))
					{
						Thing thing = ThingGen.Create(backerObj.loot, -1, -1);
						int id = backerObj.id;
						if (id <= 2531)
						{
							if (id <= 867)
							{
								if (id == 471)
								{
									goto IL_4BC;
								}
								if (id != 490 && id != 867)
								{
									goto IL_4F7;
								}
							}
							else if (id <= 1828)
							{
								if (id == 1027)
								{
									thing.ChangeMaterial(25);
									thing.SetBlessedState(BlessedState.Doomed);
									thing.ChangeRarity(Rarity.Legendary);
									goto IL_4F7;
								}
								if (id != 1828)
								{
									goto IL_4F7;
								}
								goto IL_4BC;
							}
							else
							{
								if (id == 1854)
								{
									thing.decay = 10000;
									goto IL_4F7;
								}
								if (id != 2531)
								{
									goto IL_4F7;
								}
								thing.MakeFoodFrom("putty");
								goto IL_4F7;
							}
						}
						else if (id <= 4788)
						{
							if (id == 4565)
							{
								EClass._zone.AddThing("rod", this.pos);
								EClass._zone.AddThing("money", this.pos).SetNum(121);
								goto IL_4F7;
							}
							if (id == 4615)
							{
								thing.ChangeMaterial("meat");
								goto IL_4F7;
							}
							if (id != 4788)
							{
								goto IL_4F7;
							}
							thing.SetEncLv(1);
							goto IL_4F7;
						}
						else if (id <= 5367)
						{
							if (id != 5160)
							{
								if (id != 5367)
								{
									goto IL_4F7;
								}
								thing.Dye("obsidian");
								goto IL_4F7;
							}
						}
						else
						{
							if (id == 5529)
							{
								thing.ChangeMaterial(25);
								goto IL_4F7;
							}
							if (id != 5765)
							{
								goto IL_4F7;
							}
							goto IL_4BC;
						}
						thing.c_charges = 0;
						thing.c_priceFix = -100;
						goto IL_4F7;
						IL_4BC:
						thing.SetBlessedState(BlessedState.Cursed);
						IL_4F7:
						EClass._zone.AddCard(thing, this.pos);
					}
					else
					{
						Debug.LogError("exception: Backer Loot not valid:" + backerObj.id.ToString() + "/" + backerObj.loot);
					}
					Debug.Log(string.Concat(new string[]
					{
						backerObj.id.ToString(),
						"/",
						backerObj.Name,
						"/",
						backerObj.loot
					}));
				}
				EClass.player.doneBackers.Add(backerObj.id);
				if (!backerObj.Text.IsEmpty() && EClass.core.config.backer.Show(backerObj))
				{
					bool flag2 = backerObj.type == 1;
					Msg.Say(flag2 ? "backerRemain_read" : "backerTree_read");
					Msg.Say(flag2 ? "backerRemain" : "backerTree", backerObj.Text, null, null, null);
				}
			}
			if (this.owner.IsPC)
			{
				EClass.pc.CalculateFOV();
			}
			this.owner.elements.ModExp(this.idEle, exp, false);
			if (this.wasReapSeed)
			{
				this.owner.ModExp(286, 20);
			}
			this.owner.stamina.Mod(-num2);
			if (this.owner == null)
			{
				return;
			}
			if (this.owner.IsPC)
			{
				EClass.player.recipes.ComeUpWithRecipe(idRecipe, 30);
			}
		};
	}

	public bool HaveHarvestThing()
	{
		string text = this.target.source.components[0].Split('|', StringSplitOptions.None)[0].Split('/', StringSplitOptions.None)[0];
		if (this.target.IsEquipmentOrRanged || this.target.IsAmmo)
		{
			text = this.target.material.thing;
		}
		return !(this.target.trait is TraitGrave) && !text.Contains("$") && !text.Contains("#") && !text.Contains("@") && !text.Contains("-") && !(text == this.target.id) && EClass.sources.cards.map.ContainsKey(text) && !this.target.source.components.IsEmpty();
	}

	public void HarvestThing()
	{
		string text = this.target.source.components[0].Split('|', StringSplitOptions.None)[0].Split('/', StringSplitOptions.None)[0];
		if (this.target.IsEquipmentOrRanged || this.target.IsAmmo)
		{
			text = this.target.material.thing;
		}
		float num = (float)this.target.Num;
		float num2 = 1.1f;
		if (text == "log" || text == "rock")
		{
			num2 = 2.2f;
		}
		if (this.target.trait is TraitAmmo)
		{
			num2 = 50f;
		}
		float num3 = num % num2;
		num /= num2;
		Debug.Log(string.Concat(new string[]
		{
			"num:",
			num.ToString(),
			" div:",
			num3.ToString(),
			" chance:",
			num2.ToString(),
			" check:",
			(num2 - num3 + 1f).ToString()
		}));
		if (num3 > 0f && EClass.rndf(num2 - num3 + 1f) < 1f)
		{
			num += 1f;
		}
		if (this.target.sockets != null)
		{
			this.target.EjectSockets();
		}
		int decay = this.target.decay;
		int lv = this.target.LV;
		this.target.Die(null, null, AttackSource.None);
		if (this.target.trait is TraitGrave)
		{
			return;
		}
		if (text.Contains("$") || text.Contains("#") || text.Contains("@") || text.Contains("-"))
		{
			return;
		}
		if (text == this.target.id || !EClass.sources.cards.map.ContainsKey(text))
		{
			return;
		}
		if ((int)num <= 0 || this.target.source.components.IsEmpty())
		{
			return;
		}
		if (this.target.isCopy)
		{
			text = "ash3";
		}
		CardBlueprint.Set(new CardBlueprint
		{
			fixedQuality = true
		});
		Thing thing = ThingGen.Create(text, 1, Mathf.Max(1, lv * 2 / 3));
		if (thing != null)
		{
			thing.SetNum((int)num);
			thing.ChangeMaterial(this.target.material);
			thing.decay = decay;
			if (thing.IsDecayed && thing.IsFood)
			{
				thing.elements.SetBase(73, -10, 0);
			}
			EClass._map.TrySmoothPick(this.pos.IsBlocked ? this.owner.pos : this.pos, thing, EClass.pc);
		}
	}

	[CompilerGenerated]
	internal static bool <TryGetAct>g__IsValidTarget|20_0(string[] raw, ref TaskHarvest.<>c__DisplayClass20_0 A_1)
	{
		if (raw[0] == "digging")
		{
			return A_1.hasDiggingTool;
		}
		bool flag = A_1.p.cell.CanHarvest();
		int num = flag ? 250 : EClass.sources.elements.alias[raw[0]].id;
		bool flag2 = !flag && num != 250;
		return (flag2 || A_1.t == null || A_1.t.trait.CanHarvest) && (!flag2 | A_1.hasTool);
	}

	public bool wasReapSeed;

	public bool wasCrime;

	public BaseTaskHarvest.HarvestType mode = BaseTaskHarvest.HarvestType.Obj;
}
