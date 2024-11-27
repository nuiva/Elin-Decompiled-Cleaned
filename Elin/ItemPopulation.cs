using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopulation : EMono
{
	public void SetTopic(ContentPopulation.Topic t)
	{
		this.header.text1.text = t.header + "  ( " + t.list.Count.ToString() + " )";
		this.header.image1.color = ContentPopulation.Instance.colors[t.color];
		UIButton t2 = this.layout.CreateMold(null);
		using (List<Chara>.Enumerator enumerator = t.list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Chara c = enumerator.Current;
				UIButton uibutton = Util.Instantiate<UIButton>(t2, this.layout);
				uibutton.icon.sprite = c.GetSprite(0);
				uibutton.icon.SetNativeSize();
				uibutton.icon.transform.localScale = (c.IsPCC ? Vector3.one : new Vector3(0.7f, 0.7f, 1f));
				uibutton.icon.rectTransform.pivot = (c.IsPCC ? new Vector2(0.5f, 0f) : new Vector2(0.5f, 0.1f));
				uibutton.icon.rectTransform.anchoredPosition = Vector2.zero;
				uibutton.onClick.AddListener(delegate()
				{
					EMono.ui.AddLayerDontCloseOthers<LayerChara>().SetChara(c);
				});
				uibutton.tooltip.onShowTooltip = delegate(UITooltip a)
				{
					a.textMain.text = c.Name;
				};
			}
		}
	}

	public UIItem header;

	public LayoutGroup layout;
}
