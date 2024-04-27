using System;
using System.Collections.Generic;

namespace CBK.Framework.Utility
{
    /// <summary>
    /// Disposable管理组 用于管理多个IDisposable对象进行统一Dispose
    /// </summary>
    public sealed class DisposableGroup : IDisposable
    {
        public void Add(IDisposable disposable)
        {
            m_Disposables.Add(disposable);
        }
        
        public void Dispose()
        {
            for (var index = m_Disposables.Count - 1; index >= 0; --index)
            {
                try
                {
                    m_Disposables[index].Dispose();
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }

            m_Disposables.Clear();
        }
        
        private readonly List<IDisposable> m_Disposables = new List<IDisposable>();
    }
    
    public static class DisposableGroupExtensions
    {
        /// <summary>
        /// 将Disposable对象添加到DisposableGroup中
        /// </summary>
        public static void AddTo(this IDisposable disposable, DisposableGroup disposableGroup)
        {
            disposableGroup.Add(disposable);
        }
    }
}