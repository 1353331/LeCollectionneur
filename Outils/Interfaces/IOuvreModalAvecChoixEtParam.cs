using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeCollectionneur.Modeles;

namespace LeCollectionneur.Outils.Interfaces
{
    public interface IOuvreModalAvecChoixEtParam<T>
    {
        void OuvrirModal(T objet, string nom);
    }
}
