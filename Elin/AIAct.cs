using System;
using System.Collections.Generic;
using UnityEngine;

public class AIAct : Act
{
	public virtual bool IsRunning
	{
		get
		{
			return this.status == AIAct.Status.Running && this.owner != null;
		}
	}

	public bool IsChildRunning
	{
		get
		{
			return this.child != null && this.child.status == AIAct.Status.Running && this.child.owner != null;
		}
	}

	public bool IsMoveAI
	{
		get
		{
			if (!this.IsChildRunning)
			{
				return this is AI_Goto;
			}
			return this.child.IsMoveAI;
		}
	}

	public virtual int MaxRestart
	{
		get
		{
			return 5;
		}
	}

	public new virtual string Name
	{
		get
		{
			return base.source.GetName();
		}
	}

	public override string ToString()
	{
		return base.GetType().Name;
	}

	protected virtual MultiSprite stateIcon
	{
		get
		{
			return null;
		}
	}

	public virtual Sprite actionIcon
	{
		get
		{
			return EClass.core.refs.orbitIcons.Default;
		}
	}

	public virtual bool IsNoGoal
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsAutoTurn
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsIdle
	{
		get
		{
			return false;
		}
	}

	public virtual bool PushChara
	{
		get
		{
			return true;
		}
	}

	public virtual int MaxProgress
	{
		get
		{
			return 20;
		}
	}

	public virtual bool ShowProgress
	{
		get
		{
			return true;
		}
	}

	public virtual bool UseTurbo
	{
		get
		{
			return true;
		}
	}

	public virtual int CurrentProgress
	{
		get
		{
			return 0;
		}
	}

	public virtual bool ShowCursor
	{
		get
		{
			return true;
		}
	}

	public virtual bool CancelWhenDamaged
	{
		get
		{
			return true;
		}
	}

	public virtual bool CancelWhenMoved
	{
		get
		{
			return false;
		}
	}

	public virtual bool InformCancel
	{
		get
		{
			return true;
		}
	}

	public virtual Thing RenderThing
	{
		get
		{
			return null;
		}
	}

	public AIAct Current
	{
		get
		{
			if (!this.IsChildRunning)
			{
				return this;
			}
			return this.child.Current;
		}
	}

	public override MultiSprite GetStateIcon()
	{
		return (this.child ?? this).stateIcon;
	}

	public override bool IsAct
	{
		get
		{
			return false;
		}
	}

	public override bool ShowPotential
	{
		get
		{
			return false;
		}
	}

	public override bool UsePotential
	{
		get
		{
			return false;
		}
	}

	public override bool ShowRelativeAttribute
	{
		get
		{
			return true;
		}
	}

	public virtual Point GetDestination()
	{
		if (!this.IsChildRunning)
		{
			return this.owner.pos;
		}
		return this.child.GetDestination();
	}

	public AIProgress GetProgress()
	{
		if (this.IsChildRunning)
		{
			return this.child.GetProgress();
		}
		return this as AIProgress;
	}

	public string GetCurrentActionText()
	{
		if (this.IsChildRunning && this.child.source.id != 0 && !this.child.source.name_JP.IsEmpty())
		{
			return this.child.GetCurrentActionText();
		}
		return base.source.GetName().IsEmpty("idle".lang());
	}

	public override bool IsToolValid()
	{
		return this.owner == EClass.pc && Act.TOOL != null && Act.TOOL.parent == EClass.pc;
	}

	public virtual void OnStart()
	{
	}

	public void SetOwner(Chara c)
	{
		this.owner = c;
		this.status = AIAct.Status.Running;
		this.OnSetOwner();
	}

	public virtual void OnSetOwner()
	{
	}

	public void SetChild(AIAct seq, Func<AIAct.Status> _onChildFail = null)
	{
		if (this.child != null)
		{
			this.child.Reset();
		}
		this.child = seq;
		this.child.parent = this;
		this.child.SetOwner(this.owner);
		this.onChildFail = (_onChildFail ?? new Func<AIAct.Status>(this.Cancel));
	}

	public void Start()
	{
		this.Enumerator = this.Run().GetEnumerator();
		this.OnStart();
		if (this.owner == null)
		{
			return;
		}
		if (this.owner.held != null)
		{
			if (this.DropHeldOnStart)
			{
				this.owner.DropHeld(null);
			}
			else if (this.PickHeldOnStart)
			{
				this.owner.PickHeld(false);
			}
		}
		if (this.RightHand != 0)
		{
			this.owner.SetTempHand(this.RightHand, this.LeftHand);
		}
		if (this.owner.IsPC && this.UseTurbo)
		{
			ActionMode.Adv.SetTurbo(-1);
		}
	}

	public override bool Perform()
	{
		Act.CC.SetAIImmediate(this);
		return false;
	}

