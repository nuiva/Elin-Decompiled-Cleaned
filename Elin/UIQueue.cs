using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class UIQueue : EMono
{
	public QueueManager queues
	{
		get
		{
			return EMono.player.queues;
		}
	}

	private void OnEnable()
	{
		UIQueue.Instance = this;
	}

	private void OnDisable()
	{
		UIQueue.Instance = null;
	}

	public void OnAdd(Queue q, bool insert = false)
	{
		UIButton uibutton = q.button = Util.Instantiate<UIButton>(this.mold, this.layout);
		uibutton.onClick.AddListener(delegate()
		{
			if (q.CanCancel)
			{
				this.queues.Cancel(q);
			}
		});
		if (insert)
		{
			uibutton.transform.SetAsFirstSibling();
		}
		uibutton.tooltip.onShowTooltip = delegate(UITooltip a)
		{
			string text = q.interaction.GetType().ToString() + "\n";
			text += q.interaction.status.ToString();
			a.textMain.SetText(text);
		};
		uibutton.transform.DOScale(0f, 0.2f).From<TweenerCore<Vector3, Vector3, VectorOptions>>();
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
			if (q.button.gameObject)
			{
				UnityEngine.Object.DestroyImmediate(q.button.gameObject);
			}
		});
	}

	public void OnSetOwner()
	{
		if (!this.mold)
		{
			this.mold = this.layout.CreateMold(null);
		}
		this.layout.DestroyChildren(false, true);
	}

	private void Update()
	{
		if (this.queues.list.Count > 0 && !this.queues.currentQueue.interaction.IsRunning)
		{
			this.OnRemove(this.queues.currentQueue);
		}
	}

	public static UIQueue Instance;

	public LayoutGroup layout;

	public UIButton mold;
}
