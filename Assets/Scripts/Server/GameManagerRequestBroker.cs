using System;
using UnityEngine;

namespace Game.Server
{
    public static class GameManagerRequestBroker
    {
        public static event Action<Vector3> OnRequestBomb;
        public static void RequestBomb(Vector3 position) => OnRequestBomb?.Invoke(position);
    }
}
