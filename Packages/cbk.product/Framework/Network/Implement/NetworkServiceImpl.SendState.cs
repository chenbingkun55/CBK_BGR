//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.IO;

namespace CBK.Framework.Network
{
    internal sealed partial class NetworkServiceImpl
    {
        private sealed class SendState : IDisposable
        {
            private const int DefaultBufferLength = 1024 * 64;
            public MemoryStream Stream { get; private set; } = new(DefaultBufferLength);

            public void Reset()
            {
                Stream.Position = 0L;
                Stream.SetLength(0L);
            }

            public void Dispose()
            {
                if (Stream == null)
                    return;

                Stream.Dispose();
                Stream = null;
            }
        }
    }
}