using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShortcutSlimes.Components
{
    internal class AutoDepositAssistance : MonoBehaviour
    {
        public static ScorePlort cachedScorePlort;

        void Awake()
        {
            if (cachedScorePlort is null)
                cachedScorePlort = GameObject.Find("zoneConservatory/cellConservatory/Sector/Ranch Features/techMarket (1)/triggerDeposit").GetComponent<ScorePlort>();
        }
    }
}