	public AIAct.Status Restart()
	{
		this.restartCount += 1;
		if ((int)this.restartCount >= this.MaxRestart)
		{
			return this.Success(null);
		}
		byte b = this.restartCount;
		Chara chara = this.owner;
		this.Reset();
		this.SetOwner(chara);
		this.restartCount = b;
		return this.status = AIAct.Status.Running;
	}

	public AIAct.Status Success(Action action = null)
	{
		this.status = AIAct.Status.Success;
		this.OnSuccess();
		if (action != null)
		{
			action();
		}
		this.OnCancelOrSuccess();
		this.Reset();
		return this.status;
	}

	public virtual void OnSuccess()
	{
	}

	public bool TryCancel(Card c)
	{
		if (!this.IsRunning || this.Enumerator == null)
		{
			return false;
		}
		if (this.InformCancel)
		{
			if (this.owner.IsPC)
			{
				this.owner.Say("cancel_act_pc", this.owner, null, null);
			}
			else if (c != null)
			{
				this.owner.Say("cancel_act", this.owner, c, null, null);
			}
			else
			{
				this.owner.Say("cancel_act2", this.owner, null, null);
			}
		}
		this.Cancel();
		return true;
	}

	public virtual AIAct.Status Cancel()
	{
		this.status = AIAct.Status.Fail;
		if (this.owner != null && this.owner.held != null && !this.owner.IsPC)
		{
			this.owner.PickHeld(false);
		}
		this.OnCancel();
		this.OnCancelOrSuccess();
		this.Reset();
		return this.status;
	}

	public virtual void OnCancel()
	{
	}

	public virtual void OnCancelOrSuccess()
	{
	}

	public virtual bool CanManualCancel()
	{
		return this.IsChildRunning && this.child is AI_Goto;
	}

	public AIAct.Status KeepRunning()
	{
		return AIAct.Status.Running;
	}

	public void Reset()
	{
		if (this.owner == null)
		{
			return;
		}
		if (this.child != null)
		{
			this.child.Reset();
		}
		this.OnReset();
		if (this.Enumerator != null)
		{
			this.Enumerator.Dispose();
			this.Enumerator = null;
		}
		this.owner = null;
		this.child = null;
		this.restartCount = 0;
		this.onChildFail = null;
		this.isFail = null;
	}

	public virtual void OnReset()
	{
	}

	public AIAct.Status Tick()
	{
		if (this.owner == null || (this.isFail != null && this.isFail()))
		{
			return this.Cancel();
		}
		if (this.IsChildRunning)
		{
			switch (this.child.Tick())
			{
			case AIAct.Status.Running:
				return AIAct.Status.Running;
			case AIAct.Status.Fail:
				if (this.onChildFail != null)
				{
					return this.onChildFail();
				}
				return AIAct.Status.Fail;
			case AIAct.Status.Success:
				if (this.owner == null || (this.isFail != null && this.isFail()))
				{
					return this.Cancel();
				}
				break;
			}
		}
		if (this.Enumerator == null)
		{
			this.Start();
			if (this.status != AIAct.Status.Running)
			{
				return this.status;
			}
		}
		if (!this.Enumerator.MoveNext())
		{
			return this.Success(null);
		}
		return this.status;
	}

	public virtual IEnumerable<AIAct.Status> Run()
	{
		yield return this.Success(null);
		yield break;
	}

	public AIAct.Status TickChild()
	{
		if (this.child == null || this.onChildFail == null)
		{
			AIAct aiact = this.child;
			string str = (aiact != null) ? aiact.ToString() : null;
			string str2 = "/";
			Func<AIAct.Status> func = this.onChildFail;
			Debug.Log(str + str2 + ((func != null) ? func.ToString() : null));
			return AIAct.Status.Fail;
		}
		if (this.child.Tick() != AIAct.Status.Fail)
		{
			return AIAct.Status.Running;
		}
		if (this.onChildFail != null)
		{
			return this.onChildFail();
		}
		return AIAct.Status.Fail;
	}

	public AIAct.Status Do(AIAct _seq, Func<AIAct.Status> _onChildFail = null)
	{
		this.SetChild(_seq, _onChildFail);
		return this.TickChild();
	}

	public AIAct.Status DoGotoInteraction(Point pos, Func<AIAct.Status> _onChildFail = null)
	{
		if (pos != null && pos.IsValid)
		{
			this.SetChild(new AI_Goto(pos, 0, false, true), _onChildFail);
			return this.TickChild();
		}
		if (_onChildFail == null)
		{
			return this.Cancel();
		}
		return _onChildFail();
	}

	public AIAct.Status DoGoto(Point pos, int dist = 0, bool ignoreConnection = false, Func<AIAct.Status> _onChildFail = null)
	{
		if (pos != null && pos.IsValid)
		{
			this.SetChild(new AI_Goto(pos, dist, ignoreConnection, false), _onChildFail);
			return this.TickChild();
		}
		if (_onChildFail == null)
		{
			return this.Cancel();
		}
		return _onChildFail();
	}

