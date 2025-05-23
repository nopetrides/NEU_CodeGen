﻿using System.Collections;
using UnityEngine;

namespace UniStorm.Utility
{
    public class DespawnObjectUniStorm : MonoBehaviour
    {
        public int Seconds = 3;

        void OnEnable()
        {
            StartCoroutine(Despawn());
        }

        IEnumerator Despawn()
        {
            yield return new WaitForSeconds(Seconds);
            UniStormPool.Despawn(this.gameObject);
        }
    }
}