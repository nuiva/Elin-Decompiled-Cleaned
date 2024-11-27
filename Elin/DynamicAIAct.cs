using System;
using System.Collections.Generic;

public class DynamicAIAct : AIAct
{
	public override bool CancelWhenDamaged
	{
		get
		{
			return false;
		}
	}

	public override int PerformDistance
	{
		get
		{
			return base.PerformDistance;
		}
	}

	public override CursorInfo CursorIcon
	{
		get
		{
			return this.cursor;
		}
	}

	public override bool CloseLayers
	{
		get
		{
			return this.closeLayers;
		}
	}

	public override string GetText(string str = "")
	{
		return Lang.Get(this.lang);
	}

	public DynamicAIAct(string _lang, Func<bool> _onPerform = null, bool _closeLayers = false)
	{
		this.lang = _lang;
		this.onPerform = _onPerform;
		this.closeLayers = _closeLayers;
	}

	public override bool Perform()
	{
		if (this.pos != null || this.wait > 1)
		{
			return base.Perform();
		}
		return this.onPerform != null && this.onPerform();
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.pos != null)
		{
			yield return base.DoGotoInteraction(this.pos, null);
			this.owner.LookAt(this.pos);
		}
		yield return base.DoWait(this.wait);
		if (this.onPerform != null)
		{
			this.onPerform();
		}
		yield break;
	}

	public string lang;

	public Func<bool> onPerform;

	public bool closeLayers;

	public Point pos;

	public CursorInfo cursor;

	public int wait;
}
