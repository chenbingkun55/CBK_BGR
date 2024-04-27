namespace CBK.Framework.App
{
    public interface IApp
    {
        /// <summary>
        /// 热重载游戏
        /// </summary>
        void Reboot();

        /// <summary>
        /// 退出游戏
        /// </summary>
        void Quit(int errorCode = 0);
    }
}