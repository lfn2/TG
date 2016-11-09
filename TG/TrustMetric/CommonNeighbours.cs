using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.Code;

namespace TG.TrustMetric
{
	public class CommonNeighbours : TrustMatrixBuilder
	{
		private int? topN;

		public CommonNeighbours(Matrix<float> trustMatrix) : base(trustMatrix)
		{
		}

		public CommonNeighbours(Matrix<float> trustMatrix, int topN) : base(trustMatrix)
		{
			this.topN = topN;
		}

		protected override void VisitNode(BFSQueue queue, BFSNode node, int sourceUser, int neighbourhoodDistance)
		{
			Dictionary<int, float> commonNeighbours = new Dictionary<int, float>();

			foreach (int user in this.originalTrustMatrix[node.value])
				if (!this.estimatedTrustMatrix[sourceUser].Contains(user))
					commonNeighbours[user] = CN(sourceUser, user);

			List<KeyValuePair<int, float>> cnList = commonNeighbours.ToList();
			if (topN.HasValue)
			{
				cnList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
				if (topN < commonNeighbours.Count)
					cnList.RemoveRange(topN.Value, commonNeighbours.Count - topN.Value);
			}

		}

		private float CN(int x, int y)
		{
			int cn = 0;

			foreach (int user in this.originalTrustMatrix[x])
				if (this.originalTrustMatrix.HasRow(y) && this.originalTrustMatrix[y].Contains(user))
					cn++;

			return cn;
		}
	}
}
