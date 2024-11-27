using System;
using System.Collections.Generic;

public class InspectGroup : EClass
{
	public IInspect FirstTarget
	{
		get
		{
			return this.targets[0];
		}
	}

	public bool Solo
	{
		get
		{
			return this.targets.Count == 1;
		}
	}

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
		for (int i = this.targets.Count - 1; i >= 0; i--)
		{
			if (!this.targets[i].CanInspect)
			{
				this.targets.RemoveAt(i);
			}
		}
		return this.targets.Count > 0 && this.FirstTarget.CanInspect;
	}

	public virtual bool Contains(IInspect t)
	{
		return this.targets.Contains(t);
	}

	public string GetName()
	{
		if (!this.Solo)
		{
			return this.MultiName + " x " + this.targets.Count.ToString();
		}
		return this.FirstTarget.InspectName;
	}

	public virtual string MultiName
	{
		get
		{
			return this.type.ToString();
		}
	}

	public virtual void SetActions()
	{
	}

	public Type type;

	public List<InspectGroup.Item> actions = new List<InspectGroup.Item>();

	public List<IInspect> targets = new List<IInspect>();

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
}
