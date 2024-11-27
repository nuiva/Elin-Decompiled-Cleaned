using System;
using UnityEngine.Events;

public class LayerDebug : ELayer
{
	public override void OnSwitchContent(Window window)
	{
		this.list.Clear();
		BaseList baseList = this.list;
		UIList.Callback<CoreDebug.DebugCommand, UIButton> callback = new UIList.Callback<CoreDebug.DebugCommand, UIButton>();
		callback.onClick = delegate(CoreDebug.DebugCommand a, UIButton b)
		{
		};
		callback.onInstantiate = delegate(CoreDebug.DebugCommand a, UIButton b)
		{
			b.mainText.text = a.name;
			b.onClick.AddListener(new UnityAction(a.action.Invoke));
		};
		baseList.callbacks = callback;
		foreach (CoreDebug.DebugCommand debugCommand in ELayer.debug.commands)
		{
			if (debugCommand.cat == window.idTab)
			{
				this.list.Add(debugCommand);
			}
		}
		this.list.Refresh(false);
	}

	public UIList list;
}
