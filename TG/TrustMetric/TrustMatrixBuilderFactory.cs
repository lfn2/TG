using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.TrustMetric
{
	public class TrustMatrixBuilderFactory
	{
		public static TrustMatrixBuilder GetMatrixBuilder(EMetric metric, Matrix<float> trustMatrix, int topN)
		{
			TrustMatrixBuilder matrixBuilder = null;

			switch (metric)
			{
				case EMetric.LinearTrustPropagation:
					matrixBuilder = new LinearTrustPropagation(trustMatrix);
					break;
				case EMetric.ResourceAllocation:
					matrixBuilder = new ResourceAllocation(trustMatrix, topN);
					break;
			}

			return matrixBuilder;
		}
	}
}
