using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Algorithms
{
	[Serializable]
	public class PathFinder : IPathfinder
	{
		[DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
		public unsafe static extern bool ZeroMemory(byte* destination, int length);

		public void Init(IPathfindGrid _grid, WeightCell[,] _weightMap, int size)
		{
			this.grid = _grid;
			this.weightMap = _weightMap;
			this.mGridX = (int)((ushort)size);
			this.mGridZ = (int)((ushort)size);
			this.mGridXZ = this.mGridX * this.mGridZ;
			if (this.mCalcGrid == null || this.mCalcGrid.Length != this.mGridXZ)
			{
				this.mCalcGrid = new PathFinder.PathFinderNodeFast[this.mGridXZ];
				this.mOpen = new PriorityQueueB<int>(new PathFinder.ComparePFNodeMatrix(this.mCalcGrid));
			}
		}

		public void FindPath(PathProgress path)
		{
			this.moveType = path.moveType;
			this._FindPath(path);
			if (path.nodes.Count > 0)
			{
				PathFinderNode pathFinderNode = path.nodes[path.nodes.Count - 1];
				if (pathFinderNode.X == path.startPoint.x && pathFinderNode.Z == path.startPoint.z)
				{
					path.nodes.RemoveAt(path.nodes.Count - 1);
				}
			}
			if (path.nodes.Count == 0)
			{
				path.state = PathProgress.State.Fail;
				return;
			}
			path.state = PathProgress.State.PathReady;
			path.nodeIndex = path.nodes.Count - 1;
		}

		private void _FindPath(PathProgress path)
		{
			lock (this)
			{
				Point startPoint = path.startPoint;
				Point destPoint = path.destPoint;
				path.nodes.Clear();
				this.mFound = (this.mStop = (this.mStopped = false));
				this.mCloseNodeCounter = 0;
				this.mOpenNodeValue += 2;
				this.mCloseNodeValue += 2;
				this.mOpen.Clear();
				this.mLocation = (this.mStartLocation = startPoint.z * this.mGridX + startPoint.x);
				this.mEndLocation = destPoint.z * this.mGridX + destPoint.x;
				if (this.mLocation >= this.mCalcGrid.Length)
				{
					if (this.debug)
					{
						Debug.Log("length over");
					}
				}
				else if (this.mEndLocation != this.mLocation)
				{
					this.mCalcGrid[this.mLocation].G = 0;
					this.mCalcGrid[this.mLocation].F = this.mHEstimate;
					this.mCalcGrid[this.mLocation].PX = (ushort)startPoint.x;
					this.mCalcGrid[this.mLocation].PZ = (ushort)startPoint.z;
					this.mCalcGrid[this.mLocation].Status = this.mOpenNodeValue;
					this.mOpen.Push(this.mLocation);
					while (this.mOpen.Count > 0 && !this.mStop)
					{
						this.mLocation = this.mOpen.Pop();
						if (this.mCalcGrid[this.mLocation].Status != this.mCloseNodeValue)
						{
							this.mLocationX = (ushort)(this.mLocation % this.mGridX);
							this.mLocationZ = (ushort)(this.mLocation % this.mGridXZ / this.mGridX);
							if (this.mLocation == this.mEndLocation)
							{
								this.mCalcGrid[this.mLocation].Status = this.mCloseNodeValue;
								this.mFound = true;
								break;
							}
							if (this.mCloseNodeCounter > path.searchLimit)
							{
								this.mStopped = true;
								if (path.searchLimit > 1000)
								{
									string str = "search limit: ";
									Point startPoint2 = path.startPoint;
									string str2 = (startPoint2 != null) ? startPoint2.ToString() : null;
									string str3 = " ";
									Point destPoint2 = path.destPoint;
									Debug.Log(str + str2 + str3 + ((destPoint2 != null) ? destPoint2.ToString() : null));
								}
								return;
							}
							if (this.PunishChangeDirection)
							{
								this.mHoriz = (int)(this.mLocationX - this.mCalcGrid[this.mLocation].PX);
							}
							for (int i = 0; i < (this.Diagonals ? 8 : 4); i++)
							{
								this.mNewLocationX = this.mLocationX + (ushort)this.mDirection[i, 0];
								this.mNewLocationZ = this.mLocationZ + (ushort)this.mDirection[i, 1];
								this.mNewLocation = (int)this.mNewLocationZ * this.mGridX + (int)this.mNewLocationX;
								if ((int)this.mNewLocationX < this.mGridX && (int)this.mNewLocationZ < this.mGridZ && (this.mCalcGrid[this.mNewLocation].Status != this.mCloseNodeValue || this.mReopenCloseNodes))
								{
									if (path.ignoreConnection && this.mEndLocation == this.mNewLocation)
									{
										this._weight = 1;
									}
									else
									{
										if (i < 4)
										{
											this.index = ((this.mNewLocationZ < this.mLocationZ) ? 0 : ((this.mNewLocationX > this.mLocationX) ? 1 : ((this.mNewLocationZ > this.mLocationZ) ? 2 : 3)));
											this._weight = this.weightMap[(int)this.mLocationX, (int)this.mLocationZ].weights[this.index];
										}
										else
										{
											this.mx = (int)(this.mLocationX + (ushort)this.mDirection[i, 0]);
											this.mz = (int)this.mLocationZ;
											this.index = ((this.mz < (int)this.mLocationZ) ? 0 : ((this.mx > (int)this.mLocationX) ? 1 : ((this.mz > (int)this.mLocationZ) ? 2 : 3)));
											this._weight = this.weightMap[(int)this.mLocationX, (int)this.mLocationZ].weights[this.index];
											if (this.weightMap[this.mx, this.mz].IsPathBlocked(this.moveType))
											{
												this._weight = 0;
											}
											if (this._weight > 0)
											{
												this.index = ((this.mz < (int)this.mNewLocationZ) ? 0 : ((this.mx > (int)this.mNewLocationX) ? 1 : ((this.mz > (int)this.mNewLocationZ) ? 2 : 3)));
												this._weight = this.weightMap[(int)this.mNewLocationX, (int)this.mNewLocationZ].weights[this.index];
												if (this._weight > 0)
												{
													this.mx = (int)this.mLocationX;
													this.mz = (int)(this.mLocationZ + (ushort)this.mDirection[i, 1]);
													this.index = ((this.mz < (int)this.mLocationZ) ? 0 : ((this.mx > (int)this.mLocationX) ? 1 : ((this.mz > (int)this.mLocationZ) ? 2 : 3)));
													this._weight = this.weightMap[(int)this.mLocationX, (int)this.mLocationZ].weights[this.index];
													if (this.weightMap[this.mx, this.mz].IsPathBlocked(this.moveType))
													{
														this._weight = 0;
													}
													if (this._weight > 0)
													{
														this.index = ((this.mz < (int)this.mNewLocationZ) ? 0 : ((this.mx > (int)this.mNewLocationX) ? 1 : ((this.mz > (int)this.mNewLocationZ) ? 2 : 3)));
														this._weight = this.weightMap[(int)this.mNewLocationX, (int)this.mNewLocationZ].weights[this.index];
													}
												}
											}
										}
										if (this._weight == 0)
										{
											goto IL_A70;
										}
										this._weight += this.weightMap[(int)this.mLocationX, (int)this.mLocationZ].baseWeight;
										if (this.mEndLocation != this.mNewLocation && this.weightMap[(int)this.mNewLocationX, (int)this.mNewLocationZ].IsPathBlocked(this.moveType))
										{
											goto IL_A70;
										}
									}
									this.mNewG = this.mCalcGrid[this.mLocation].G + (int)this._weight;
									if (this.PunishChangeDirection)
									{
										if (this.mNewLocationX - this.mLocationX != 0 && this.mHoriz == 0)
										{
											this.mNewG += Math.Abs((int)this.mNewLocationX - destPoint.x) + Math.Abs((int)this.mNewLocationZ - destPoint.z);
										}
										if (this.mNewLocationZ - this.mLocationZ != 0 && this.mHoriz != 0)
										{
											this.mNewG += Math.Abs((int)this.mNewLocationX - destPoint.x) + Math.Abs((int)this.mNewLocationZ - destPoint.z);
										}
									}
									if ((this.mCalcGrid[this.mNewLocation].Status != this.mOpenNodeValue && this.mCalcGrid[this.mNewLocation].Status != this.mCloseNodeValue) || this.mCalcGrid[this.mNewLocation].G > this.mNewG)
									{
										this.mCalcGrid[this.mNewLocation].PX = this.mLocationX;
										this.mCalcGrid[this.mNewLocation].PZ = this.mLocationZ;
										this.mCalcGrid[this.mNewLocation].G = this.mNewG;
										switch (this.mFormula)
										{
										default:
											this.mH = this.mHEstimate * (Math.Abs((int)this.mNewLocationX - destPoint.x) + Math.Abs((int)this.mNewLocationZ - destPoint.z));
											break;
										case HeuristicFormula.MaxDXDY:
											this.mH = this.mHEstimate * Math.Max(Math.Abs((int)this.mNewLocationX - destPoint.x), Math.Abs((int)this.mNewLocationZ - destPoint.z));
											break;
										case HeuristicFormula.DiagonalShortCut:
										{
											int num = Math.Min(Math.Abs((int)this.mNewLocationX - destPoint.x), Math.Abs((int)this.mNewLocationZ - destPoint.z));
											int num2 = Math.Abs((int)this.mNewLocationX - destPoint.x) + Math.Abs((int)this.mNewLocationZ - destPoint.z);
											this.mH = this.mHEstimate * 2 * num + this.mHEstimate * (num2 - 2 * num);
											break;
										}
										case HeuristicFormula.Euclidean:
											this.mH = (int)((double)this.mHEstimate * Math.Sqrt(Math.Pow((double)((int)this.mNewLocationZ - destPoint.x), 2.0) + Math.Pow((double)((int)this.mNewLocationZ - destPoint.z), 2.0)));
											break;
										case HeuristicFormula.EuclideanNoSQR:
											this.mH = (int)((double)this.mHEstimate * (Math.Pow((double)((int)this.mNewLocationX - destPoint.x), 2.0) + Math.Pow((double)((int)this.mNewLocationZ - destPoint.z), 2.0)));
											break;
										}
										if (this.TieBreaker)
										{
											int num3 = (int)this.mLocationX - destPoint.x;
											int num4 = (int)this.mLocationZ - destPoint.z;
											int num5 = startPoint.x - destPoint.x;
											int num6 = startPoint.z - destPoint.z;
											int num7 = Math.Abs(num3 * num6 - num5 * num4);
											this.mH = (int)((double)this.mH + (double)num7 * 0.001);
										}
										this.mCalcGrid[this.mNewLocation].F = this.mNewG + this.mH;
										this.mOpen.Push(this.mNewLocation);
										this.mCalcGrid[this.mNewLocation].Status = this.mOpenNodeValue;
									}
								}
								IL_A70:;
							}
							this.mCloseNodeCounter++;
							this.mCalcGrid[this.mLocation].Status = this.mCloseNodeValue;
						}
					}
					if (this.mFound)
					{
						int num8 = destPoint.x;
						int num9 = destPoint.z;
						PathFinder.PathFinderNodeFast pathFinderNodeFast = this.mCalcGrid[destPoint.z * this.mGridX + destPoint.x];
						PathFinderNode pathFinderNode;
						pathFinderNode.G = pathFinderNodeFast.G;
						pathFinderNode.PX = (int)pathFinderNodeFast.PX;
						pathFinderNode.PZ = (int)pathFinderNodeFast.PZ;
						pathFinderNode.X = destPoint.x;
						pathFinderNode.Z = destPoint.z;
						while (pathFinderNode.X != pathFinderNode.PX || pathFinderNode.Z != pathFinderNode.PZ)
						{
							path.nodes.Add(pathFinderNode);
							num8 = pathFinderNode.PX;
							num9 = pathFinderNode.PZ;
							pathFinderNodeFast = this.mCalcGrid[num9 * this.mGridX + num8];
							pathFinderNode.G = pathFinderNodeFast.G;
							pathFinderNode.PX = (int)pathFinderNodeFast.PX;
							pathFinderNode.PZ = (int)pathFinderNodeFast.PZ;
							pathFinderNode.X = num8;
							pathFinderNode.Z = num9;
						}
						path.nodes.Add(pathFinderNode);
						this.mStopped = true;
					}
					else
					{
						this.mStopped = true;
					}
				}
			}
		}

		public bool debug;

		public bool Diagonals;

		public bool PunishChangeDirection;

		public float HeavyDiagonals = 1f;

		public bool TieBreaker;

		public HeuristicFormula mFormula = HeuristicFormula.Manhattan;

		private IPathfindGrid grid;

		private int mHEstimate = 2;

		private PriorityQueueB<int> mOpen;

		private bool mStop;

		private bool mStopped = true;

		private int mHoriz;

		private bool mReopenCloseNodes = true;

		private PathFinder.PathFinderNodeFast[] mCalcGrid;

		private byte mOpenNodeValue = 1;

		private byte mCloseNodeValue = 2;

		private PathManager.MoveType moveType;

		[NonSerialized]
		public int total;

		private int mH;

		private int mLocation;

		private int mNewLocation;

		private ushort mLocationX;

		private ushort mLocationZ;

		private ushort mNewLocationX;

		private ushort mNewLocationZ;

		private int mGridXZ;

		private int mGridX;

		private int mGridZ;

		private int mCloseNodeCounter;

		private bool mFound;

		private sbyte[,] mDirection = new sbyte[,]
		{
			{
				0,
				-1
			},
			{
				1,
				0
			},
			{
				0,
				1
			},
			{
				-1,
				0
			},
			{
				-1,
				-1
			},
			{
				1,
				-1
			},
			{
				-1,
				1
			},
			{
				1,
				1
			}
		};

		private int mEndLocation;

		private int mStartLocation;

		private int mNewG;

		private byte _weight;

		public WeightCell[,] weightMap;

		private int index;

		private int mx;

		private int mz;

		internal struct PathFinderNodeFast
		{
			public int F;

			public int G;

			public ushort PX;

			public ushort PZ;

			public byte Status;
		}

		internal class ComparePFNodeMatrix : IComparer<int>
		{
			public ComparePFNodeMatrix(PathFinder.PathFinderNodeFast[] matrix)
			{
				this.mMatrix = matrix;
			}

			public int Compare(int a, int b)
			{
				if (this.mMatrix[a].F > this.mMatrix[b].F)
				{
					return 1;
				}
				if (this.mMatrix[a].F < this.mMatrix[b].F)
				{
					return -1;
				}
				return 0;
			}

			private PathFinder.PathFinderNodeFast[] mMatrix;
		}
	}
}
