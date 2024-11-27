using System;
using System.Collections.Generic;

public class QueueManager : EClass
{
	public UIQueue uiq
	{
		get
		{
			return UIQueue.Instance;
		}
	}

	public Queue currentQueue
	{
		get
		{
			if (this.list.Count <= 0)
			{
				return null;
			}
			return this.list[0];
		}
	}

	public void OnSetGoal(AIAct newGoal)
	{
	}

	public AIAct Pop()
	{
		return null;
	}

	public Queue ManualAdd(AIAct interaction)
	{
		return null;
	}

	public Queue Add(AIAct interaction, bool insert = false)
	{
		Queue queue = new Queue
		{
			interaction = interaction
		};
		if (insert)
		{
			this.list.Insert(0, queue);
		}
		else
		{
			this.list.Add(queue);
		}
		if (this.uiq)
		{
			this.uiq.OnAdd(queue, insert);
		}
		return queue;
	}

	public void Remove(Queue q)
	{
		q.removed = true;
		this.list.Remove(q);
		if (this.uiq)
		{
			this.uiq.OnRemove(q);
		}
	}

	public void Cancel(Queue q)
	{
		if (this.list[0] == q)
		{
			q.interaction.Cancel();
			return;
		}
		this.Remove(q);
	}

	public void SetOwner(Chara _owner)
	{
		this.list.Clear();
		this.owner = _owner;
		if (this.uiq)
		{
			this.uiq.OnSetOwner();
		}
	}

	public Chara owner;

	public List<Queue> list = new List<Queue>();
}
