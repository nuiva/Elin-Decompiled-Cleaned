using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class CardInspector : EMono
{
	public static CardInspector Instance;

	public Card card;

	public List<EditorTag> tags = new List<EditorTag>();

	public string traitVals;

	public string Info
	{
		get
		{
			if (card != null)
			{
				return card.id + "(" + card.NameSimple + ")";
			}
			return "";
		}
	}

	public string idEditor
	{
		get
		{
			return card?.c_idEditor;
		}
		set
		{
			if (card != null)
			{
				card.c_idEditor = value;
			}
		}
	}

	public string trait
	{
		get
		{
			return card?.c_idTrait;
		}
		set
		{
			if (card != null)
			{
				card.c_idTrait = value;
			}
		}
	}

	public string BGM
	{
		get
		{
			if (card == null || card.refVal == 0)
			{
				return "0";
			}
			BGMData bGMData = EMono.core.refs.bgms.FirstOrDefault((BGMData a) => a.id == card.refVal);
			if (!(bGMData == null))
			{
				return bGMData.name;
			}
			return "0";
		}
		set
		{
			if (card != null)
			{
				card.refVal = value.Split(' ')[0].ToInt();
			}
		}
	}

	public bool IsTape => card?.trait is TraitTape;

	public string spawnList
	{
		get
		{
			if (card == null || card.c_idRefCard.IsEmpty() || !EMono.editorSources.spawnLists.map.ContainsKey(card.c_idRefCard))
			{
				return "-";
			}
			SourceSpawnList.Row row = EMono.editorSources.spawnLists.rows.First((SourceSpawnList.Row a) => a.id == card.c_idRefCard);
			if (row != null)
			{
				return row.id;
			}
			return "-";
		}
		set
		{
			if (card != null)
			{
				card.c_idRefCard = value;
			}
		}
	}

	private IEnumerable<string> _TapeList()
	{
		List<string> list = new List<string>();
		foreach (BGMData bgm in EMono.core.refs.bgms)
		{
			list.Add(bgm.name);
		}
		return list;
	}

	private IEnumerable<string> Traits()
	{
		IEnumerable<Type> obj = ((card == null || !card.isChara) ? GetInheritedClasses(typeof(Trait), typeof(TraitChara)) : GetInheritedClasses(typeof(TraitChara)));
		List<string> list = new List<string> { "" };
		foreach (Type item in obj)
		{
			list.Add(item.Name);
		}
		return list;
	}

	private IEnumerable<Type> GetInheritedClasses(Type MyType, Type exclude = null)
	{
		return from TheType in Assembly.GetAssembly(MyType).GetTypes()
			where TheType.IsClass && TheType.IsSubclassOf(MyType) && (exclude == null || !TheType.IsSubclassOf(exclude))
			select TheType;
	}

	protected IEnumerable<string> _SpawnList()
	{
		return EMono.editorSources.spawnLists.GetListString();
	}

	private void Awake()
	{
		Instance = this;
	}

	public void SetCard(Card c, bool select = true)
	{
		card = c;
		tags.Clear();
		if (c != null && !c.c_editorTags.IsEmpty())
		{
			string[] array = c.c_editorTags.Split(',');
			foreach (string text in array)
			{
				try
				{
					tags.Add(text.ToEnum<EditorTag>());
				}
				catch
				{
					Debug.Log("No Editor Tag Found:" + text);
				}
			}
		}
		traitVals = c.c_editorTraitVal;
	}

	private void OnValidate()
	{
		if (!Application.isPlaying || !EMono.core.IsGameStarted || card == null)
		{
			return;
		}
		string text = null;
		bool flag = true;
		if (tags != null)
		{
			foreach (EditorTag tag in tags)
			{
				text = text + (flag ? "" : ",") + tag;
				flag = false;
			}
		}
		card.c_editorTags = text;
		card.c_editorTraitVal = (traitVals.IsEmpty() ? null : traitVals);
	}
}
