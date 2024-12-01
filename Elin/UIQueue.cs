using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIQueue : EMono
{
	public static UIQueue Instance;

	public LayoutGroup layout;

	public UIButton mold;

	public QueueManager queues => EMono.player.queues;

	private void OnEnable()
	{
		Instance = this;
	}

	private void OnDisable()
	{
		Instance = null;
	}

	public void OnAdd(Queue q, bool insert = false)
	{
		UIButton uIButton = (q.button = Util.Instantiate(mold, layout));
		uIButton.onClick.AddListener(delegate
		{
			if (q.CanCancel)
			{
				queues.Cancel(q);
			}
		});
		if (insert)
		{
			uIButton.transform.SetAsFirstSibling();
		}
		uIButton.tooltip.onShowTooltip = delegate(UITooltip a)
		{
			string text = q.interaction.GetType().ToString() + "\n";
			text += q.interaction.status;
			a.textMain.SetText(text);
		};
		uIButton.transform.DOScale(0f, 0.2f).From();
	}

	public void OnRemove(Queue q)
	{
		if (!q.button.interactable)
		{
			return;
		}
		q.button.interactable = false;
		q.button.transform.DOScale(0f, 0.3f).OnComplete(delegate
		{
			if ((bool)q.button.gameObject)
			{
				Object.DestroyImmediate(q.button.gameObject);
			}
		});
	}

	public void OnSetOwner()
	{
		if (!mold)
		{
			mold = layout.CreateMold<UIButton>();
		}
		layout.DestroyChildren();
	}

	private void Update()
	{
		if (queues.list.Count > 0 && !queues.currentQueue.interaction.IsRunning)
		{
			OnRemove(queues.currentQueue);
		}
	}
}
