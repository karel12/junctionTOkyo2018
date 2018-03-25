using UnityEngine;
using UnityEngine.UI;

namespace Assets.Utils
{
    public class ViewHelpers
    {
        public static void RefreshTextToTriggerRendering(MonoBehaviour parent)
        {
            // fix in order to trigger a re -rendering of texts to increase sharpness
            var allTexts = parent.GetComponentsInChildren<Text>(true);
            foreach (var text in allTexts)
            {
                text.text = text.text + " ";
                text.text = text.text.TrimEnd();
            }
        }

    }
}