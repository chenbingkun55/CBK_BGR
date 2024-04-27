//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using CBK.Framework.Reference;

namespace CBK.Framework.Network
{
    /// <summary>
    /// 网络消息包基类。
    /// </summary>
    public abstract class Packet : EventArgs, IReference
    {
        public abstract int SerialId { get; set; }
        public abstract IReferenceService ReferenceService { get; set; }
        public abstract void OnRecycle();
    }
}
