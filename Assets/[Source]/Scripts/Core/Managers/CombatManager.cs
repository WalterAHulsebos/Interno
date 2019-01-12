using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class CombatManager
    {
        private List<ICombatable> combatants;
        private int activeCombatantIndex = -1;
        private bool returnOnNewRound;

        public delegate void OnNext();
        public delegate void OnNewRound();

        private event OnNext OnNextEvent;
        private event OnNewRound OnNewRoundEvent;

        public CombatManager(int maxCombatants, bool returnOnNewRound)
        {
            this.returnOnNewRound = returnOnNewRound;
            combatants = new List<ICombatable>(maxCombatants);
        }

        public void Next()
        {
            activeCombatantIndex++;

            if(activeCombatantIndex >= combatants.Count)
            {
                activeCombatantIndex = 0;
                Sort();

                OnNewRoundEvent();

                if (returnOnNewRound)
                    return;
            }

            OnNextEvent();
            combatants[activeCombatantIndex].OnActiveCombatant();
        }

        private void RemoveNonActiveCombatants(ICombatable combatable)
        {
            int combatantsCount = combatants.Count;
            for (int i = 0; i < combatantsCount; i++)
            {
                if (!combatable.Equals(combatants[i]))
                    continue;

                if (i <= activeCombatantIndex)
                    activeCombatantIndex--;
                combatants.RemoveAt(i);
                return;
            }
        }

        public void Add(ICombatable combatable)
        {
            combatants.Add(combatable);
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
            combatants.Sort();
        }
    }

    public interface ICombatable : IComparable<ICombatable>
    {
        void OnActiveCombatant();
    }
}
