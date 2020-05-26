using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EyeTracking.Scripts
{
    public class UIObject : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Toggle _toggle;
        private ObjectDescription _description;

        public void UpdateDescription(ObjectDescription description)
        {
            _text.text = description.Text;
            _description = description;
        }

        public Toggle Toggle => _toggle;

        public GameObject ObjectToSpawn() => _description.Object;

        public string Name => _description.Text;
    }
}