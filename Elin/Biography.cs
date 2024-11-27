using System;
using Newtonsoft.Json;

public class Biography : EClass
{
	public string idLike
	{
		get
		{
			return this.strs[0];
		}
		set
		{
			this.strs[0] = value;
		}
	}

	public int gender
	{
		get
		{
			return this.ints[0];
		}
		set
		{
			this.ints[0] = value;
		}
	}

	public int weight
	{
		get
		{
			return this.ints[2];
		}
		set
		{
			this.ints[2] = value;
		}
	}

	public int height
	{
		get
		{
			return this.ints[3];
		}
		set
		{
			this.ints[3] = value;
		}
	}

	public int birthDay
	{
		get
		{
			return this.ints[4];
		}
		set
		{
			this.ints[4] = value;
		}
	}

	public int birthMonth
	{
		get
		{
			return this.ints[5];
		}
		set
		{
			this.ints[5] = value;
		}
	}

	public int birthYear
	{
		get
		{
			return this.ints[6];
		}
		set
		{
			this.ints[6] = value;
		}
	}

	public int idHome
	{
		get
		{
			return this.ints[7];
		}
		set
		{
			this.ints[7] = value;
		}
	}

	public int idDad
	{
		get
		{
			return this.ints[8];
		}
		set
		{
			this.ints[8] = value;
		}
	}

	public int idMom
	{
		get
		{
			return this.ints[9];
		}
		set
		{
			this.ints[9] = value;
		}
	}

	public int idLoc
	{
		get
		{
			return this.ints[10];
		}
		set
		{
			this.ints[10] = value;
		}
	}

	public int idAdvDad
	{
		get
		{
			return this.ints[11];
		}
		set
		{
			this.ints[11] = value;
		}
	}

	public int idAdvMom
	{
		get
		{
			return this.ints[12];
		}
		set
		{
			this.ints[12] = value;
		}
	}

	public int idHobby
	{
		get
		{
			return this.ints[13];
		}
		set
		{
			this.ints[13] = value;
		}
	}

	public int stability
	{
		get
		{
			return this.ints[14];
		}
		set
		{
			this.ints[14] = value;
		}
	}

	public int law
	{
		get
		{
			return this.ints[15];
		}
		set
		{
			this.ints[15] = value;
		}
	}

	public int affection
	{
		get
		{
			return this.ints[16];
		}
		set
		{
			this.ints[16] = value;
		}
	}

	public int dominance
	{
		get
		{
			return this.ints[17];
		}
		set
		{
			this.ints[17] = value;
		}
	}

	public int extroversion
	{
		get
		{
			return this.ints[18];
		}
		set
		{
			this.ints[18] = value;
		}
	}

	public int idInterest
	{
		get
		{
			return this.ints[19];
		}
		set
		{
			this.ints[19] = value;
		}
	}

	public string nameHome
	{
		get
		{
			return this.StrBio(this.idHome);
		}
	}

	public string nameLoc
	{
		get
		{
			return this.StrBio(this.idLoc);
		}
	}

	public string nameDad
	{
		get
		{
			return "textParent".lang(this.StrBio(this.idAdvDad), this.StrBio(this.idDad), null, null, null);
		}
	}

	public string nameMom
	{
		get
		{
			return "textParent".lang(this.StrBio(this.idAdvMom), this.StrBio(this.idMom), null, null, null);
		}
	}

	public string nameBirthplace
	{
		get
		{
			return "birthLoc2".lang(this.nameHome, this.nameLoc, null, null, null);
		}
	}

	public int age
	{
		get
		{
			return EClass.world.date.year - this.birthYear;
		}
		set
		{
			this.birthYear = EClass.world.date.year - value;
		}
	}

	public int ageOriginal
	{
		get
		{
			return EClass.game.Prologue.year - this.birthYear;
		}
	}

	public bool IsUnderAge
	{
		get
		{
			return this.age < 18;
		}
	}

	public string TextAge(Chara c)
	{
		string result;
		if (this.age < 1000)
		{
			if ((result = (c.IsUnique ? this.ageOriginal : this.age).ToString()) == null)
			{
				return "";
			}
		}
		else
		{
			result = "???";
		}
		return result;
	}

