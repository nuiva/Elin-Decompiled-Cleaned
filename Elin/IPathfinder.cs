using Algorithms;

public interface IPathfinder
{
	void FindPath(PathProgress progress);

	void Init(IPathfindGrid _grid, WeightCell[,] _weightMap, int size);
}
