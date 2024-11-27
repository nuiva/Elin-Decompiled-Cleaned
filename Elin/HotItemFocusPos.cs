using System;
using Newtonsoft.Json;

public class HotItemFocusPos : HotAction
{
	public override string Name
	{
		get
		{
			return "focusTo".lang(this.text.IsEmpty() ? (this.x.ToString() + "/" + this.y.ToString()) : "", this.text.IsEmpty((this.zone == null) ? "???" : this.zone.Name), null, null, null);
		}
	}

	public override string pathSprite
	{
		get
		{
			return "icon_focus";
		}
	}

	public override bool CanChangeIconColor
	{
		get
		{
			return true;
		}
	}

	public override void Perform()
	{
		if (this.zone != EClass.game.activeZone)
		{
			SE.Beep();
			return;
		}
		EClass.pc.SetAIImmediate(new AI_Goto(new Point(this.x, this.y), 0, false, false));
		ActionMode.Adv.SetTurbo(5);
	}

	[JsonProperty]
	public int x;

	[JsonProperty]
	public int y;

	[JsonProperty]
	public Zone zone;
}
