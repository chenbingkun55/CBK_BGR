//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

namespace CBK.Framework.Network
{
    internal sealed partial class NetworkServiceImpl
    {
        private sealed class HeartBeatState
        {
            public float HeartBeatElapseSeconds { get; set; }
            public int MissHeartBeatCount { get; set; }

            public void Reset(bool resetHeartBeatElapseSeconds)
            {
                if (resetHeartBeatElapseSeconds)
                {
                    HeartBeatElapseSeconds = 0f;
                }

                MissHeartBeatCount = 0;
            }
        }
    }
}
