namespace CBK.Logic.Launch
{
    /// <summary>
    /// 启动环节定义
    /// </summary>
    public enum LaunchStep
    {
        /// <summary>
        /// 启动开始
        /// </summary>
        LaunchStart,
        
        /// <summary>
        /// 初始化本地化服务
        /// </summary>
        InitLocalization,
        
        /// <summary>
        /// 打开启动界面
        /// </summary>
        OpenLaunchForm,
        
        /// <summary>
        /// 检查版本
        /// </summary>
        CheckVersion,
        
        /// <summary>
        /// 更新资源
        /// </summary>
        UpdateResource,
        
        /// <summary>
        /// 加载配置表
        /// </summary>
        LoadConfig,
        
        /// <summary>
        /// 创建玩法实例
        /// </summary>
        CreateGame,
        
        /// <summary>
        /// 加载登陆界面
        /// </summary>
        OpenLoginForm,
        
        /// <summary>
        /// 启动完成
        /// </summary>
        LaunchComplete,
    }
}