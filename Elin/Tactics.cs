using System;

public class Tactics : EClass
{
	public static ConfigAutoCombat at
	{
		get
		{
			return EClass.game.config.autoCombat;
		}
	}

	public int RandomFacotr
	{
		get
		{
			if (!this.owner.IsPC)
			{
				return 30;
			}
			return 10;
		}
	}

	public int DestDist
	{
		get
		{
			if (this.owner.IsPC)
			{
				return 1;
			}
			if (this.sourceChara.aiParam.Length < 1)
			{
				return this.source.dist;
			}
			return this.sourceChara.aiParam[0];
		}
	}

	public int ChanceMove
	{
		get
		{
			if (this.owner.IsPC)
			{
				return this.source.movePC;
			}
			if (this.sourceChara.aiParam.Length < 2)
			{
				return this.source.move;
			}
			return this.sourceChara.aiParam[1];
		}
	}

	public int ChanceSecondMove
	{
		get
		{
			if (this.owner.IsPC)
			{
				return 100;
			}
			if (this.sourceChara.aiParam.Length < 3)
			{
				return 100;
			}
			return this.sourceChara.aiParam[2];
		}
	}

	public int P_Party
	{
		get
		{
			return this.source.party;
		}
	}

	public int P_Melee
	{
		get
		{
			if (!this.owner.IsPC)
			{
				return 50;
			}
			return this.source.melee;
		}
	}

	public int P_Range
	{
		get
		{
			if (!this.owner.IsPC)
			{
				return 50;
			}
			return this.source.range;
		}
	}

	public int P_Heal
	{
		get
		{
			return this.source.heal;
		}
	}

	public int P_Spell
	{
		get
		{
			if (!this.owner.IsPC)
			{
				return 50;
			}
			return this.source.spell;
		}
	}

	public int P_Buff
	{
		get
		{
			return this.source.buff;
		}
	}

	public int P_Debuff
	{
		get
		{
			return this.source.debuff;
		}
	}

	public int P_Summon
	{
		get
		{
			return this.source.summon;
		}
	}

	public bool CastPartyBuff
	{
		get
		{
			if (!this.owner.IsPC)
			{
				return this.source.tag.Contains("pt");
			}
			return Tactics.at.bCastParty;
		}
	}

	public int AbilityChance
	{
		get
		{
			if (this.owner.IsPC)
			{
				return 100;
			}
			if (!this.owner.IsPCParty)
			{
				return 50;
			}
			return 80;
		}
	}

	public int RangedChance
	{
		get
		{
			if (this.owner.IsPC)
			{
				return 100;
			}
			if (!this.owner.IsPCParty)
			{
				return 50;
			}
			return 100;
		}
	}

	public Tactics(Chara c)
	{
		this.owner = c;
		if (c.c_genes != null)
		{
			foreach (DNA dna in c.c_genes.items)
			{
				if (dna.type == DNA.Type.Brain && !dna.id.IsEmpty())
				{
					SourceChara.Row row = EClass.sources.charas.map[dna.id];
					string tactics = row.tactics;
					SourceTactics.Row row2 = EClass.sources.tactics.map.TryGetValue(row.id, null);
					string defaultStr;
					if ((defaultStr = ((row2 != null) ? row2.id : null)) == null)
					{
						SourceTactics.Row row3 = EClass.sources.tactics.map.TryGetValue(row.job, null);
						defaultStr = (((row3 != null) ? row3.id : null) ?? "predator");
					}
					string key = tactics.IsEmpty(defaultStr);
					this.source = EClass.sources.tactics.map[key];
					this.sourceChara = row;
					break;
				}
			}
		}
		if (this.source == null)
		{
			string text;
			if (!c.IsPC)
			{
				string tactics2 = c.source.tactics;
				SourceTactics.Row row4 = EClass.sources.tactics.map.TryGetValue(c.id, null);
				string defaultStr2;
				if ((defaultStr2 = ((row4 != null) ? row4.id : null)) == null)
				{
					SourceTactics.Row row5 = EClass.sources.tactics.map.TryGetValue(c.job.id, null);
					defaultStr2 = (((row5 != null) ? row5.id : null) ?? "predator");
				}
				text = tactics2.IsEmpty(defaultStr2);
			}
			else
			{
				text = Tactics.at.idType;
			}
			string key2 = text;
			this.source = EClass.sources.tactics.map[key2];
			this.sourceChara = c.source;
		}
	}

	public SourceTactics.Row source;

	public SourceChara.Row sourceChara;

	public Chara owner;
}
