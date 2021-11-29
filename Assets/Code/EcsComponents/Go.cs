using System;
using UnityEngine;

namespace Code.EcsComponents
{
    [Serializable]
    public struct Go
    {
        public readonly GameObject go;

        public Go(GameObject go) {
            this.go = go;
        }
    }
}