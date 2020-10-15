using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace LeCollectionneur.Outils.Messages
{
   public static class EvenementSysteme
   {
      private static IEventAggregator _courant;
      public static IEventAggregator Courant
      {
         get
         {
            return _courant ?? (_courant = new EventAggregator());
         }
      }

      private static PubSubEvent<TEvent> ObtenirEvenement<TEvent>()
      {
         return Courant.GetEvent<PubSubEvent<TEvent>>();
      }

      public static void Publier<TEvent>()
      {
         Publier<TEvent>(default(TEvent));
      }

      public static void Publier<TEvent>(TEvent evenement)
      {
         ObtenirEvenement<TEvent>().Publish(evenement);
      }

      public static SubscriptionToken Abonnement<TEvent>(Action action, ThreadOption optionThread = ThreadOption.PublisherThread, bool garderReferenceAbonnement = false)
      {
         return Abonnement<TEvent>(e => action(), optionThread, garderReferenceAbonnement);
      }

      public static SubscriptionToken Abonnement<TEvent>(Action<TEvent> action, ThreadOption optionThread = ThreadOption.PublisherThread, bool garderReferenceAbonnement = false, Predicate<TEvent> filtre = null)
      {
         return ObtenirEvenement<TEvent>().Subscribe(action, optionThread, garderReferenceAbonnement, filtre);
      }

      public static void Desabonnement<TEvent>(SubscriptionToken token)
      {
         ObtenirEvenement<TEvent>().Unsubscribe(token);
      }
      public static void Desabonnement<TEvent>(Action<TEvent> abonne)
      {
         ObtenirEvenement<TEvent>().Unsubscribe(abonne);
      }
   }
}
