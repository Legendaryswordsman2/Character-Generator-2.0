using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiscordController : MonoBehaviour
{
    public static string Username { get; private set; }

    public static bool RetreivedUsername { get; private set; } = false;

    Discord.Discord discord;
    Discord.UserManager userManager;

    [SerializeField] long applicationID;

    [Space]

    //[SerializeField] string details;
    [SerializeField] string state;

    //[Space]

    private void Awake()
    {
        try
        {
            discord = new Discord.Discord(applicationID, (ulong)Discord.CreateFlags.NoRequireDiscord);
            userManager = discord.GetUserManager();
            userManager.OnCurrentUserUpdate += UserManager_OnCurrentUserUpdate;

            StartCoroutine(UpdateDiscordActivity());
        }
        catch
        {
            Debug.Log("Unable to connect to discord (Discord is likely not open or installed)");
            Destroy(this);
        }
    }


    IEnumerator UpdateDiscordActivity()
    {
        var activityManager = discord.GetActivityManager();

        Discord.Activity activity;
        activity = new Discord.Activity
        {
            State = state,
            Assets =
            {
                LargeImage = "logo",
                LargeText = "Character Generator 2.0"
            },

            Timestamps =
            {
                Start = System.DateTimeOffset.Now.ToUnixTimeMilliseconds()
            }
        };

        bool done = false;

        activityManager.UpdateActivity(activity, (res) =>
        {
            if (res == Discord.Result.Ok)
            {
                //Debug.Log("Discord status set to: " + detailsDescription);
                //Debug.Log("Updated State: " + updateState);
            }
            else
            {
                Debug.Log("Discord status failed!");
            }

            done = true;
        });
        yield return new WaitWhile(() => done == false);
    }
    private void UserManager_OnCurrentUserUpdate()
    {
        string discriminator = userManager.GetCurrentUser().Discriminator;

        if (discriminator == "0")
            Username = userManager.GetCurrentUser().Username;
        else
            Username = userManager.GetCurrentUser().Username + "#" + userManager.GetCurrentUser().Discriminator;

        RetreivedUsername = true;
    }

    private void Update()
    {
        discord.RunCallbacks();
    }

    private void OnDestroy()
    {
        userManager.OnCurrentUserUpdate -= UserManager_OnCurrentUserUpdate;

#if UNITY_EDITOR
        discord?.Dispose();
#endif
    }
}

