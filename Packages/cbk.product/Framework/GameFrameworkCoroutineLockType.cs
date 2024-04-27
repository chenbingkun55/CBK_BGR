using System.Threading;
using Cysharp.Threading.Tasks;
using CBK.Framework.CoroutineLock;

namespace CBK.Framework
{
    /// <summary>
    /// 框架协程锁类型定义
    /// </summary>
    public enum GameFrameworkCoroutineLockType : uint
    {
        /// <summary>
        /// 打开UI key为UIKey的hash值
        /// </summary>
        OpenUI = 1,
    }
    
    public static class GameFrameworkCoroutineLockTypeExtensions
    {
        /// <summary>
        /// 分配协程锁
        /// </summary>
        public static UniTask<ICoroutineLock> Allocate(this ICoroutineLockService service, GameFrameworkCoroutineLockType type, long key, CancellationToken token = default)
        {
            return service.Allocate((uint)type, key, token);
        }
        
        /// <summary>
        /// 分配协程锁
        /// </summary>
        public static UniTask<ICoroutineLock> Allocate(this ICoroutineLockService service, GameFrameworkCoroutineLockType type, long key, uint priority, CancellationToken token = default)
        {
            return service.Allocate((uint)type, key, priority, token);
        }
    }
}