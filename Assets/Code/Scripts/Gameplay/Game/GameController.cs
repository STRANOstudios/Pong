using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using PsychoGarden.TriggerEvents;

namespace AndreaFrigerio.Gameplay.Controller
{
    [HideMonoScript]
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text scoreDx;

        [SerializeField]
        private TMP_Text scoreSx;

        [SerializeField]
        private TriggerEvent OnWin;

        private int scoreDxValue = 0;
        private int scoreSxValue = 0;

        public void UpdateScore(bool isLeft)
        {
            if (isLeft)
            {
                scoreSxValue++;
                scoreSx.text = scoreSxValue.ToString();
            }
            else
            {
                scoreDxValue++;
                scoreDx.text = scoreDxValue.ToString();
            }

            if (scoreDxValue >= 10 || scoreSxValue >= 10)
            {
                scoreDxValue = 0;
                scoreSxValue = 0;

                OnWin?.Invoke(this.transform);
            }
        }
    }
}
