using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameItem", menuName = "Game/Item")]
public class GameItem : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField] public Sprite Icon;
}
