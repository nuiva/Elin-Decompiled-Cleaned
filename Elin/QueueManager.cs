using System.Collections.Generic;

public class QueueManager : EClass
{
	public Chara owner;

	public List<Queue> list = new List<Queue>();

	public UIQueue uiq => UIQueue.Instance;

	public Queue currentQueue
	{
		get
		{
			if (list.Count <= 0)
			{
				return null;
			}
			return list[0];
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
			list.Insert(0, queue);
		}
		else
		{
			list.Add(queue);
		}
		if ((bool)uiq)
		{
			uiq.OnAdd(queue, insert);
		}
		return queue;
	}

	public void Remove(Queue q)
	{
		q.removed = true;
		list.Remove(q);
		if ((bool)uiq)
		{
			uiq.OnRemove(q);
		}
	}

	public void Cancel(Queue q)
	{
		if (list[0] == q)
		{
			q.interaction.Cancel();
		}
		else
		{
			Remove(q);
		}
	}

	public void SetOwner(Chara _owner)
	{
		list.Clear();
		owner = _owner;
		if ((bool)uiq)
		{
			uiq.OnSetOwner();
		}
	}
}
