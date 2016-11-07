using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.Code;

namespace TG.TrustMetric
{
	public class ResourceAllocation : TrustMatrixBuilder
	{
		private int? topN;

		public ResourceAllocation(Matrix<float> trustMatrix) : base(trustMatrix)
		{
		}

		public ResourceAllocation(Matrix<float> trustMatrix, int topN) : base(trustMatrix)
		{
			this.topN = topN;
		}

		protected override void VisitNode(BFSQueue queue, BFSNode node, int sourceUser, int neighbourhoodDistance)
		{
			Dictionary<int, float> indexes = new Dictionary<int, float>();

			foreach (int user in this.originalTrustMatrix[node.value])
				if (!this.estimatedTrustMatrix[sourceUser].Contains(user))
					indexes[user] = ResourceAllocationIndex(sourceUser, user);

			List<KeyValuePair<int, float>> indexesList = indexes.ToList();
			if (topN.HasValue)
			{
				indexesList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
				if (topN < indexesList.Count)
					indexesList.RemoveRange(topN.Value, indexesList.Count - topN.Value);
			}

			int distance = node.distance + 1;

			foreach (KeyValuePair<int, float> pair in indexesList)
			{
				int user = pair.Key;

				estimatedTrustMatrix[sourceUser, user] = BasicTrustMetric(neighbourhoodDistance, distance);

				queue.Enqueue(new BFSNode(pair.Key, distance));
			}
		}

		private float ResourceAllocationIndex(int x, int y)
		{
			float ra = 0;

			foreach (int neighbour in this.originalTrustMatrix[x])
			{
				if (this.originalTrustMatrix.HasRow(y) && this.originalTrustMatrix[y].Contains(neighbour) && this.originalTrustMatrix.HasRow(neighbour))
					ra += (float)1 / this.originalTrustMatrix[neighbour].Count();
			}

			return ra;
		}

	}
}
