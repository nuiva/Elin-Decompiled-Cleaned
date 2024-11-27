using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CardInspector : EMono
{
	public string Info
	{
		get
		{
			if (this.card != null)
			{
				return this.card.id + "(" + this.card.NameSimple + ")";
			}
			return "";
		}
	}

	public string idEditor
	{
		get
		{
			Card card = this.card;
			if (card == null)
			{
				return null;
			}
			return card.c_idEditor;
		}
		set
		{
			if (this.card != null)
			{
				this.card.c_idEditor = value;
			}
		}
	}

	public string trait
	{
		get
		{
			Card card = this.card;
			if (card == null)
			{
				return null;
			}
			return card.c_idTrait;
		}
		set
		{
			if (this.card != null)
			{
				this.card.c_idTrait = value;
			}
		}
	}

	public string BGM
	{
		get
		{
			if (this.card == null || this.card.refVal == 0)
			{
				return "0";
			}
			BGMData bgmdata = EMono.core.refs.bgms.FirstOrDefault((BGMData a) => a.id == this.card.refVal);
			if (!(bgmdata == null))
			{
				return bgmdata.name;
			}
			return "0";
		}
		set
		{
			if (this.card != null)
			{
				this.card.refVal = value.Split(' ', StringSplitOptions.None)[0].ToInt();
			}
		}
	}

	private IEnumerable<string> _TapeList()
	{
		List<string> list = new List<string>();
		foreach (BGMData bgmdata in EMono.core.refs.bgms)
		{
			list.Add(bgmdata.name);
		}
		return list;
	}

	public bool IsTape
	{
		get
		{
			Card card = this.card;
			return ((card != null) ? card.trait : null) is TraitTape;
		}
	}

	private IEnumerable<string> Traits()
	{
		IEnumerable<Type> enumerable = (this.card == null || !this.card.isChara) ? this.GetInheritedClasses(typeof(Trait), typeof(TraitChara)) : this.GetInheritedClasses(typeof(TraitChara), null);
		List<string> list = new List<string>();
		list.Add("");
		foreach (Type type in enumerable)
		{
			list.Add(type.Name);
		}
		return list;
	}

	private IEnumerable<Type> GetInheritedClasses(Type MyType, Type exclude = null)
	{
		return from TheType in Assembly.GetAssembly(MyType).GetTypes()
		where TheType.IsClass && TheType.IsSubclassOf(MyType) && (exclude == null || !TheType.IsSubclassOf(exclude))
		select TheType;
	}

	public string spawnList
	{
		get
		{
			if (this.card == null || this.card.c_idRefCard.IsEmpty() || !EMono.editorSources.spawnLists.map.ContainsKey(this.card.c_idRefCard))
			{
				return "-";
			}
			SourceSpawnList.Row row = EMono.editorSources.spawnLists.rows.First((SourceSpawnList.Row a) => a.id == this.card.c_idRefCard);
			if (row != null)
			{
				return row.id;
			}
			return "-";
		}
		set
		{
			if (this.card != null)
			{
				this.card.c_idRefCard = value;
			}
		}
	}

	protected IEnumerable<string> _SpawnList()
	{
		return EMono.editorSources.spawnLists.GetListString();
	}

	private void Awake()
	{
		CardInspector.Instance = this;
	}

	public void SetCard(Card c, bool select = true)
	{
		this.card = c;
		this.tags.Clear();
		if (c != null && !c.c_editorTags.IsEmpty())
		{
			foreach (string text in c.c_editorTags.Split(',', StringSplitOptions.None))
			{
				try
				{
					this.tags.Add(text.ToEnum(true));
				}
				catch
				{
					Debug.Log("No Editor Tag Found:" + text);
				}
			}
		}
		this.traitVals = c.c_editorTraitVal;
	}

	private void OnValidate()
	{
		if (!Application.isPlaying || !EMono.core.IsGameStarted || this.card == null)
		{
			return;
		}
		string text = null;
		bool flag = true;
		if (this.tags != null)
		{
			foreach (EditorTag editorTag in this.tags)
			{
				text = text + (flag ? "" : ",") + editorTag.ToString();
				flag = false;
			}
		}
		this.card.c_editorTags = text;
		this.card.c_editorTraitVal = (this.traitVals.IsEmpty() ? null : this.traitVals);
	}

	public static CardInspector Instance;

	public Card card;

	public List<EditorTag> tags = new List<EditorTag>();

	public string traitVals;
}
