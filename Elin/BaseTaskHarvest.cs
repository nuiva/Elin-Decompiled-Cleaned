using System;
using UnityEngine;

public class BaseTaskHarvest : TaskDesignation
{
	public bool IsTooHard
	{
		get
		{
			return this.isToolRequired && this.reqLv > this.toolLv;
		}
	}

	public bool IsHarvest
	{
		get
		{
			return this.pos.cell.CanHarvest();
		}
	}

	public virtual bool CanReapSeed
	{
		get
		{
			return this.pos.cell.CanReapSeed();
		}
	}

	public virtual BaseTaskHarvest.HarvestType harvestType
	{
		get
		{
			return BaseTaskHarvest.HarvestType.Block;
		}
	}

	public virtual bool IsGrowth
	{
		get
		{
			return false;
		}
	}

	public override bool CanPressRepeat
	{
		get
		{
			return true;
		}
	}

	public override int LeftHand
	{
		get
		{
			return -1;
		}
	}

	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.Cut;
		}
	}

	public virtual string GetBaseText(string str)
	{
		return base.GetText(str);
	}

	public override string GetTextSmall(Card c)
	{
		return this.pos.cell.GetObjName();
	}

	public override string GetText(string str = "")
	{
		if (this.owner == null)
		{
			this.owner = EClass.pc;
		}
		this.SetTarget(EClass.pc, null);
		string text = "tHarvest".lang(this.GetBaseText(str), (EClass.pc.Tool == null) ? "hand".lang() : this.owner.Tool.NameSimple, this.toolLv.ToString() ?? "", null, null);
		text = text + " (" + this.GetTextDifficulty() + ")";
		if (this.IsTooHard)
		{
			text = text.TagColor(Color.gray);
		}
		return text;
	}

	public static Thing GetBestTool(Chara c, Point p)
	{
		BaseTaskHarvest task = new BaseTaskHarvest
		{
			pos = p
		};
		task.SetOwner(c);
		Thing tool = null;
		int best = 0;
		c.things.Foreach(delegate(Thing t)
		{
			if (!(t.trait is TraitTool))
			{
				return;
			}
			task.SetTarget(c, t);
			if (task.efficiency > best)
			{
				best = task.efficiency;
				tool = t;
			}
		}, true);
		return tool;
	}

	public void SetTarget(Chara c, Thing tool = null)
	{
		if (c == null)
		{
			c = EClass.pc;
		}
		if (tool == null)
		{
			tool = c.Tool;
		}
		string[] array = null;
		SourceMaterial.Row row = null;
		switch (this.harvestType)
		{
		case BaseTaskHarvest.HarvestType.Floor:
			if (this.pos.cell.HasBridge)
			{
				array = this.pos.sourceBridge.reqHarvest;
				row = this.pos.matBridge;
			}
			else
			{
				array = this.pos.sourceFloor.reqHarvest;
				row = this.pos.matFloor;
			}
			break;
		case BaseTaskHarvest.HarvestType.Block:
			if (this.pos.HasBlock)
			{
				array = this.pos.sourceBlock.reqHarvest;
				row = this.pos.matBlock;
			}
			else
			{
				array = new string[this.pos.sourceFloor.reqHarvest.Length];
				Array.Copy(this.pos.sourceFloor.reqHarvest, array, this.pos.sourceFloor.reqHarvest.Length);
				array[0] = "mining";
				row = this.pos.matFloor;
			}
			break;
		case BaseTaskHarvest.HarvestType.Obj:
			array = this.pos.sourceObj.reqHarvest;
			row = this.pos.cell.matObj;
			break;
		case BaseTaskHarvest.HarvestType.Thing:
			array = this.target.trait.ReqHarvest.Split(',', StringSplitOptions.None);
			row = this.target.material;
			break;
		case BaseTaskHarvest.HarvestType.Disassemble:
			if (this.target == null)
			{
				return;
			}
			array = new string[]
			{
				"handicraft",
				"1"
			};
			row = this.target.material;
			break;
		}
		this.matHardness = row.hardness;
		if (this.harvestType == BaseTaskHarvest.HarvestType.Obj)
		{
			this.matHardness = this.matHardness * ((this.pos.growth != null) ? this.pos.growth.GetHp() : this.pos.sourceObj.hp) / 100;
		}
		if (row.tag.Contains("hard"))
		{
			this.matHardness *= 3;
		}
		BaseTaskHarvest.HarvestType harvestType = this.harvestType;
		if (harvestType <= BaseTaskHarvest.HarvestType.Block && row.id == 0)
		{
			this.matHardness *= 100;
		}
		this.idEle = (this.IsHarvest ? 250 : EClass.sources.elements.alias[array[0]].id);
		this.reqElement = Element.Create(this.idEle, 0);
		int num = array[1].ToInt();
		this.reqLv = this.matHardness + num;
		this.isToolRequired = (!this.IsHarvest && this.idEle != 250);
		this.toolLv = 0;
		if (tool != null)
		{
			int num2 = (this.idEle == 220 || this.idEle == 225) ? this.GetToolEfficiency(tool, row) : 100;
			this.toolLv += tool.material.hardness * num2 / 100;
			this.toolLv = (int)(this.toolLv + (tool.encLV + tool.blessedState));
		}
		this.toolLv += c.Evalue(this.idEle) * 3 / 2;
		if (this.toolLv < 0)
		{
			this.toolLv = 1;
		}
		this.efficiency = (this.toolLv + 5) * 100 / ((this.reqLv + 5) * 140 / 100);
		if (this.efficiency < 50)
		{
			this.efficiency = 50;
		}
		if (this.IsTooHard)
		{
			this.difficulty = 3;
		}
		else if (this.efficiency > 150)
		{
			this.difficulty = 0;
		}
		else if (this.efficiency > 100)
		{
			this.difficulty = 1;
		}
		else
		{
			this.difficulty = 2;
		}
		PlantData plantData = EClass._map.TryGetPlant(this.pos.cell);
		if (this.harvestType == BaseTaskHarvest.HarvestType.Obj && this.IsHarvest && plantData != null && plantData.size > 0)
		{
			this.maxProgress = (int)Mathf.Clamp(Rand.Range(8f, 10f) * (float)(100 + plantData.size * 100 + plantData.size * plantData.size * 30) / (float)this.efficiency, 2f, 1000f);
			return;
		}
		this.maxProgress = (int)Mathf.Clamp(Rand.Range(8f, 16f) * 100f / (float)this.efficiency, 2f, 30f);
	}

	public static int GetReqEle(string _raw)
	{
		string[] array = _raw.Split(',', StringSplitOptions.None);
		return EClass.sources.elements.alias[array[0]].id;
	}

	public int GetToolEfficiency(Thing t, SourceMaterial.Row mat)
	{
		int[] toolEfficiency = this.GetToolEfficiency(mat);
		new int[2];
		if (!t.HasElement(220, 1))
		{
			toolEfficiency[0] = 0;
		}
		if (!t.HasElement(225, 1))
		{
			toolEfficiency[1] = 0;
		}
		if (t.trait is TraitToolHammer)
		{
			toolEfficiency[0] = 50 + this.owner.Evalue(261);
		}
		int num = 20;
		foreach (int num2 in toolEfficiency)
		{
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}

	public int[] GetToolEfficiency(SourceMaterial.Row mat)
	{
		string category = mat.category;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(category);
		if (num <= 1237752336U)
		{
			if (num <= 862676408U)
			{
				if (num != 174734082U)
				{
					if (num != 270655681U)
					{
						if (num != 862676408U)
						{
							goto IL_1EB;
						}
						if (!(category == "skin"))
						{
							goto IL_1EB;
						}
						goto IL_1B8;
					}
					else
					{
						if (!(category == "fiber"))
						{
							goto IL_1EB;
						}
						goto IL_1C9;
					}
				}
				else if (!(category == "soil"))
				{
					goto IL_1EB;
				}
			}
			else if (num != 974867124U)
			{
				if (num != 1167392201U)
				{
					if (num != 1237752336U)
					{
						goto IL_1EB;
					}
					if (!(category == "water"))
					{
						goto IL_1EB;
					}
				}
				else
				{
					if (!(category == "crystal"))
					{
						goto IL_1EB;
					}
					goto IL_1A7;
				}
			}
			else
			{
				if (!(category == "rock"))
				{
					goto IL_1EB;
				}
				goto IL_1A7;
			}
			return new int[]
			{
				100,
				100
			};
		}
		if (num <= 2585652531U)
		{
			if (num != 1527558748U)
			{
				if (num != 2226448744U)
				{
					if (num != 2585652531U)
					{
						goto IL_1EB;
					}
					if (!(category == "ore"))
					{
						goto IL_1EB;
					}
				}
				else
				{
					if (!(category == "wood"))
					{
						goto IL_1EB;
					}
					goto IL_1C9;
				}
			}
			else if (!(category == "gem"))
			{
				goto IL_1EB;
			}
		}
		else if (num != 2993663101U)
		{
			if (num != 3683705231U)
			{
				if (num != 3693684870U)
				{
					goto IL_1EB;
				}
				if (!(category == "organic"))
				{
					goto IL_1EB;
				}
				goto IL_1B8;
			}
			else
			{
				if (!(category == "bone"))
				{
					goto IL_1EB;
				}
				return new int[]
				{
					100,
					50
				};
			}
		}
		else
		{
			if (!(category == "grass"))
			{
				goto IL_1EB;
			}
			goto IL_1C9;
		}
		IL_1A7:
		return new int[]
		{
			100,
			25
		};
		IL_1B8:
		return new int[]
		{
			50,
			100
		};
		IL_1C9:
		return new int[]
		{
			25,
			100
		};
		IL_1EB:
		return new int[]
		{
			100,
			100
		};
	}

	public string GetTextDifficulty()
	{
		return Lang.GetList("skillDiff")[this.difficulty];
	}

	public Thing target;

	public Element reqElement;

	public int matHardness;

	public int idEle;

	public int reqLv;

	public int toolLv;

	public int maxProgress;

	public int efficiency;

	public int difficulty;

	public int effectFrame = 1;

	public bool isToolRequired;

	public enum HarvestType
	{
		Floor,
		Block,
		Obj,
		Thing,
		Disassemble
	}
}
