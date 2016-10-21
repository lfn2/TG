using System;
using System.Collections.Generic;
using System.Linq;
using TG.Code;

namespace TG
{
	public class EstimatedTrustMatrixBuilder
	{

		public static Matrix<float> BuildProximityBasedTrust(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = trustMatrix.Clone();

			float maxRA = float.MinValue;

			foreach (int sourceUser in trustMatrix.Rows)
			{
				BFSQueue queue = new BFSQueue(neighbourhoodDistance - 1);

				foreach (int trustedUser in trustMatrix[sourceUser])
					queue.Enqueue(new BFSNode(trustedUser, 1));

				while (queue.Count() != 0)
				{
					BFSNode node = queue.Deque();
					int user = node.value;
					int distance = node.distance;

					if (trustMatrix.HasRow(user))
					{
						Dictionary<int, float> neighboursRA = new Dictionary<int, float>();
						foreach (int neighbour in trustMatrix[user])
						{
							if (!trustMatrix[sourceUser].Contains(neighbour))
								neighboursRA[neighbour] = ResourceAllocationIndex(trustMatrix, sourceUser, neighbour);
						}
						List<KeyValuePair<int, float>> RAList = neighboursRA.ToList();
						RAList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

						for (int i = 0; i < RAList.Count/3; i++)
							estimatedTrustMatrix[sourceUser, RAList[i].Key] = BasicTrustMetric(neighbourhoodDistance, distance + 1);
					}

					//if (trustMatrix.HasRow(user)) {
					//	foreach (int neighbour in trustMatrix[user])
					//	{
					//		if (!trustMatrix[sourceUser].Contains(neighbour))
					//		{
					//			float resourceAllocation = ResourceAllocationIndex(trustMatrix, sourceUser, neighbour);
					//			maxRA = Math.Max(maxRA, resourceAllocation);
					//			if (resourceAllocation > 1)
					//			{
					//				estimatedTrustMatrix[sourceUser, neighbour] = resourceAllocation * BasicTrustMetric(neighbourhoodDistance, distance);
					//				queue.Enqueue(new BFSNode(neighbour, distance + 1));
					//			}
					//		}
					//	}
					//}
				}
			}

			return estimatedTrustMatrix;
		}

		public static Matrix<float> BuildEstimatedTrustMatrix(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = new Matrix<float>();

			foreach (int sourceUser in trustMatrix.Rows)
			{
				BFSQueue queue = new BFSQueue(neighbourhoodDistance);

				foreach (int trustedUser in trustMatrix[sourceUser])
					queue.Enqueue(new BFSNode(trustedUser, 1));

				while (queue.Count() != 0)
				{
					BFSNode node = queue.Deque();
					int user = node.value;
					int distance = node.distance;

					if (!estimatedTrustMatrix[sourceUser].Contains(user))
					{
						estimatedTrustMatrix[sourceUser, user] = (float)(neighbourhoodDistance - distance + 1) / neighbourhoodDistance;

						if (trustMatrix.Rows.Contains(user))
							foreach (int trustedUser in trustMatrix[user])
								queue.Enqueue(new BFSNode(trustedUser, distance + 1));
					}
				}
			}

			return estimatedTrustMatrix;
		}

		public static Matrix<float> BuildEstimatedTrustMatrix2(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = new Matrix<float>();

			foreach (int sourceUser in trustMatrix.Rows)
			{
				HashSet<int> queueItems = new HashSet<int>();
				Queue<int> distanceQueue = new Queue<int>();
				Queue<int> userQueue = new Queue<int>();

				foreach (int trustedUser in trustMatrix[sourceUser])
					if (!queueItems.Contains(trustedUser))
					{
						userQueue.Enqueue(trustedUser);
						distanceQueue.Enqueue(1);
						queueItems.Add(trustedUser);
					}

				while (userQueue.Count != 0)
				{
					int user = userQueue.Dequeue();
					int distance = distanceQueue.Dequeue();


					if (!estimatedTrustMatrix[sourceUser].Contains(user))
					{
						estimatedTrustMatrix[sourceUser, user] = (float)(neighbourhoodDistance - distance + 1) / neighbourhoodDistance;

						if (trustMatrix.Rows.Contains(user))
							foreach (int trustedUser in trustMatrix[user])
								if (!queueItems.Contains(trustedUser) && distance < neighbourhoodDistance)
								{
									userQueue.Enqueue(trustedUser);
									distanceQueue.Enqueue(distance + 1);
									queueItems.Add(trustedUser);
								}
					}					
				}
			}

			return estimatedTrustMatrix;
		}

		public static Matrix<float> BuildResourceAllocationMatrix(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedMatrix = trustMatrix.Clone();

			foreach (int sourceUser in trustMatrix.Rows)
			{
				BFSQueue queue = new BFSQueue(neighbourhoodDistance - 1);

				foreach (int trustedUser in trustMatrix[sourceUser])
					queue.Enqueue(new BFSNode(trustedUser, 1));

				while (queue.Count() != 0)
				{
					BFSNode node = queue.Deque();
					int user = node.value;
					int distance = node.distance;

					if (trustMatrix.HasRow(user))
					{
						foreach (int neighbour in trustMatrix[user])
						{
							if (!trustMatrix[sourceUser].Contains(neighbour))
							{
								float resourceAllocation = ResourceAllocationIndex(trustMatrix, sourceUser, neighbour);
								if (resourceAllocation > 0)
								{
									estimatedMatrix[sourceUser, neighbour] = resourceAllocation;
									queue.Enqueue(new BFSNode(neighbour, distance + 1));
								}
							}
						}
					}
					
				}

			}

			return estimatedMatrix;
		}

		public static float BasicTrustMetric(int neighbourhoodDistance, int distance)
		{
			return (float)(neighbourhoodDistance - distance + 1) / neighbourhoodDistance;
		}

		public static float ResourceAllocationIndex(Matrix<float> matrix, int x, int y)
		{
			float ra = 0;

			foreach(int neighbour in matrix[x])
			{
				if (matrix.HasRow(y) && matrix[y].Contains(neighbour) && matrix.HasRow(neighbour))
					ra += (float) 1 / matrix[neighbour].Count();
			}

			return ra;
		}


	}
}
