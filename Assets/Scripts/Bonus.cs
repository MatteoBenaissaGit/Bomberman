using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    [Serializable]
    public enum BonusType
    {
        AddBomb = 0,
        BombRange = 1
    }

    [SerializeField] public BonusType Type;
}


