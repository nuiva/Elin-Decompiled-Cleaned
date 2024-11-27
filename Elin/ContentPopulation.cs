using System;
using System.Collections.Generic;
using UnityEngine;

public class ContentPopulation : EContent
{
	public override void OnSwitchContent(int idTab)
	{
		ContentPopulation.Instance = this;
		this.groupQueryTarget.Init(ContentPopulation.queryTarget, delegate(int a)
		{
			ContentPopulation.queryTarget = a;
			this.Refresh();
		}, false);
		this.groupQueryType.Init(ContentPopulation.queryType, delegate(int a)
		{
			ContentPopulation.queryType = a;
			this.Refresh();
		}, false);
		this.Refresh();
	}

	public void Refresh()
	{
		this.list.Clear();
		BaseList baseList = this.list;
		UIList.Callback<ContentPopulation.Topic, ItemPopulation> callback = new UIList.Callback<ContentPopulation.Topic, ItemPopulation>();
		callback.onInstantiate = delegate(ContentPopulation.Topic a, ItemPopulation b)
		{
			b.SetTopic(a);
		};
		callback.onSort = this.onSort;
		baseList.callbacks = callback;
		this.onSort = ((ContentPopulation.Topic t, UIList.SortMode m) => -this.list.items.IndexOf(t) + t.sortVal * 100);
		this.topics = new Dictionary<string, ContentPopulation.Topic>();
		List<Chara> list = EClass._map.ListChara((ContentPopulation.queryTarget == 0) ? EClass.Home : EClass.game.factions.Wilds);
		switch (ContentPopulation.queryType)
		{
		case 0:
			using (List<Chara>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Chara chara = enumerator.Current;
					if (chara.happiness > 70)
					{
						this.Add("happy".lang(), chara, 1, 10);
					}
					else if (chara.happiness > 30)
					{
						this.Add("normal".lang(), chara, 0, 5);
					}
					else
					{
						this.Add("unhappy".lang(), chara, 2, 0);
					}
				}
				goto IL_287;
			}
			break;
		case 1:
			break;
		case 2:
			goto IL_177;
		case 3:
			goto IL_1B9;
		case 4:
			using (List<Chara>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Chara c = enumerator.Current;
					this.Add("querySingle".lang(), c, 2, 0);
				}
				goto IL_287;
			}
			goto IL_24D;
		default:
			goto IL_24D;
		}
		using (List<Chara>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Chara c2 = enumerator.Current;
				this.Add("noJob".lang(), c2, 2, 0);
			}
			goto IL_287;
		}
		IL_177:
		using (List<Chara>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Chara c3 = enumerator.Current;
				this.Add("houseless".lang(), c3, 2, 0);
			}
			goto IL_287;
		}
		IL_1B9:
		foreach (Chara chara2 in list)
		{
			this.Add(chara2.faith.Name, chara2, 1, 0);
		}
		this.onSort = ((ContentPopulation.Topic t, UIList.SortMode m) => t.list.Count * 1000 - this.list.items.IndexOf(t));
		goto IL_287;
		IL_24D:
		foreach (Chara c4 in list)
		{
			this.Add("test", c4, 1, 0);
		}
		IL_287:
		this.list.Refresh(false);
		this.RebuildLayout(true);
	}

	public void Add(string idTopic, Chara c, int color = 1, int sortVal = 0)
	{
		ContentPopulation.Topic topic = this.topics.TryGetValue(idTopic, null);
		if (topic == null)
		{
			topic = new ContentPopulation.Topic
			{
				header = idTopic,
				color = color,
				sortVal = sortVal
			};
			this.topics.Add(idTopic, topic);
			this.list.Add(topic);
		}
		topic.list.Add(c);
	}

	public static ContentPopulation Instance;

	public static int queryTarget;

	public static int queryType;

	public Color[] colors;

	public UIList list;

	public UISelectableGroup groupQueryTarget;

	public UISelectableGroup groupQueryType;

	public Dictionary<string, ContentPopulation.Topic> topics;

	public Func<ContentPopulation.Topic, UIList.SortMode, int> onSort;

	public class Topic
	{
		public string header;

		public List<Chara> list = new List<Chara>();

		public int color;

		public int sortVal;
	}
}
