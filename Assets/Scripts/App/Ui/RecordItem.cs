using UnityEngine;
using UnityEngine.UI;

namespace CBK.Product.Ui
{
    public class RecordItem : MonoBehaviour
    {
        [SerializeField] public Text dateTime = default;
        [SerializeField] public Text eatType = default;
        [SerializeField] public Text afterMealTime = default;
        [SerializeField] public Text medicineType = default;
        [SerializeField] public Text medicineAmount = default;
        [SerializeField] public Text sportType = default;
        [SerializeField] public Text sportAmount = default;
        [SerializeField] public Text monitorValue = default;
        [SerializeField] public Text notice = default;

        // 记录Id
        public string GUID = default;
    }
}
