using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Outils
{
    public static class Validateur
    {
        public static string Echappement(string entree)
        {
            string modif = "";
            for (int i = 0; i < entree.Length; i++)
            {
                if (entree[i] == '\'' || entree[i] == ';')
                {
                    modif += $"\\{entree[i]}";
                }
                else if (entree[i]=='\\')
                {
                    modif+=""; // Ne pas ajouter les backslashs à la chaine.
                }
                else
                modif += entree[i];
            }
            return modif;
        }
    }
}
