using UnityEngine;
using UnityEngine.UI;

public class ItemPopulation : EMono
{
	public UIItem header;

	public LayoutGroup layout;

	public void SetTopic(ContentPopulation.Topic t)
	{
		header.text1.text = t.header + "  ( " + t.list.Count + " )";
		header.image1.color = ContentPopulation.Instance.colors[t.color];
		UIButton t2 = layout.CreateMold<UIButton>();
		foreach (Chara c in t.list)
		{
			UIButton uIButton = Util.Instantiate(t2, layout);
			uIButton.icon.sprite = c.GetSprite();
			uIButton.icon.SetNativeSize();
			uIButton.icon.transform.localScale = (c.IsPCC ? Vector3.one : new Vector3(0.7f, 0.7f, 1f));
			uIButton.icon.rectTransform.pivot = (c.IsPCC ? new Vector2(0.5f, 0f) : new Vector2(0.5f, 0.1f));
			uIButton.icon.rectTransform.anchoredPosition = Vector2.zero;
			uIButton.onClick.AddListener(delegate
			{
				EMono.ui.AddLayerDontCloseOthers<LayerChara>().SetChara(c);
			});
			uIButton.tooltip.onShowTooltip = delegate(UITooltip a)
			{
				a.textMain.text = c.Name;
			};
		}
	}
}
