using System;
using System.Collections.Generic;
using UnityEngine;

public class CardRow : RenderRow
{
	public Card model
	{
		get
		{
			Card result;
			if ((result = this._model) == null)
			{
				result = (this._model = (this.isChara ? CharaGen.Create(this.id, -1) : ThingGen.Create(this.id, -1, -1)));
			}
			return result;
		}
	}

	public override string idRenderData
	{
		get
		{
			return this._idRenderData.IsEmpty((this.isChara ? "Chara/" : "Thing/") + this.id);
		}
	}

	public override string idSprite
	{
		get
		{
			return this.id;
		}
	}

	public override string idString
	{
		get
		{
			return this.id;
		}
	}

	public override string pathSprite
	{
		get
		{
			return this.pathRenderData + (this.isChara ? "Chara/" : "Thing/");
		}
	}

	public override SourcePref GetPref()
	{
		if (this.origin != null && !this.pref.UsePref)
		{
			return this.origin.pref;
		}
		return this.pref;
	}

	public override void OnImportData(SourceData data)
	{
		base.OnImportData(data);
		if (this.size == null || this.size.Length == 0)
		{
			this.W = (this.H = 1);
			this.multisize = false;
			return;
		}
		this.W = this.size[0];
		this.H = this.size[1];
		this.multisize = true;
	}

	public virtual string GetName(int i)
	{
		return this.GetName() + " x " + i.ToString();
	}

	public virtual string GetName(SourceMaterial.Row mat, int sum)
	{
		return "_of2".lang(mat.GetName(), this.GetName(), null, null, null) + " (" + sum.ToString() + ")";
	}

	public override string GetEditorListName()
	{
		return this.GetField("id") + "-" + this.GetField("name_JP");
	}

	public override string GetName()
	{
		return this.GetName(null, false);
	}

	public string GetName(Card c, bool full = false)
	{
		string text = base.GetName();
		SourceElement.Row source = Element.Void.source;
		if (c != null && c.isChara)
		{
			source = c.Chara.MainElement.source;
		}
		if (source != Element.Void.source)
		{
			text = text.Replace("#ele4", source.GetAltname(2)).Replace("#ele3", source.GetAltname(1)).Replace("#ele2", source.GetAltname(0)).Replace("#ele", source.GetName().ToLower());
		}
		else
		{
			text = text.Replace("#ele4", "").Replace("#ele3", "").Replace("#ele2", "").Replace("#ele", "");
		}
		string text2 = base.GetText("aka", false);
		if (text == "*r")
		{
			return text2;
		}
		if (full && !text2.IsEmpty())
		{
			text = text2.ToTitleCase(true) + Lang.space + text.Bracket(2);
		}
		return text;
	}

	public string id;

	public string idExtra;

	public string tierGroup;

	public string lightData;

	public string _origin;

	public int idShadow;

	public int quality;

	public int[] elements;

	public int[] shadow;

	public int[] size;

	public int[] light;

	public string[] loot;

	public string[] filter;

	public string[] trait;

	public string[] idActor;

	public string[] vals;

	public string[] name2;

	public string[] name2_JP;

	public Dictionary<int, int> elementMap;

	[NonSerialized]
	public Sprite sprite;

	[NonSerialized]
	public CardRow origin;

	[NonSerialized]
	public bool isOrigin;

	[NonSerialized]
	public bool isChara;

	[NonSerialized]
	public Card _model;
}
