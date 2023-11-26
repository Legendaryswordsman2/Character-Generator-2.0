using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlazerTech
{
    public class Log : MonoBehaviour, IPointerClickHandler
    {
        string logMessage;
        string logDetails;

        string LogData;

        [Space]

        [SerializeField] TMP_Text text;
        [SerializeField] ContentSizeFitter contentSizeFitter;

        bool isBaseInfo = false;

        bool isExpanded = false;

        public void Init(float fontSize)
        {
            text.fontSize = fontSize;
        }
        public void SetupLog(string _logMessage, string _logDetails, LogType type)
        {
            logMessage = _logMessage;
            logDetails = _logDetails;

            text.text = logMessage;

            switch (type)
            {
                case LogType.Log:
                    text.color = Color.white;
                    break;
                case LogType.Error:
                    text.color = Color.red;
                    break;
                case LogType.Warning:
                    text.color = Color.yellow;
                    break;
                case LogType.Exception:
                    text.color = Color.red;
                    break;
                default:
                    break;
            }

            contentSizeFitter.SetLayoutVertical();

            gameObject.SetActive(true);
        }

        public void SetupBaseInfoLog(string _logMessage)
        {
            text.text = _logMessage;

            isBaseInfo = true;

            //contentSizeFitter.SetLayoutVertical();

            gameObject.SetActive(true);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isBaseInfo) return;

            if (!isExpanded)
            {
                text.text = LogData + logMessage + "\n" + logDetails;
                isExpanded = true;
            }
            else
            {
                text.text = LogData + logMessage;
                isExpanded = false;
            }
            contentSizeFitter.SetLayoutVertical();
        }
    }
}