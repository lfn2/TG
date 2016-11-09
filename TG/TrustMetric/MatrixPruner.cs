using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.TrustMetric
{
	public abstract class MatrixPruner
	{

		public void PruneMatrix(Matrix<float> trustMatrix, int n)
		{
			foreach (int user in trustMatrix.Rows)
				Prune(trustMatrix, user, n);


		}

		protected abstract void Prune(Matrix<float> trustMatrix, int user, int n);

	}
}
