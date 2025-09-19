using UnityEngine;
using TMPro;

namespace Localisation
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextLocaliserUI : MonoBehaviour
    {
        TextMeshProUGUI textField;

        public string Key;

        void OnLocalise()
        {
            textField = GetComponent<TextMeshProUGUI>();
            string value = LocalisationSystem.GetLocalisedValue(Key);
            textField.text = value;
        }

        void Start()
        {
            OnLocalise();
        }

        void OnEnable()
        {
            OnLocalise();
            LocalisationSystem.Localise += OnLocalise;
        }

        void OnDisable()
        {
            LocalisationSystem.Localise -= OnLocalise;
        }
    }
}