using System;
using System.Collections.Generic;
using System.Linq;
using TG.Code;

namespace TG
{
	public class EstimatedTrustMatrixBuilder
	{

		private static float AVG_RA = 0.23f;

		private static float _Threshold = 0.1f;

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
								if (!(cnMatrix.HasRow(sourceUser) && cnMatrix[sourceUser].Contains(trustedUser)))
									cnMatrix[sourceUser, trustedUser] = CommonNeighbours(trustMatrix, sourceUser, trustedUser);

								commonNeighbours[trustedUser] = cnMatrix[sourceUser, trustedUser];
							}
						}

						List<KeyValuePair<int, float>> CNList = commonNeighbours.ToList();
						CNList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

						for (int i = 0; i < 15 && i < CNList.Count; i++)
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
								if (!(raMatrix.HasRow(sourceUser) && raMatrix[sourceUser].Contains(trustedUser)))
									raMatrix[sourceUser, trustedUser] = ResourceAllocationIndex(trustMatrix, sourceUser, trustedUser);

								RAIndexes[trustedUser] = raMatrix[sourceUser, trustedUser];
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

		public static Matrix<float> BuildSaltonIndexTrustMatrix2(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = new Matrix<float>();
			Matrix<float> saltonMatrix = new Matrix<float>();

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
						saltonMatrix[sourceUser, user] = saltonMatrix[user, sourceUser] = SaltonIndex(trustMatrix, sourceUser, user);

						if (saltonMatrix[sourceUser, user] > _Threshold)
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

		public static Matrix<float> BuildSaltonIndexTrustMatrix(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = trustMatrix.Clone();
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

					if (distance == neighbourhoodDistance)
						break;

					if (trustMatrix.HasRow(user))
					{
						foreach (int trustedUser in trustMatrix[user])
						{
							if (!(saltonMatrix.HasRow(sourceUser) && saltonMatrix[sourceUser].Contains(trustedUser)))
								saltonMatrix[sourceUser, trustedUser] = saltonMatrix[trustedUser, sourceUser] = SaltonIndex(trustMatrix, sourceUser, trustedUser);

							float jaccardIndex = saltonMatrix[sourceUser, trustedUser];

							if (jaccardIndex > _Threshold)
							{
								estimatedTrustMatrix[sourceUser, trustedUser] = BasicTrustMetric(neighbourhoodDistance, distance + 1);
								queue.Enqueue(new BFSNode(trustedUser, distance + 1));
							}
						}
					}
				}			
			}

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

						if (jaccardMatrix[sourceUser, user] > _Threshold)
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

		public static Matrix<float> BuildJaccardIndexTrustMatrix2(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = trustMatrix.Clone();
			Matrix<float> jaccardMatrix = new Matrix<float>();

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
						foreach (int trustedUser in trustMatrix[user])
						{
							if (!(jaccardMatrix.HasRow(sourceUser) && jaccardMatrix[sourceUser].Contains(trustedUser)))
								jaccardMatrix[sourceUser, trustedUser] = jaccardMatrix[trustedUser, sourceUser] = JaccardIndex(trustMatrix, sourceUser, trustedUser);

							float jaccardIndex = jaccardMatrix[sourceUser, trustedUser];

							if (jaccardIndex > _Threshold)
							{
								estimatedTrustMatrix[sourceUser, trustedUser] = BasicTrustMetric(neighbourhoodDistance, distance + 1);
								queue.Enqueue(new BFSNode(trustedUser, distance + 1));
							}
						} 
					}
				}
			}

			return estimatedTrustMatrix;
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
