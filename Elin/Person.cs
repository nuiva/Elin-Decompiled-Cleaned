using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class Person : EClass
{
	public SourcePerson.Row source
	{
		get
		{
			return EClass.sources.persons.map.TryGetValue(this.id.IsEmpty("narrator"), null);
		}
	}

	public Chara chara
	{
		get
		{
			return this._tempChara ?? this.refChara.GetAndCache(this.uidChara);
		}
	}

	public bool hasChara
	{
		get
		{
			return this.chara != null;
		}
	}

	public bool HumanSpeak
	{
		get
		{
			return this.chara == null || this.chara.IsPC || this.chara.IsHumanSpeak;
		}
	}

	public string Name
	{
		get
		{
			string str = this.tempName;
			string str2 = this.name;
			Chara chara = this.chara;
			string defaultStr;
			if ((defaultStr = ((chara != null) ? chara.Name : null)) == null)
			{
				SourcePerson.Row source = this.source;
				defaultStr = (((source != null) ? source.GetText("name", true) : null) ?? "null");
			}
			return str.IsEmpty(str2.IsEmpty(defaultStr)).ToTitleCase(false);
		}
	}

	public string NameBraced
	{
		get
		{
			string str = this.tempName;
			string str2 = this.name;
			Chara chara = this.chara;
			string defaultStr;
			if ((defaultStr = ((chara != null) ? chara.NameBraced : null)) == null)
			{
				SourcePerson.Row source = this.source;
				defaultStr = (((source != null) ? source.GetText("name", true) : null) ?? "null");
			}
			return str.IsEmpty(str2.IsEmpty(defaultStr)).ToTitleCase(false);
		}
	}

	public string Aka
	{
		get
		{
			Chara chara = this.chara;
			string result;
			if ((result = ((chara != null) ? chara.Aka : null)) == null)
			{
				SourcePerson.Row source = this.source;
				result = (((source != null) ? source.GetText("aka", true) : null) ?? "");
			}
			return result;
		}
	}

	public Person()
	{
	}

	public Person(string _id, Chara c = null)
	{
		this.id = _id;
		this.SetChara(c);
	}

	public Person(Religion r)
	{
		this.id = r.id;
		this.name = r.Name;
		this.idPortrait = "UN_" + r.id;
		if (!Portrait.modPortraits.dict.ContainsKey(this.idPortrait))
		{
			this.idPortrait = "UN_eyth";
		}
	}

	public Person(Chara c)
	{
		this.SetChara(c);
	}

	public void SetChara(Chara c)
	{
		if (c == null)
		{
			return;
		}
		this._tempChara = c;
		this.uidChara = c.uid;
		this.name = this.chara.Name;
		this.idPortrait = this.chara.GetIdPortrait();
		this.gender = this.chara.bio.gender;
		this.tones = this.chara.c_idTone;
	}

	public string ApplyTone(string text)
	{
		Chara chara = this.chara;
		return ((chara != null) ? chara.ApplyTone(text, false) : null) ?? text;
	}

	public string GetDramaTitle()
	{
		if (!this.tempName.IsEmpty())
		{
			return this.tempName;
		}
		if (this.chara != null)
		{
			Biography bio = this.chara.bio;
			string text = (!EClass.debug.showTone || !Lang.setting.useTone || this.chara.c_idTalk.IsEmpty() || this.chara.c_idTone.IsEmpty()) ? "" : string.Concat(new string[]
			{
				"  (",
				this.chara.c_idTone.Split('|', StringSplitOptions.None)[0].TrimEnd('-'),
				"な",
				MOD.listTalk.GetTalk(this.chara.c_idTalk, "name", false, true),
				")"
			});
			string dramaText = this.chara.trait.GetDramaText();
			if (!dramaText.IsEmpty())
			{
				text = text + " " + dramaText;
			}
			return string.Concat(new string[]
			{
				this.chara.Name.ToTitleCase(false),
				" - ",
				this.chara.race.GetText("name", false).ToTitleCase(false),
				" ",
				Lang.Parse("age", bio.TextAge(this.chara), null, null, null, null),
				" ",
				Lang._gender(bio.gender),
				text
			});
		}
		if (this.Name.IsEmpty())
		{
			return this.Aka;
		}
		if (this.Aka.IsEmpty())
		{
			return "「" + this.Name + "」";
		}
		return this.Aka.ToTitleCase(false) + "「" + this.Name + "」";
	}

	public void SetImage(Image image)
	{
		Chara chara = this.chara;
		Sprite sprite;
		if ((sprite = ((chara != null) ? chara.GetSprite(0) : null)) == null)
		{
			sprite = (Resources.Load<Sprite>("Media/Drama/Actor/" + this.source.idActor) ?? SpriteSheet.Get(this.source.idActor));
		}
		Sprite sprite2 = sprite;
		if (sprite2)
		{
			image.sprite = sprite2;
			image.SetNativeSize();
		}
	}

	[JsonProperty]
	public string name;

	[JsonProperty]
	public string idPortrait;

	[JsonProperty]
	public string id;

	[JsonProperty]
	public string tones;

	[JsonProperty]
	public int uidChara;

	[JsonProperty]
	public int gender;

	[JsonProperty]
	public SerializableColor colorHair;

	public RefChara refChara = new RefChara();

	public Chara _tempChara;

	public string tempName;
}
