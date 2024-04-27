using CBK.Entry;
using UnityEngine;

namespace CBK.Logic
{
    public static class LogicEntry
    {
        public static void Init(IEntryLoader entryLoader)
        {
            new GameObject(nameof(AppLogic)).AddComponent<AppLogic>().Init(entryLoader);
        }
    }
}