	public AIAct.Status DoGoto(Card card, Func<AIAct.Status> _onChildFail = null)
	{
		return this.DoGoto(card, (card.isChara || card.pos.cell.blocked) ? 1 : 0, _onChildFail);
	}

	public AIAct.Status DoGoto(Card card, int dist, Func<AIAct.Status> _onChildFail = null)
	{
		if (card != null && card == this.owner.held)
		{
			return AIAct.Status.Running;
		}
		if (card != null && card.ExistsOnMap)
		{
			this.SetChild(new AI_Goto(card, dist, false, false), _onChildFail);
			return this.TickChild();
		}
		if (_onChildFail == null)
		{
			return this.Cancel();
		}
		return _onChildFail();
	}

	public AIAct.Status DoGotoSpot(Card card, Func<AIAct.Status> _onChildFail = null)
	{
		if (card != null && card == this.owner.held)
		{
			return AIAct.Status.Running;
		}
		if (card != null && card.ExistsOnMap)
		{
			Point randomPoint = card.trait.GetRandomPoint(null, null);
			int dist = randomPoint.cell.blocked ? 1 : 0;
			this.SetChild(new AI_Goto(randomPoint, dist, false, false), _onChildFail);
			return this.TickChild();
		}
		if (_onChildFail == null)
		{
			return this.Cancel();
		}
		return _onChildFail();
	}

	public AIAct.Status DoGoto<T>(Func<AIAct.Status> _onChildFail = null) where T : Trait
	{
		Trait random = EClass._map.Installed.traits.GetTraitSet<T>().GetRandom();
		return this.DoGoto((random != null) ? random.owner : null, _onChildFail);
	}

	public AIAct.Status DoGotoSpot<T>(Func<AIAct.Status> _onChildFail = null, bool ignoreAccessType = false) where T : Trait
	{
		Trait random = EClass._map.Installed.traits.GetTraitSet<T>().GetRandom(ignoreAccessType ? null : this.owner);
		if (random != null)
		{
			this.DoGoto(random.GetRandomPoint(null, ignoreAccessType ? null : this.owner), 0, false, _onChildFail);
		}
		if (_onChildFail == null)
		{
			return AIAct.Status.Fail;
		}
		return _onChildFail();
	}

	public AIAct.Status DoGrab(Card card, int num = -1, bool pickHeld = false, Func<AIAct.Status> _onChildFail = null)
	{
		if (card != null && card == this.owner.held)
		{
			return AIAct.Status.Running;
		}
		if (card == null || !card.GetRootCard().ExistsOnMap)
		{
			return this.Cancel();
		}
		this.SetChild(new AI_Grab
		{
			target = card,
			num = num,
			pickHeld = pickHeld
		}, _onChildFail);
		return this.TickChild();
	}

	public AIAct.Status DoGrab<T>() where T : Trait
	{
		this.SetChild(new AI_Grab<T>(), null);
		return this.TickChild();
	}

	public AIAct.Status DoProgress()
	{
		this.SetChild(this.CreateProgress(), null);
		return this.TickChild();
	}

	public AIAct.Status DoIdle(int repeat = 3)
	{
		this.SetChild(new AI_Idle
		{
			maxRepeat = repeat
		}, new Func<AIAct.Status>(this.KeepRunning));
		return this.TickChild();
	}

	public AIAct.Status DoWait(int count = 1)
	{
		this.SetChild(new AI_Wait
		{
			count = count
		}, new Func<AIAct.Status>(this.KeepRunning));
		return this.TickChild();
	}

	public virtual bool HasProgress
	{
		get
		{
			return true;
		}
	}

	public virtual AIProgress CreateProgress()
	{
		Progress_Custom progress_Custom = new Progress_Custom
		{
			onProgress = delegate(Progress_Custom p)
			{
				this.OnProgress();
			},
			onProgressComplete = new Action(this.OnProgressComplete),
			canProgress = new Func<bool>(this.CanProgress),
			onBeforeProgress = new Action(this.OnBeforeProgress)
		};
		this.OnCreateProgress(progress_Custom);
		return progress_Custom;
	}

	public virtual void OnCreateProgress(Progress_Custom p)
	{
	}

	public virtual bool CanProgress()
	{
		return true;
	}

	public virtual void OnBeforeProgress()
	{
	}

	public virtual void OnProgress()
	{
	}

	public virtual void OnProgressComplete()
	{
	}

	public void SimulateZone(int days)
	{
		if (this.owner.conSuspend != null)
		{
			return;
		}
		this.OnSimulateZone(days);
	}

	public virtual void OnSimulateZone(int days)
	{
	}

	public virtual void OnSimulatePosition()
	{
	}

	public new Chara owner;

	public AIAct.Status status;

	public IEnumerator<AIAct.Status> Enumerator;

	public AIAct child;

	public AIAct parent;

	public byte restartCount;

	public Func<AIAct.Status> onChildFail;

	public Func<bool> isFail;

	public enum Status
	{
		Running,
		Fail,
		Success
	}
}
