namespace CBK.Framework
{
    /// <summary>
    /// 游戏框架错误码
    /// </summary>
    public enum FrameworkErrorCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,
        
        /// <summary>
        /// 参数无效
        /// </summary>
        ParamInvalid = 1,
        
        /// <summary>
        /// 配置未找到
        /// </summary>
        ConfigNotFound = 2,

        /// <summary>
        /// 应答包类型不匹配
        /// </summary>
        ResponseTypeNotMatch = 1001,

        /// <summary>
        /// 应答包为空
        /// </summary>
        ResponseIsNull,

        /// <summary>
        /// 请求捕获到异常
        /// </summary>
        RequestCatchException,
        
        /// <summary>
        /// 请求已被取消
        /// </summary>
        RequestHasBeenCanceled,
        
        /// <summary>
        /// 手动取消
        /// </summary>
        ManualCanceled,

        #region [UI服务错误码 2001]

        /// <summary>
        /// UI视图被关闭
        /// </summary>
        UIHasBeenClosed = 2001,
        
        /// <summary>
        /// UI资源加载失败
        /// </summary>
        UIResourceLoadFailed,
        
        /// <summary>
        /// UI组未定义
        /// </summary>
        UIGroupUndefined,
        
        /// <summary>
        /// UI界面未定义
        /// </summary>
        UIFormUndefined,
        
        /// <summary>
        /// UI请求 - 界面被关闭
        /// </summary>
        UIRequestFormClosed,
        
        /// <summary>
        /// UI请求 - 请求被覆盖
        /// </summary>
        UIRequestOverride,
        
        /// <summary>
        /// UI请求 - 请求类型无效
        /// </summary>
        UIRequestTypeInvalid,

        #endregion

        #region [路由服务错误码 3001]

        /// <summary>
        /// 未定义的路由
        /// </summary>
        RouteUndefined = 3001,
        
        /// <summary>
        /// 路由格式错误
        /// </summary>
        RouteFormatError,
        
        /// <summary>
        /// 路由不可为空的参数未定义
        /// </summary>
        RouteNotNullParameterUndefined,
        
        /// <summary>
        /// 路由参数类型不匹配
        /// </summary>
        RouteParameterTypeNotMatch,

        #endregion

        #region [本地化服务错误码 4001]

        /// <summary>
        /// 本地化服务 - 不支持的语言
        /// </summary>
        LocalizationNotSupport = 4001,
        
        /// <summary>
        /// 本地化服务 - 加载失败
        /// </summary>
        LocalizationLoadFailed,

        #endregion

        #region [网络服务错误码 5001]

        /// <summary>
        /// 网络服务 - 未知错误
        /// </summary>
        NetworkUnknown = 5001,

        /// <summary>
        /// 网络服务 - 地址族错误。
        /// </summary>
        NetworkAddressFamilyError,

        /// <summary>
        /// 网络服务 - Socket 错误。
        /// </summary>
        NetworkSocketError,

        /// <summary>
        /// 网络服务 - 连接错误。
        /// </summary>
        NetworkConnectError,

        /// <summary>
        /// 网络服务 - 发送错误。
        /// </summary>
        NetworkSendError,

        /// <summary>
        /// 网络服务 - 接收错误。
        /// </summary>
        NetworkReceiveError,

        /// <summary>
        /// 网络服务 - 序列化错误。
        /// </summary>
        NetworkSerializeError,

        /// <summary>
        /// 网络服务 - 反序列化消息包头错误。
        /// </summary>
        NetworkDeserializePacketHeaderError,

        /// <summary>
        /// 网络服务 - 反序列化消息包错误。
        /// </summary>
        NetworkDeserializePacketError,

        #endregion

        #region [游戏服务器交互服务错误码 6001]

        /// <summary>
        /// 游戏服务器交互服务 - 手动断开连接
        /// </summary>
        GameServerManualDisconnect = 6001,
        
        /// <summary>
        /// 游戏服务器交互服务 - 断开连接
        /// </summary>
        GameServerDisconnect,

        #endregion

        #region [Http服务错误码 7001]

        /// <summary>
        /// Http服务 - 状态码错误
        /// </summary>
        HttpStatusError = 7001,

        #endregion

        /// <summary>
        /// 框架错误码的最大值
        /// </summary>
        Max = 1000000,
    }
}