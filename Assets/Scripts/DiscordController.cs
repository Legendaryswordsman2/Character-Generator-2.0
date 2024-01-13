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

    long startTime;

    private void Awake()
    {
        try
        {
            startTime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

            discord = new Discord.Discord(applicationID, (ulong)Discord.CreateFlags.NoRequireDiscord);
            userManager = discord.GetUserManager();
            userManager.OnCurrentUserUpdate += UserManager_OnCurrentUserUpdate;

            StartCoroutine(UpdateDiscordActivity());

            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }
        catch
        {
#if UNITY_EDITOR
            Debug.Log("Unable to connect to discord (Discord is likely not open or installed)");
#endif
            Destroy(this);
        }
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        StartCoroutine(UpdateDiscordActivity());
    }

    IEnumerator UpdateDiscordActivity()
    {
        var activityManager = discord.GetActivityManager();

        string state = null;

        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                state = "Making Characters";
                break;
            case 1:
                state = "Trying Characters";
                break;
        }

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
                Start = startTime
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
        try
        {
            discord.RunCallbacks();
        }
        catch (System.Exception exception)
        {
            Debug.Log(exception);
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (userManager != null)
            userManager.OnCurrentUserUpdate -= UserManager_OnCurrentUserUpdate;

        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;

#if UNITY_EDITOR
        discord?.Dispose();
#endif
    }
}

