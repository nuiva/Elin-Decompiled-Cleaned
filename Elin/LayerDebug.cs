public class LayerDebug : ELayer
{
	public UIList list;

	public override void OnSwitchContent(Window window)
	{
		list.Clear();
		list.callbacks = new UIList.Callback<CoreDebug.DebugCommand, UIButton>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(CoreDebug.DebugCommand a, UIButton b)
			{
				b.mainText.text = a.name;
				b.onClick.AddListener(a.action.Invoke);
			}
		};
		foreach (CoreDebug.DebugCommand command in ELayer.debug.commands)
		{
			if (command.cat == window.idTab)
			{
				list.Add(command);
			}
		}
		list.Refresh();
	}
}
