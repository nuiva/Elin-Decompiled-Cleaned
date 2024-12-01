public interface ISyncScreen
{
	long Sync { get; }

	void OnEnterScreen();

	void OnLeaveScreen();
}
