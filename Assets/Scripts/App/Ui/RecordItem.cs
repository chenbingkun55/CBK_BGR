using UnityEngine;
using UnityEngine.UI;

namespace CBK.Product.Ui
{
    public class RecordItem : MonoBehaviour
    {
        [SerializeField] public Text dateTime = default;
        [SerializeField] public Text eatType = default;
        [SerializeField] public Text afterMealTime = default;
        [SerializeField] public Text medicineName = default;
        [SerializeField] public Text medicineAmount = default;
        [SerializeField] public Text notice = default;
    }
}
