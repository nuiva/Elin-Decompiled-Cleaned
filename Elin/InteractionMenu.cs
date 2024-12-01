using UnityEngine;
using UnityEngine.UI;

public class InteractionMenu : EMono
{
	public LayoutGroup layout;

	public UIButton mold;

	public Vector3 offset;

	public Vector3 modPos;

	public SoundData soundPop;

	private void Awake()
	{
		mold = layout.CreateMold<UIButton>();
	}

	public void Show()
	{
		layout.RebuildLayout(recursive: true);
		soundPop.Play();
	}

	public UIButton Add()
	{
		return Util.Instantiate(mold, layout);
	}

	public void Clear()
	{
		layout.DestroyChildren();
	}
}
