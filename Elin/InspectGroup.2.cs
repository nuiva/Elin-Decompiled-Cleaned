using System;

public class InspectGroup<T> : InspectGroup where T : IInspect
{
	public new T FirstTarget
	{
		get
		{
			return (T)((object)base.FirstTarget);
		}
	}

	public sealed override void SetActions()
	{
		this.actions.Clear();
		this.OnSetActions();
	}

	public virtual void OnSetActions()
	{
	}

	public InspectGroup.Item Add(string text, string idSprite, Action action, bool sound = false, int priority = 0, bool auto = false)
	{
		InspectGroup.Item item = new InspectGroup.Item
		{
			text = text,
			idSprite = idSprite,
			action = delegate(IInspect a)
			{
				action();
			},
			sound = sound,
			priority = priority,
			auto = auto
		};
		this.actions.Add(item);
		return item;
	}

	public InspectGroup.Item Add(string text, string idSprite, Action<T> action, bool sound = false, int priority = 0, bool auto = false)
	{
		InspectGroup.Item item = new InspectGroup.Item
		{
			text = text,
			idSprite = idSprite,
			action = delegate(IInspect a)
			{
				action((T)((object)a));
			},
			sound = sound,
			priority = priority,
			auto = auto,
			multi = true
		};
		this.actions.Add(item);
		return item;
	}
}