	public void Generate(Chara c)
	{
		string bio = c.source.bio;
		SourceRace.Row race = c.Chara.race;
		this.stability = Rand.rndNormal(100);
		this.law = Rand.rndNormal(100);
		this.affection = Rand.rndNormal(100);
		this.dominance = Rand.rndNormal(100);
		this.extroversion = Rand.rndNormal(100);
		this.idInterest = EClass.rnd(Enum.GetNames(typeof(Interest)).Length);
		if (Biography.idFemale.Contains(c.id) || c.race.id == "roran")
		{
			this.SetGender(1);
		}
		else
		{
			this.SetGender(Gender.GetRandom());
		}
		this.RerollBio(c, 0, false);
		c.SetRandomTone();
		c.SetRandomTalk();
		bool flag = c.IsHuman;
		if (!bio.IsEmpty())
		{
			string[] array = bio.Split('/', StringSplitOptions.None);
			this.SetGender((array[0] == "f") ? 1 : 2);
			if (array.Length > 1)
			{
				if (!c.source.HasTag(CTAG.randomPortrait))
				{
					flag = false;
				}
				this.age = int.Parse(array[1]);
				c.pccData = IO.LoadFile<PCCData>(CorePath.packageCore + "Data/PCC/" + c.id + ".txt", false, null);
			}
			if (array.Length > 2)
			{
				this.height = int.Parse(array[2]);
			}
			if (array.Length > 3)
			{
				this.weight = int.Parse(array[3]);
			}
			if (array.Length > 4)
			{
				c.c_idTone = array[4];
			}
			if (array.Length > 5 && !array[5].IsEmpty())
			{
				c.c_idTalk = array[5];
			}
		}
		if (c.trait is TraitGuildClerk)
		{
			flag = true;
		}
		if (flag && !c.HasTag(CTAG.noPortrait))
		{
			this.SetPortrait(c);
		}
		if (c.id == "prostitute" && this.age < 15)
		{
			this.age = 15;
		}
		SourceThing.Row row = EClass.sources.things.rows.RandomItem<SourceThing.Row>();
		this.idLike = row.id;
		this.idHobby = EClass.sources.elements.hobbies.RandomItem<SourceElement.Row>().id;
	}

	public void RerollBio(Chara c, int ageIndex = 0, bool keepParent = false)
	{
		this.GenerateBirthday(c, ageIndex);
		this.GenerateAppearance(c);
		if (!keepParent)
		{
			this.GenerateDad();
			this.GenerateMom();
			this.idHome = this.RndBio("home");
			this.idLoc = this.RndBio("loc");
		}
	}

	public void GenerateBirthday(Chara c, int ageIndex = 0)
	{
		SourceRace.Row race = c.race;
		int num = race.age[0];
		int num2 = race.age[1];
		if (ageIndex != 0)
		{
			int num3 = (num2 - num) / 4;
			this.age = Rand.Range(num + num3 * (ageIndex - 1), num + num3 * ageIndex);
		}
		else
		{
			this.age = Rand.Range(num, num2);
		}
		this.birthDay = EClass.rnd(30) + 1;
		this.birthMonth = EClass.rnd(12) + 1;
	}

	public void GenerateAppearance(Chara c)
	{
		SourceRace.Row race = c.race;
		this.height = race.height + EClass.rnd(race.height / 5 + 1) - EClass.rnd(race.height / 5 + 1);
		this.weight = this.height * this.height * (EClass.rnd(6) + 18) / 10000;
	}

	public void GenerateDad()
	{
		this.idDad = this.RndBio("parent");
		this.idAdvDad = this.RndBio("adv");
	}

	public void GenerateMom()
	{
		this.idMom = this.RndBio("parent");
		this.idAdvMom = this.RndBio("adv");
	}

	private int RndBio(string group)
	{
		return WordGen.GetID(group);
	}

	private string StrBio(int id)
	{
		string result;
		if (!EClass.sources.langWord.map.ContainsKey(id))
		{
			if ((result = id.ToString()) == null)
			{
				return "";
			}
		}
		else
		{
			result = EClass.sources.langWord.map[id].GetText("name", false).Split(',', StringSplitOptions.None)[0];
		}
		return result;
	}

	public void SetGender(int g)
	{
		this.gender = g;
		if (this.gender > 2)
		{
			this.gender = 0;
		}
	}

	public void SetPortrait(Chara c)
	{
		string id = c.id;
		if (id == "shojo")
		{
			c.c_idPortrait = Portrait.GetRandomPortrait("special_f-littlegirl");
			return;
		}
		if (!(id == "sister"))
		{
			c.c_idPortrait = Portrait.GetRandomPortrait(this.gender, c.GetIdPortraitCat());
			return;
		}
		c.c_idPortrait = Portrait.GetRandomPortrait("special_f-littlesister");
	}

	public string TextBio(Chara c)
	{
		return string.Concat(new string[]
		{
			c.race.GetText("name", false).ToTitleCase(true),
			" ",
			Lang.Parse("age", this.TextAge(c), null, null, null, null),
			" ",
			Lang._gender(this.gender)
		});
	}

	public string TextBio2(Chara c)
	{
		return Lang.Parse("heightWeight", this.height.ToString() ?? "", this.weight.ToString() ?? "", null, null, null) + " " + ((c.material.alias == "meat") ? "" : c.material.GetName().ToTitleCase(true));
	}

	public string TextBirthDate(Chara c, bool _age = false)
	{
		return Lang.Parse("birthText", (this.birthYear >= 0) ? (this.birthYear.ToString() ?? "") : "???", this.birthMonth.ToString() ?? "", this.birthDay.ToString() ?? "", null, null) + (_age ? (" (" + Lang.Parse("age", this.TextAge(c), null, null, null, null) + ")") : "");
	}

	public string TextAppearance()
	{
		return Lang.Parse("heightWeight", this.height.ToString() ?? "", this.weight.ToString() ?? "", null, null, null);
	}

	public static string[] idFemale = new string[]
	{
		"shojo",
		"sister",
		"sister_cat",
		"younglady",
		"sister_undead"
	};

	[JsonProperty]
	public int[] ints = new int[20];

	[JsonProperty]
	public string[] strs = new string[3];

	public int[] personalities = new int[5];
}
