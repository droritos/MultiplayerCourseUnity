using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SessionInfoConverter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI sesstionName;
    [SerializeField] TextMeshProUGUI playersInfo;
    [SerializeField] Button enterSession;

    public void UpdateSession(string sesstionName , int currentPlayer ,int maxPlayers, UnityEngine.Events.UnityAction call = default)
    {
        this.sesstionName.text = sesstionName;
        playersInfo.text = currentPlayer + "/" + maxPlayers;

        enterSession.onClick.AddListener(call);
    }

}
