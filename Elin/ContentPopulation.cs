using System;
using System.Collections.Generic;
using UnityEngine;

public class ContentPopulation : EContent
{
	public class Topic
	{
		public string header;

		public List<Chara> list = new List<Chara>();

		public int color;

		public int sortVal;
	}

	public static ContentPopulation Instance;

	public static int queryTarget;

	public static int queryType;

	public Color[] colors;

	public UIList list;

	public UISelectableGroup groupQueryTarget;

	public UISelectableGroup groupQueryType;

	public Dictionary<string, Topic> topics;

	public Func<Topic, UIList.SortMode, int> onSort;

	public override void OnSwitchContent(int idTab)
	{
		Instance = this;
		groupQueryTarget.Init(queryTarget, delegate(int a)
		{
			queryTarget = a;
			Refresh();
		});
		groupQueryType.Init(queryType, delegate(int a)
		{
			queryType = a;
			Refresh();
		});
		Refresh();
	}

	public void Refresh()
	{
		this.list.Clear();
		this.list.callbacks = new UIList.Callback<Topic, ItemPopulation>
		{
			onInstantiate = delegate(Topic a, ItemPopulation b)
			{
				b.SetTopic(a);
			},
			onSort = onSort
		};
		onSort = (Topic t, UIList.SortMode m) => -this.list.items.IndexOf(t) + t.sortVal * 100;
		topics = new Dictionary<string, Topic>();
		List<Chara> list = EClass._map.ListChara((queryTarget == 0) ? EClass.Home : EClass.game.factions.Wilds);
		switch (queryType)
		{
		case 0:
			foreach (Chara item in list)
			{
				if (item.happiness > 70)
				{
					Add("happy".lang(), item, 1, 10);
				}
				else if (item.happiness > 30)
				{
					Add("normal".lang(), item, 0, 5);
				}
				else
				{
					Add("unhappy".lang(), item, 2);
				}
			}
			break;
		case 1:
			foreach (Chara item2 in list)
			{
				Add("noJob".lang(), item2, 2);
			}
			break;
		case 2:
			foreach (Chara item3 in list)
			{
				Add("houseless".lang(), item3, 2);
			}
			break;
		case 3:
			foreach (Chara item4 in list)
			{
				Add(item4.faith.Name, item4);
			}
			onSort = (Topic t, UIList.SortMode m) => t.list.Count * 1000 - this.list.items.IndexOf(t);
			break;
		case 4:
			foreach (Chara item5 in list)
			{
				Add("querySingle".lang(), item5, 2);
			}
			break;
		default:
			foreach (Chara item6 in list)
			{
				Add("test", item6);
			}
			break;
		}
		this.list.Refresh();
		this.RebuildLayout(recursive: true);
	}

	public void Add(string idTopic, Chara c, int color = 1, int sortVal = 0)
	{
		Topic topic = topics.TryGetValue(idTopic);
		if (topic == null)
		{
			topic = new Topic
			{
				header = idTopic,
				color = color,
				sortVal = sortVal
			};
			topics.Add(idTopic, topic);
			list.Add(topic);
		}
		topic.list.Add(c);
	}
}
