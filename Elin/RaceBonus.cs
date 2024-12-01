using UnityEngine;

public class RaceBonus : Element
{
	public override bool ShowValue => false;

	public override Sprite GetIcon(string suffix = "")
	{
		return SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "ele_Feat");
	}

	public override void OnWriteNote(UINote n, ElementContainer owner)
	{
		AddText(60);
		AddText(61);
		AddText(79);
		AddText(65);
		AddText(64);
		AddText(55);
		AddText(56);
		AddText(57);
		void AddText(int e)
		{
			int num = owner.Card.Evalue(e);
			if (num != 0)
			{
				n.AddTopic("TopicLeft", EClass.sources.elements.map[e].GetName(), num.ToString() ?? "");
			}
		}
	}
}
