using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.Code;

namespace TG.TrustMetric
{
	public abstract class TrustMatrixBuilder
	{

		protected Matrix<float> originalTrustMatrix;
		protected Matrix<float> estimatedTrustMatrix;
		protected BFSQueue queue;

		public TrustMatrixBuilder(Matrix<float> trustMatrix)
		{
			this.originalTrustMatrix = trustMatrix;
			this.estimatedTrustMatrix = trustMatrix.Clone();
		}

		public virtual Matrix<float> BuildMatrix(int neighbourhoodDistance)
		{
			foreach (int sourceUser in this.originalTrustMatrix.Rows)
			{
				BFSQueue queue = new BFSQueue(neighbourhoodDistance);

				foreach (int trustedUser in this.originalTrustMatrix[sourceUser])
					queue.Enqueue(new BFSNode(trustedUser, 1));

				while (queue.Count() != 0)
				{
					BFSNode node = queue.Deque();

					if (this.originalTrustMatrix.HasRow(node.value))
						VisitNode(queue, node, sourceUser, neighbourhoodDistance);
				}
			}

			return this.estimatedTrustMatrix;
		}

		protected abstract void VisitNode(BFSQueue queue, BFSNode node, int sourceUser, int neighbourhoodDistance);

		protected float BasicTrustMetric(int neighbourhoodDistance, int distance)
		{
			return (float)(neighbourhoodDistance - distance + 1) / neighbourhoodDistance;
		}
	}
}