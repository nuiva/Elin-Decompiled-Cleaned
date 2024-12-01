public class PathThread
{
	public void Start(PathProgress progress)
	{
		PathManager.Instance.pathfinder.FindPath(progress);
	}
}
