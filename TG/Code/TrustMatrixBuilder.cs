using System;
using System.Collections.Generic;
using System.Linq;
using TG.Code;

namespace TG
{
	public class EstimatedTrustMatrixBuilder
	{

		private static float AVG_RA = 0.23f;

		public static Matrix<float> BuildCommonNeighboursBasedTrust(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = trustMatrix.Clone();
			Matrix<float> cnMatrix = new Matrix<float>();

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
							{
								if (!(cnMatrix.HasRow(sourceUser) && cnMatrix[sourceUser].Contains(neighbour)))
									cnMatrix[sourceUser, neighbour] = cnMatrix[neighbour, sourceUser] = CommonNeighbours(trustMatrix, sourceUser, neighbour);

								neighboursRA[neighbour] = cnMatrix[sourceUser, neighbour];
							}
						}
						List<KeyValuePair<int, float>> RAList = neighboursRA.ToList();
						RAList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

						for (int i = 0; i < RAList.Count / 3; i++)
							estimatedTrustMatrix[sourceUser, RAList[i].Key] = BasicTrustMetric(neighbourhoodDistance, distance + 1);
					}
				}
			}

			return estimatedTrustMatrix;
		}

		public static Matrix<float> BuildRABasedTrust(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = trustMatrix.Clone();
			Matrix<float> raMatrix = new Matrix<float>();

			float totalRA = 0;
			float countRA = 0;

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
							{
								float ra;
								if (!(raMatrix.HasRow(sourceUser) && raMatrix[sourceUser].Contains(neighbour)))
									raMatrix[sourceUser, neighbour] = raMatrix[neighbour, sourceUser] = ResourceAllocationIndex(trustMatrix, sourceUser, neighbour);

								ra = raMatrix[sourceUser, neighbour];
																	
								if (ra > AVG_RA)
								{
									estimatedTrustMatrix[sourceUser, neighbour] = BasicTrustMetric(neighbourhoodDistance + 1, distance + 1);
								}
							}
						}
					}
				}
			}

			return estimatedTrustMatrix;
		}

		public static Matrix<float> BuildRABasedTrust2(Matrix<float> trustMatrix, int neighbourhoodDistance)
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

		public static Matrix<float> BuildSaltonIndexTrustMatrix2(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = new Matrix<float>();
			Matrix<float> saltonMatrix = new Matrix<float>();

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

					if (estimatedTrustMatrix.HasRow(sourceUser) && !estimatedTrustMatrix[sourceUser].Contains(user))
					{
						if (!(saltonMatrix.HasRow(sourceUser) && saltonMatrix[sourceUser].Contains(user)))
							saltonMatrix[sourceUser, user] = saltonMatrix[user, sourceUser] = SaltonIndex(trustMatrix, sourceUser, user);

						if (saltonMatrix[sourceUser, user] > 0.5)
							estimatedTrustMatrix[sourceUser, user] = (float)(neighbourhoodDistance - distance + 1) / neighbourhoodDistance;

						if (trustMatrix.Rows.Contains(user))
							foreach (int trustedUser in trustMatrix[user])
								queue.Enqueue(new BFSNode(trustedUser, distance + 1));
					}
				}
			}

			return estimatedTrustMatrix;
		}

		public static Matrix<float> BuildSaltonIndexTrustMatrix(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = trustMatrix.Clone();
			Matrix<float> saltonMatrix = new Matrix<float>();

			float maxSalton = 0;

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
								float saltonIndex;
								if (!(saltonMatrix.HasRow(sourceUser) && saltonMatrix[sourceUser].Contains(neighbour)))
									saltonMatrix[sourceUser, neighbour] = saltonMatrix[neighbour, sourceUser] = SaltonIndex(trustMatrix, sourceUser, neighbour);

								saltonIndex = saltonMatrix[sourceUser, neighbour];
								maxSalton = Math.Max(maxSalton, saltonIndex);

								if (saltonIndex > 0.5f)
									estimatedTrustMatrix[sourceUser, neighbour] = BasicTrustMetric(neighbourhoodDistance + 1, distance + 1);
							}
						}
					}
				}
			}

			Console.WriteLine($"Max Salton: {maxSalton}");

			return estimatedTrustMatrix;
		}

		public static Matrix<float> BuildJaccardIndexTrustMatrix(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = new Matrix<float>();
			Matrix<float> jaccardMatrix = new Matrix<float>();

			float maxJaccard = 0;
			float minJaccard = 1;

			foreach (int sourceUser in trustMatrix.Rows)
			{
				BFSQueue queue = new BFSQueue(neighbourhoodDistance);
				estimatedTrustMatrix.AddRow(sourceUser);

				foreach (int trustedUser in trustMatrix[sourceUser])
					queue.Enqueue(new BFSNode(trustedUser, 1));

				while (queue.Count() != 0)
				{
					BFSNode node = queue.Deque();
					int user = node.value;
					int distance = node.distance;

					if (!estimatedTrustMatrix[sourceUser].Contains(user))
					{
						jaccardMatrix[sourceUser, user] = jaccardMatrix[user, sourceUser] = JaccardIndex(trustMatrix, sourceUser, user);

						if (jaccardMatrix[sourceUser, user] > 0.5f)
						{
							estimatedTrustMatrix[sourceUser, user] = BasicTrustMetric(neighbourhoodDistance, distance);

							if (trustMatrix.Rows.Contains(user))
								foreach (int trustedUser in trustMatrix[user])
									queue.Enqueue(new BFSNode(trustedUser, distance + 1));
						}						
					}
				}
			}

			Console.WriteLine($"Max Jaccard: {maxJaccard}");
			Console.WriteLine($"Min Jaccard: {minJaccard}");

			return estimatedTrustMatrix;
		}

		public static Matrix<float> BuildEstimatedTrustMatrix(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = new Matrix<float>();

			foreach (int sourceUser in trustMatrix.Rows)
			{
				BFSQueue queue = new BFSQueue(neighbourhoodDistance);
				estimatedTrustMatrix.AddRow(sourceUser);

				foreach (int trustedUser in trustMatrix[sourceUser])
					queue.Enqueue(new BFSNode(trustedUser, 1));

				while (queue.Count() != 0)
				{
					BFSNode node = queue.Deque();
					int user = node.value;
					int distance = node.distance;

					if (!estimatedTrustMatrix[sourceUser].Contains(user))
					{
						estimatedTrustMatrix[sourceUser, user] = BasicTrustMetric(neighbourhoodDistance, distance);

						if (trustMatrix.Rows.Contains(user))
							foreach (int trustedUser in trustMatrix[user])
								queue.Enqueue(new BFSNode(trustedUser, distance + 1));
					}
					
				}

				if (estimatedTrustMatrix[sourceUser].Count() == 0)
					estimatedTrustMatrix.remove(sourceUser);
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
								}
							}
						}
					}
					
				}

			}

			return estimatedMatrix;
		}

		private static float BasicTrustMetric(int neighbourhoodDistance, int distance)
		{
			return (float)(neighbourhoodDistance - distance + 1) / neighbourhoodDistance;
		}

		private static float SaltonIndex(Matrix<float> matrix, int x, int y)
		{
			float sa = 0;

			if (matrix.HasRow(x) && matrix.HasRow(y))
			{
				float commonNeighbours = CommonNeighbours(matrix, x, y);
				float xDegree = matrix[x].Count();
				float yDegree = matrix[y].Count();

				sa = commonNeighbours / ((float)Math.Sqrt(xDegree * yDegree));
			}			

			return sa;			
		}

		private static float JaccardIndex(Matrix<float> matrix, int x, int y)
		{
			float jaccardIndex = 0;

			if (matrix.HasRow(x) || matrix.HasRow(y))
			{
				float commonNeighbours = CommonNeighbours(matrix, x, y);
				HashSet<int> union = new HashSet<int>();
				if (matrix.HasRow(x))
					foreach (int i in matrix[x])
						union.Add(i);

				if (matrix.HasRow(y))
					foreach (int i in matrix[y])
						union.Add(i);

				jaccardIndex = commonNeighbours / union.Count;
			}

			return jaccardIndex;
		}

		private static float ResourceAllocationIndex(Matrix<float> matrix, int x, int y)
		{
			float ra = 0;

			foreach(int neighbour in matrix[x])
			{
				if (matrix.HasRow(y) && matrix[y].Contains(neighbour) && matrix.HasRow(neighbour))
					ra += (float) 1 / matrix[neighbour].Count();
			}

			return ra;
		}

		private static float CommonNeighbours(Matrix<float> matrix, int x, int y)
		{
			float commonNeighbours = 0;

			foreach (int neighbour in matrix[x])
				if (matrix.HasRow(y) && matrix[y].Contains(neighbour))
					commonNeighbours += 1;

			return commonNeighbours;
		}

	}
}
