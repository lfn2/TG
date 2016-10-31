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

					if (distance == neighbourhoodDistance)
						break;

					if (trustMatrix.HasRow(user))
					{
						Dictionary<int, float> commonNeighbours = new Dictionary<int, float>();
						foreach (int trustedUser in trustMatrix[user])
						{
							if (!trustMatrix[sourceUser].Contains(trustedUser))
							{
								if (!(cnMatrix.HasRow(user) && cnMatrix[user].Contains(trustedUser)))
									cnMatrix[user, trustedUser] = CommonNeighbours(trustMatrix, user, trustedUser);

								commonNeighbours[trustedUser] = cnMatrix[user, trustedUser];
							}
						}

						List<KeyValuePair<int, float>> CNList = commonNeighbours.ToList();
						CNList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

						for (int i = 0; i < 5 && i < CNList.Count; i++)
						{
							queue.Enqueue(new BFSNode(CNList[i].Key, distance + 1));
							estimatedTrustMatrix[sourceUser, CNList[i].Key] = BasicTrustMetric(neighbourhoodDistance, distance + 1);
						}
					}
				}
			}

			return estimatedTrustMatrix;
		}

		public static Matrix<float> BuildRABasedTrust(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = trustMatrix.Clone();
			Matrix<float> raMatrix = new Matrix<float>();

			foreach (int sourceUser in trustMatrix.Rows)
			{
				BFSQueue queue = new BFSQueue(neighbourhoodDistance);

				foreach (int trustedUser in trustMatrix[sourceUser])
					queue.Enqueue(new BFSNode(trustedUser, 1));

				while (queue.Count() > 0)
				{
					BFSNode node = queue.Deque();
					int user = node.value;
					int distance = node.distance;

					if (distance == neighbourhoodDistance)
						break;

					if (trustMatrix.HasRow(user))
					{
						Dictionary<int, float> RAIndexes = new Dictionary<int, float>();
						foreach (int trustedUser in trustMatrix[user])
						{
							if (!trustMatrix[sourceUser].Contains(trustedUser))
							{
								if (!(raMatrix.HasRow(user) && raMatrix[user].Contains(trustedUser)))
									raMatrix[user, trustedUser] = ResourceAllocationIndex(trustMatrix, user, trustedUser);

								RAIndexes[trustedUser] = raMatrix[user, trustedUser];
							}							
						}

						List<KeyValuePair<int, float>> RAList = RAIndexes.ToList();
						RAList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

						for (int i = 0; i < 15 && i < RAList.Count; i++)
						{
							queue.Enqueue(new BFSNode(RAList[i].Key, distance + 1));
							estimatedTrustMatrix[sourceUser, RAList[i].Key] = BasicTrustMetric(neighbourhoodDistance, distance + 1);
						}
					}
				}

			}

			return estimatedTrustMatrix;
		}

		public static Matrix<float> BuildSaltonIndexTrustMatrix(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = new Matrix<float>();
			Matrix<float> saltonMatrix = new Matrix<float>();

			float maxSalton = 0;

			foreach (int sourceUser in trustMatrix.Rows)
			{
				BFSQueue queue = new BFSQueue(neighbourhoodDistance );
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
						saltonMatrix[sourceUser, user] = saltonMatrix[user, sourceUser] = JaccardIndex(trustMatrix, sourceUser, user);

						if (saltonMatrix[sourceUser, user] > 0.1f)
						{
							estimatedTrustMatrix[sourceUser, user] = BasicTrustMetric(neighbourhoodDistance, distance);

							if (trustMatrix.Rows.Contains(user))
								foreach (int trustedUser in trustMatrix[user])
									queue.Enqueue(new BFSNode(trustedUser, distance + 1));
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
