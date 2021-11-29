using System;
using Code.SO;

namespace Code.EcsComponents
{
    [Serializable]
    public struct BallType
    {
        public readonly BallTypeConfig config;

        public BallType(BallTypeConfig config) {
            this.config = config;
        }
    }
}