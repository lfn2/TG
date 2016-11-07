using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.Code;

namespace TG.TrustMetric
{
	public class LinearTrustPropagation : TrustMatrixBuilder
	{
		public LinearTrustPropagation(Matrix<float> trustMatrix) : base(trustMatrix)
		{
		}

		protected override void VisitNode(BFSQueue queue, BFSNode node, int sourceUser, int neighbourhoodDistance)
		{
			foreach (int user in this.originalTrustMatrix[node.value])
			{
				if (!this.estimatedTrustMatrix[sourceUser].Contains(user))
				{
					int distance = node.distance + 1;
					this.estimatedTrustMatrix[sourceUser, user] = BasicTrustMetric(neighbourhoodDistance, distance);

					queue.Enqueue(new BFSNode(user, distance));
				}
			}
		}

	}
}
