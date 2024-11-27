using System;

public class Hobby : EClass
{
	public AIWork GetAI(Chara c)
	{
		string str = "AIWork_" + this.source.ai.IsEmpty(this.source.alias);
		AIWork aiwork;
		if (Type.GetType(str + ", Elin") != null)
		{
			aiwork = ClassCache.Create<AIWork>(str, "Elin");
		}
		else
		{
			aiwork = new AIWork();
		}
		aiwork.owner = c;
		aiwork.sourceWork = this.source;
		return aiwork;
	}

	public int GetLv(Chara c)
	{
		if (!this.source.skill.IsEmpty())
		{
			return c.Evalue(this.source.skill);
		}
		return c.LV;
	}

	public int GetEfficiency(Chara c)
	{
		int num = 50;
		if (c.homeBranch == null || c.currentZone != c.homeBranch.owner)
		{
			return 0;
		}
		if (c.currentZone == EClass._zone)
		{
			if ((!this.source.destTrait.IsEmpty() || !this.source.workTag.IsEmpty()) && !this.GetAI(c).SetDestination())
			{
				return 0;
			}
			if (c.noMove && c.pos.FindThing<TraitGeneratorWheel>() != null)
			{
				return 0;
			}
			TraitBed traitBed = c.FindBed();
			if (traitBed != null)
			{
				num += 30 + traitBed.owner.GetTotalQuality(true) + traitBed.owner.Evalue(750);
			}
		}
		if (this.source.alias == "Breeding")
		{
			num = num * c.race.breeder / 100;
		}
		num += this.GetLv(c);
		return num * (100 + c.homeBranch.Evalue(3708) * 10) / 100;
	}

	public SourceHobby.Row source
	{
		get
		{
			return EClass.sources.hobbies.map[this.id];
		}
	}

	public string Name
	{
		get
		{
			return this.source.GetName();
		}
	}

	public int id;
}
