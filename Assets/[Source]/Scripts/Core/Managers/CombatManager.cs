using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Combat
{
    public class CombatManager<T> where T : ICombatable
    {
        public List<T> Combatants { get; private set; }
        private int activeCombatantIndex = -1;
        private bool returnOnNewRound;

        public delegate void OnNext();
        public delegate void OnNewRound();

        private event OnNext OnNextEvent;
        private event OnNewRound OnNewRoundEvent;

        public CombatManager(int maxCombatants, bool returnOnNewRound)
        {
            this.returnOnNewRound = returnOnNewRound;
            Combatants = new List<T>(maxCombatants);
        }

        public void Next()
        {
            activeCombatantIndex++;

            if(activeCombatantIndex >= Combatants.Count)
            {
                activeCombatantIndex = 0;
                Sort();

                if(OnNewRoundEvent != null)
                    OnNewRoundEvent();

                if (returnOnNewRound)
                    return;
            }

            if(OnNextEvent != null)
                OnNextEvent();
            Combatants[activeCombatantIndex].OnActiveCombatant();
        }

        public void Remove(T combatable)
        {
            int combatantsCount = Combatants.Count;
            for (int i = 0; i < combatantsCount; i++)
            {
                if (!combatable.Equals(Combatants[i]))
                    continue;

                if (i <= activeCombatantIndex)
                    activeCombatantIndex--;
                Combatants.RemoveAt(i);
                return;
            }
        }

        public void Add(T combatable)
        {
            Combatants.Add(combatable);
        }

        #region Events
        public void Add(OnNext onNext)
        {
            OnNextEvent += onNext;
        }

        public void Add(OnNewRound onNewRound)
        {
            OnNewRoundEvent += onNewRound;
        }

        public void Remove(OnNext onNext)
        {
            OnNextEvent -= onNext;
        }

        public void Remove(OnNewRound onNewRound)
        {
            OnNewRoundEvent -= onNewRound;
        }
        #endregion

        public void Sort()
        {
            Combatants.Sort();
        }

        public void Clear()
        {
            Combatants.Clear();
            activeCombatantIndex = -1;
        }
    }

    public interface ICombatable : IComparable<ICombatable>
    {
        int CombatOrder { get; }
        void OnActiveCombatant();       
    }
}
