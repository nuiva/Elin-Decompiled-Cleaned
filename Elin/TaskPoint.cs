using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class TaskPoint : Task, IInspect
{
	public override bool HasProgress
	{
		get
		{
			return false;
		}
	}

	public virtual int destDist
	{
		get
		{
			return 0;
		}
	}

	public virtual bool destIgnoreConnection
	{
		get
		{
			return false;
		}
	}

	public virtual bool isBlock
	{
		get
		{
			return false;
		}
	}

	public virtual bool Loop
	{
		get
		{
			return false;
		}
	}

	public override bool CanManualCancel()
	{
		return base.IsChildRunning && this.child is AI_Goto;
	}

	public override bool CanPerform()
	{
		if (this.owner == null)
		{
			this.owner = Act.CC;
		}
		TaskPoint.tempPos.Set(this.pos);
		this.pos.Set(Act.TP);
		HitResult hitResult = this.GetHitResult();
		this.pos.Set(TaskPoint.tempPos);
		return hitResult == HitResult.Valid || hitResult == HitResult.Warning;
	}

	public override bool _CanPerformTask(Chara chara, int radius)
	{
		return (radius == -1 || chara.pos.Distance(this.pos) <= radius) && !this.pos.cell.isSurrounded;
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.owner.IsPC && AM_Adv.actCount > 0)
		{
			while (EInput.rightMouse.pressing)
			{
				this.pos.Set(Act.TP.Set(ActionMode.Adv.mouseTarget.pos));
				if (this.CanPerform())
				{
					break;
				}
				yield return AIAct.Status.Running;
			}
			this.pos.Set(Act.TP.Set(ActionMode.Adv.mouseTarget.pos));
			if (!this.CanPerform() || !EInput.rightMouse.pressing || (this.HasProgress && !this.CanProgress()))
			{
				yield return base.Destroy();
			}
		}
		do
		{
			if (this.owner.pos.Distance(this.pos) > this.destDist)
			{
				yield return base.DoGoto(this.pos, this.destDist, this.destIgnoreConnection, null);
				if (this.owner.pos.Distance(this.pos) > this.destDist)
				{
					string[] array = new string[5];
					array[0] = AM_Adv.actCount.ToString();
					array[1] = "/";
					int num = 2;
					Point point = this.pos;
					array[num] = ((point != null) ? point.ToString() : null);
					array[3] = "/";
					int num2 = 4;
					Point point2 = this.owner.pos;
					array[num2] = ((point2 != null) ? point2.ToString() : null);
					Debug.Log(string.Concat(array));
					Debug.Log(this.destDist.ToString() + "/" + this.owner.pos.Distance(this.pos).ToString());
					yield return base.Destroy();
				}
			}
			if (!this.CanProgress())
			{
				yield return base.Destroy();
			}
			if (this.HasProgress)
			{
				yield return base.DoProgress();
			}
			else
			{
				yield return base.KeepRunning();
				this.OnProgressComplete();
			}
			if (this.owner != null && this.owner.IsPC)
			{
				AM_Adv.actCount++;
			}
		}
		while (this.Loop);
		yield break;
	}

	public void OnInspect()
	{
	}

	public bool CanInspect
	{
		get
		{
			return !this.isDestroyed;
		}
	}

	public string InspectName
	{
		get
		{
			return base.source.GetName();
		}
	}

	public Point InspectPoint
	{
		get
		{
			return this.pos;
		}
	}

	public unsafe Vector3 InspectPosition
	{
		get
		{
			return *this.pos.Position();
		}
	}

	public void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
		n.Clear();
		n.AddHeaderCard(this.Name, null);
		n.Build();
	}

	private static Point tempPos = new Point();

	[JsonProperty]
	public Point pos = new Point();

	public bool isRepeated;
}
