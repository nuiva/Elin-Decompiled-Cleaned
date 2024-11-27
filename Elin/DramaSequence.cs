using System;
using System.Collections.Generic;
using UnityEngine;

public class DramaSequence : EClass
{
	public DialogDrama dialog
	{
		get
		{
			return this.manager.dialog;
		}
	}

	public void Clear()
	{
		this.steps.Clear();
		this.actors.Clear();
		this.events.Clear();
		this.tempEvents.Clear();
	}

	public DramaActor GetActor(string id)
	{
		if (this.actors.ContainsKey(id))
		{
			return this.actors[id];
		}
		if (EClass.sources.persons.map.ContainsKey(id))
		{
			return this.AddActor(id, new Person(id, null));
		}
		if (this.actors.Count <= 0)
		{
			return this.GetActor("narrator");
		}
		return this.actors.FirstItem<string, DramaActor>();
	}

	public T GetEvent<T>(string idStep) where T : DramaEvent
	{
		foreach (DramaEvent dramaEvent in this.events)
		{
			if (dramaEvent.step == idStep)
			{
				return dramaEvent as T;
			}
		}
		return default(T);
	}

	public DramaActor AddActor(string id, Person person)
	{
		if (this.actors.ContainsKey(id))
		{
			return this.actors[id];
		}
		DramaActor dramaActor = Util.Instantiate<DramaActor>(this.manager.moldActor, this.manager.actorPos);
		dramaActor.Init(this, id, person);
		this.actors.Add(id, dramaActor);
		return dramaActor;
	}

	public void AddStep(string id)
	{
		this.steps.Add(id, this.events.Count);
		this.events.Add(new DramaEvent
		{
			sequence = this,
			step = id
		});
	}

	public DramaEvent AddEvent(DramaEvent e)
	{
		if (!e.step.IsEmpty())
		{
			this.steps.Add(e.step, this.events.Count);
		}
		e.sequence = this;
		this.events.Add(e);
		return e;
	}

	public void PlayNext()
	{
		this.Play(this.currentEventID + 1);
	}

	public void Play(string id)
	{
		if (id == "last")
		{
			id = this.lastlastStep;
		}
		if (!id.IsEmpty() && !this.steps.ContainsKey(id))
		{
			Debug.Log(id);
		}
		this.Play(string.IsNullOrEmpty(id) ? 0 : this.steps[id]);
	}

	public void Play(int eventID = 0)
	{
		if (this.isExited)
		{
			return;
		}
		if (eventID >= this.events.Count)
		{
			if (!this.isLoop)
			{
				this.Exit();
				return;
			}
			eventID = 0;
		}
		this.currentEventID = eventID;
		this.currentEvent = this.events[eventID];
		this.currentEvent.Reset();
		string str = eventID.ToString() + "/";
		foreach (KeyValuePair<string, int> keyValuePair in this.steps)
		{
			if (keyValuePair.Value == eventID)
			{
				str += keyValuePair.Key;
			}
			if (keyValuePair.Value == eventID && !keyValuePair.Key.StartsWith("flag"))
			{
				this.lastlastStep = this.lastStep;
				this.lastStep = keyValuePair.Key;
				break;
			}
		}
		this.OnUpdate();
	}

	public void Exit()
	{
		this.isExited = true;
		this.currentEvent = null;
		this.manager.SetActive(false);
	}

	public void OnUpdate()
	{
		if (this.tempEvents.Count > 0)
		{
			if (this.tempEvents[0].Play() && this.tempEvents.Count > 0)
			{
				this.tempEvents.RemoveAt(0);
			}
			return;
		}
		if (this.currentEvent == null)
		{
			return;
		}
		if (this.currentEvent is DramaEventGoto)
		{
			string a = this.currentEvent.idJump;
			if (a == "*")
			{
				if (this.setup.step.IsEmpty())
				{
					this.PlayNext();
					return;
				}
				a = this.setup.step;
			}
			this.Play(a);
			return;
		}
		if (this.currentEvent.Play())
		{
			this.PlayNext();
		}
	}

	public string id;

	public DramaManager manager;

	public Dictionary<string, int> steps = new Dictionary<string, int>();

	public Dictionary<string, DramaActor> actors = new Dictionary<string, DramaActor>();

	public List<DramaEvent> events = new List<DramaEvent>();

	public bool isLoop;

	public bool canCancel = true;

	public bool isExited;

	public bool fullPortrait;

	public DramaEventTalk firstTalk;

	public List<DramaEvent> tempEvents = new List<DramaEvent>();

	private DramaEvent currentEvent;

	public string message = "";

	public string skipJump;

	public string lastStep;

	public string lastlastStep;

	public DramaSetup setup;

	private int currentEventID;

	public enum Template
	{
		Default
	}
}
