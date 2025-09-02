using UnityEngine;
using PlayFab;
using PlayFab.ServerModels;
using PlayFab.ClientModels;
using System.Collections;
using UnityEngine.Networking;
using System.Text;

public class ModeratorSystem : MonoBehaviour
{
    public string[] hostIds = new string[] { }; // Add moderator IDs here
    private string discordWebhook = "YOUR_WEBHOOK_URL";

    private string currentPlayerId = ""; // Fill this after PlayFab login

    // Example: Ban a player
    public void BanPlayer(string targetId)
    {
        if (IsHost(currentPlayerId))
        {
            StartCoroutine(SendWebhook("```" + currentPlayerId + " is banning " + targetId + "```"));

            var banRequest = new BanUsersRequest
            {
                Bans = new System.Collections.Generic.List<BanRequest>
                {
                    new BanRequest
                    {
                        PlayFabId = targetId,
                        Reason = "BANNED BY MODERATOR"
                    }
                }
            };

            PlayFabServerAPI.BanUsers(banRequest, OnSuccess, OnError);
        }
    }

    // Example: Give self SR
    public void GiveSelfCurrency()
    {
        if (IsHost(currentPlayerId))
        {
            StartCoroutine(SendWebhook("```" + currentPlayerId + " is giving themselves SR```"));

            var request = new AddUserVirtualCurrencyRequest
            {
                PlayFabId = currentPlayerId,
                VirtualCurrency = "SR",
                Amount = 10
            };

            PlayFabServerAPI.AddUserVirtualCurrency(request, OnSuccess, OnError);
        }
    }

    // Example: Give another player SR
    public void GiveCurrencyToPlayer(string targetId, int amount)
    {
        if (IsHost(currentPlayerId))
        {
            StartCoroutine(SendWebhook("```" + currentPlayerId + " is giving SR to " + targetId + "```"));

            var request = new AddUserVirtualCurrencyRequest
            {
                PlayFabId = targetId,
                VirtualCurrency = "SR",
                Amount = amount
            };

            PlayFabServerAPI.AddUserVirtualCurrency(request, OnSuccess, OnError);
        }
    }

    // === Helpers ===
    private bool IsHost(string playerId)
    {
        foreach (string id in hostIds)
            if (id == playerId) return true;
        return false;
    }

    private IEnumerator SendWebhook(string msg)
    {
        var json = "{\"content\":\"" + msg + "\"}";
        var request = new UnityWebRequest(discordWebhook, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
    }

    private void OnSuccess(object result)
    {
        Debug.Log("Success: " + result);
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
    }
}
