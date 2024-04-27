namespace CBK.Framework.Path
{
    /// <summary>
    /// 路径服务
    /// </summary>
    public interface IPathService
    {
        string Resolve(PathType pathType, string path);
        string Resolve(PathType pathType, string path1, string path2);
        string Resolve(PathType pathType, string path1, string path2, string path3);
        string Resolve(PathType pathType, params string[] paths);
    }
}