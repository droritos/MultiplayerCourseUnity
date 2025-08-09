﻿using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GameData.asset", menuName = "Game/GameData")]
    public class GameData : ScriptableObject
    {
        public CharacterData[] characters;
    }
}
