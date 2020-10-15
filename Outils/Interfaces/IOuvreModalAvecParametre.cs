using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeCollectionneur.Modeles;

namespace LeCollectionneur.Outils.Interfaces
{
	public interface IOuvreModalAvecParametre<T>
	{
		void OuvrirModal(T objet);	
	}
}
