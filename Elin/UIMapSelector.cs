using UnityEngine;
using UnityEngine.UI;

public class UIMapSelector : EMono
{
	public UINote note;

	public Image imageZone;

	public void WriteNote(Zone z)
	{
		ZoneProfile.Load(z.source.idProfile);
		note.Clear();
		note.AddHeader(z.Name);
		note.AddText(z.source.GetDetail()).Hyphenate();
		note.Space();
		note.AddTopic("climate".lang(), "climateTemp".lang());
		_ = z.source.cost;
		note.AddTopic("ecosystem".lang(), "ecosystemTemp".lang());
		note.Build();
		imageZone.sprite = Resources.Load<Sprite>("Media/Graphics/Image/Zone/" + z.source.image.IsEmpty("default"));
	}
}
