using System.Threading;
using Cysharp.Threading.Tasks;
using FairyGUI;

namespace CBK.Framework.UI.FairyGUI
{
    public static class FairyGUIUtility
    {
        /// <summary>
        /// 异步创建GObject
        /// </summary>
        public static UniTask<GObject> CreateObjectAsync(string packageName, string componentName, CancellationToken cancellationToken)
        {
            return CreateObjectAsync(UIPackage.URL_PREFIX + packageName + "/" + componentName, cancellationToken);
        }
        
        /// <summary>
        /// 异步创建GObject
        /// </summary>
        public static async UniTask<GObject> CreateObjectAsync(string url, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;
            
            var completionSource = AutoResetUniTaskCompletionSource<GObject>.Create();
            var uniTask = completionSource.Task;
            
            UIPackage.CreateObjectFromURLAsync(url, o =>
            {
                completionSource.TrySetResult(o);
            });

            var gObject = await uniTask;
            
            await UniTask.SwitchToMainThread();
            
            if (gObject == null)
                return null;
            
            if (!cancellationToken.IsCancellationRequested)
                return gObject;
            
            gObject.Dispose();
            return null;
        }

        /// <summary>
        /// 从对象池中异步获取GObject
        /// </summary>
        public static async UniTask<GObject> CreateObjectAsync(this GObjectPool pool, string url, CancellationToken cancellationToken)
        {
            var gObject = pool.GetObjectFromPoolDirectly(url);
            if (gObject != null)
                return gObject;

            gObject = await CreateObjectAsync(url, CancellationToken.None);
            if (gObject == null)
                return null;
            
            if (!cancellationToken.IsCancellationRequested)
                return gObject;
            
            pool.ReturnObject(gObject);
            return null;
        }
    }
}