using Newtonsoft.Json;

public class HotItemFocusPos : HotAction
{
	[JsonProperty]
	public int x;

	[JsonProperty]
	public int y;

	[JsonProperty]
	public Zone zone;

	public override string Name => "focusTo".lang(text.IsEmpty() ? (x + "/" + y) : "", text.IsEmpty((zone == null) ? "???" : zone.Name));

	public override string pathSprite => "icon_focus";

	public override bool CanChangeIconColor => true;

	public override void Perform()
	{
		if (zone != EClass.game.activeZone)
		{
			SE.Beep();
			return;
		}
		EClass.pc.SetAIImmediate(new AI_Goto(new Point(x, y), 0));
		ActionMode.Adv.SetTurbo(5);
	}
}
