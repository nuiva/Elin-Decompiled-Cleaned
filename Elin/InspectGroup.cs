using System;
using System.Collections.Generic;

public class InspectGroup : EClass
{
	public class Item
	{
		public string text;

		public string idSprite;

		public Action<IInspect> action;

		public int priority;

		public bool auto;

		public bool sound;

		public bool multi;

		public Func<string> textFunc;
	}

	public Type type;

	public List<Item> actions = new List<Item>();

	public List<IInspect> targets = new List<IInspect>();

	public IInspect FirstTarget => targets[0];

	public bool Solo => targets.Count == 1;

	public virtual string MultiName => type.ToString();

	public static InspectGroup Create(IInspect t)
	{
		InspectGroup inspectGroup = null;
		if (t is Area)
		{
			inspectGroup = new InspectGroupArea();
		}
		else if (t is Chara)
		{
			inspectGroup = new InspectGroupChara();
		}
		else if (t is Thing)
		{
			inspectGroup = new InspectGroupThing();
		}
		else if (t is TaskPoint)
		{
			inspectGroup = new InspectGroupTask();
		}
		else if (t is ObjInfo)
		{
			inspectGroup = new InspectGroupObj();
		}
		else if (t is BlockInfo)
		{
			inspectGroup = new InspectGroupBlock();
		}
		else if (t is EloPos)
		{
			inspectGroup = new InspectGroupEloPos();
		}
		inspectGroup.type = t.GetType();
		inspectGroup.targets.Add(t);
		return inspectGroup;
	}

	public bool CanInspect()
	{
		for (int num = targets.Count - 1; num >= 0; num--)
		{
			if (!targets[num].CanInspect)
			{
				targets.RemoveAt(num);
			}
		}
		if (targets.Count > 0)
		{
			return FirstTarget.CanInspect;
		}
		return false;
	}

	public virtual bool Contains(IInspect t)
	{
		return targets.Contains(t);
	}

	public string GetName()
	{
		if (!Solo)
		{
			return MultiName + " x " + targets.Count;
		}
		return FirstTarget.InspectName;
	}

	public virtual void SetActions()
	{
	}
}
public class InspectGroup<T> : InspectGroup where T : IInspect
{
	public new T FirstTarget => (T)base.FirstTarget;

	public sealed override void SetActions()
	{
		actions.Clear();
		OnSetActions();
	}

	public virtual void OnSetActions()
	{
	}

	public Item Add(string text, string idSprite, Action action, bool sound = false, int priority = 0, bool auto = false)
	{
		Item item = new Item
		{
			text = text,
			idSprite = idSprite,
			action = delegate
			{
				action();
			},
			sound = sound,
			priority = priority,
			auto = auto
		};
		actions.Add(item);
		return item;
	}

	public Item Add(string text, string idSprite, Action<T> action, bool sound = false, int priority = 0, bool auto = false)
	{
		Item item = new Item
		{
			text = text,
			idSprite = idSprite,
			action = delegate(IInspect a)
			{
				action((T)a);
			},
			sound = sound,
			priority = priority,
			auto = auto,
			multi = true
		};
		actions.Add(item);
		return item;
	}
}
