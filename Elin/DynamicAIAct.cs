using System;
using System.Collections.Generic;

public class DynamicAIAct : AIAct
{
	public string lang;

	public Func<bool> onPerform;

	public bool closeLayers;

	public Point pos;

	public CursorInfo cursor;

	public int wait;

	public override bool CancelWhenDamaged => false;

	public override int PerformDistance => base.PerformDistance;

	public override CursorInfo CursorIcon => cursor;

	public override bool CloseLayers => closeLayers;

	public override string GetText(string str = "")
	{
		return Lang.Get(lang);
	}

	public DynamicAIAct(string _lang, Func<bool> _onPerform = null, bool _closeLayers = false)
	{
		lang = _lang;
		onPerform = _onPerform;
		closeLayers = _closeLayers;
	}

	public override bool Perform()
	{
		if (pos != null || wait > 1)
		{
			return base.Perform();
		}
		if (onPerform != null)
		{
			return onPerform();
		}
		return false;
	}

	public override IEnumerable<Status> Run()
	{
		if (pos != null)
		{
			yield return DoGotoInteraction(pos);
			owner.LookAt(pos);
		}
		yield return DoWait(wait);
		if (onPerform != null)
		{
			onPerform();
		}
	}
}
