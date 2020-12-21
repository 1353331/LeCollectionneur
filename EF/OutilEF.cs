using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.EF
{
	public class OutilEF
	{
		public static Context ctx;
		public OutilEF()
		{
			try
			{
				ctx = new Context();
				
			}
			catch (Exception e)
			{

				throw;
			}
		}
	}
}
