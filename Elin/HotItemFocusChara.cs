using Newtonsoft.Json;
using UnityEngine;

public class HotItemFocusChara : HotItem
{
	[JsonProperty]
	public int uid;

	public Chara chara => RefChara.Get(uid) ?? EClass.pc;

	public override Color SpriteColor => new Color(1f, 1f, 1f, 0.9f);

	public override Vector3 SpriteScale => new Vector3(0.8f, 0.8f, 1f);

	public override string Name => "focusTo".lang(chara?.NameSimple ?? "???");

	public override string pathSprite => "icon_focus";

	public override Sprite GetSprite()
	{
		if (chara != null)
		{
			return chara.GetSprite();
		}
		return base.GetSprite();
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		if (chara == null || EClass.AdvMode)
		{
			SE.Beep();
		}
		else
		{
			EClass.screen.Focus(chara);
		}
	}
}
