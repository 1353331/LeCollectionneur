using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Outils
{
	// Argument de l'évènement: ce que l'évènement transporte comme information
	public class ChangementImageItemEventArgs
	{
		//public int ItemId { get; set; }

		//public ChangementImageItemEventArgs(int Id)
		//{
		//	ItemId = Id;
		//}
	}
	// Le prototype que devront respecter les méthodes qui désirent réagir à l'évènement
	//public delegate void ChangementImageItemEventHandler(object sender, ChangementImageItemEventArgs e);

	class Evenements
    {

        //static public event ChangementImageItemEventHandler ChangementImageItem;
        //static public void OnChangementImageItem(ChangementImageItemEventArgs e)
        //{
        //    ChangementImageItem?.Invoke(null, e);
        //}
    }
}
