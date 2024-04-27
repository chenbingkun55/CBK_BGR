namespace CBK.Framework.UI.Exception
{
    /// <summary>
    /// UI分组已创建的异常
    /// </summary>
    public sealed class UIGroupAlreadyCreatedException : System.Exception
    {
        public string GroupName { get; }

        public UIGroupAlreadyCreatedException(string groupName)
        {
            GroupName = groupName;
        }
    }
}