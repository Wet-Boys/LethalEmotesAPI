using BepInEx;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Text;
using EmotesAPI;
using System.Collections;
using LethalEmotesAPI;

public class EmoteWheel : MonoBehaviour
{
    internal GameObject text;
    internal static GameObject dontPlayButton;

    internal List<GameObject> gameObjects = new List<GameObject>();
    //TODO input effectively
    //internal Image joy;

    //internal RoR2.UI.MPInput input = GameObject.Find("MPEventSystem Player0").GetComponent<RoR2.UI.MPInput>();
    //internal RoR2.UI.MPEventSystem events;

    internal string[] leftPage = new string[8];
    internal string[] middlePage = new string[8];
    internal string[] rightPage = new string[8];
    int activePage = 1; //0 left, 1 middle, 2 right

    GameObject selected;
    int selectedNum;
    float XScale = 1, YScale = 1;
    string bLock = "asd";
    void Start()
    {
        transform.localPosition = new Vector3(0, 2000, 0);
        EmotesInputSettings.Instance.EmoteWheel.started += EmoteWheel_Pressed;
        EmotesInputSettings.Instance.EmoteWheel.canceled += EmoteWheel_DePressed;
        EmotesInputSettings.Instance.Left.started += Left_performed;
        EmotesInputSettings.Instance.Right.started += Right_performed;
        EmotesInputSettings.Instance.SetCurrentEmoteToWheel.started += SetCurrentEmoteToWheel_started;
    }

    private void SetCurrentEmoteToWheel_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (selected != null && CustomEmotesAPI.allClipNames.Contains(CustomEmotesAPI.localMapper.currentClipName))
        {
            //TODO TMPUGUI
            //selected.GetComponentInChildren<TextMeshProUGUI>().text = CustomEmotesAPI.localMapper.currentClipName;
            SaveWheelFromGame(activePage, selectedNum, CustomEmotesAPI.localMapper.currentClipName);
            int actualPage = activePage == 0 ? 1 : activePage == 1 ? 0 : 2;
            int number = (actualPage * 8) + selectedNum;
            //TODO TMPUGUI
            //ScrollManager.circularButtons[number].GetComponentInChildren<HGTextMeshProUGUI>().text = CustomEmotesAPI.localMapper.currentClipName;
            bLock = CustomEmotesAPI.localMapper.currentClipName;
            RefreshWheels();
        }
    }

    private void Right_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Vector3 v = new Vector3(0, 0, 0);
        if (transform.localPosition == v)
        {
            //Event.current.Use();
            if (activePage == 1)
            {
                activePage = 2;
                StartCoroutine(SwitchPage(rightPage));
            }
            if (activePage == 0)
            {
                activePage = 1;
                StartCoroutine(SwitchPage(middlePage));
            }
        }
    }

    private void Left_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Vector3 v = new Vector3(0, 0, 0);
        if (transform.localPosition == v)
        {
            //Event.current.Use();
            if (activePage == 1)
            {
                activePage = 0;
                StartCoroutine(SwitchPage(leftPage));
            }
            if (activePage == 2)
            {
                activePage = 1;
                StartCoroutine(SwitchPage(middlePage));
            }
        }
    }

    private void EmoteWheel_DePressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Vector3 v = new Vector3(0, 0, 0);
        if (transform.localPosition == v)
        {
            try
            {
                XScale = Screen.width / 1980f;
                YScale = Screen.height / 1080f;
                if (Math.Abs(Input.mousePosition.x - (Screen.width / 2.0f)) < 30f * XScale && Math.Abs(Input.mousePosition.y - (Screen.height / 2.0f)) < 30f * YScale)
                {
                    CustomEmotesAPI.PlayAnimation("none");
                }
                else
                {
                    //TODO TMPUGUI
                    //if (bLock != selected.GetComponentInChildren<TextMeshProUGUI>().text && !selected.GetComponentInChildren<TextMeshProUGUI>().text.StartsWith("Continue Playing Current E"))
                    //    CustomEmotesAPI.PlayAnimation(selected.GetComponentInChildren<TextMeshProUGUI>().text);
                }
            }
            catch (Exception e)
            {
                DebugClass.Log(e);
            }
            bLock = "asd";
            //TODO gamepad support?
            //if (events.cursorOpenerForGamepadCount > 0)
            //{
            //    events.cursorOpenerForGamepadCount -= 1;
            //    events.cursorOpenerCount -= 1;
            //}
        }
        transform.localPosition = new Vector3(0, 2000, 0);
        StartCoroutine(SwitchPage(middlePage));
        activePage = 1;
    }

    private void EmoteWheel_Pressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Vector3 v = new Vector3(0, 0, 0);
        if (transform.localPosition != v)
        {
            //TODO joy again
            //events.cursorOpenerForGamepadCount += 1;
            //events.cursorOpenerCount += 1;
            //CustomEmotesAPI.EmoteWheelOpened(joy.sprite);
        }
        transform.localPosition = v;

    }

    internal void RefreshWheels()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            //TODO emote wheel pages (TMPUGUI)
            //middlePage[i] = ScrollManager.circularButtons[i].GetComponentInChildren<HGTextMeshProUGUI>().text;
            //leftPage[i] = ScrollManager.circularButtons[i + 8].GetComponentInChildren<HGTextMeshProUGUI>().text;
            //rightPage[i] = ScrollManager.circularButtons[i + 16].GetComponentInChildren<HGTextMeshProUGUI>().text;
        }
    }
    bool started = false;
    void Update()
    {
        //TODO emote wheel update
        //if (!started)
        //{
        //    try
        //    {
        //        foreach (var item in CustomEmotesAPI.nameTokenSpritePairs)
        //        {
        //            //DebugClass.Log($"{NetworkUser.readOnlyLocalPlayersList[0].master?.GetBody().baseNameToken}         {item.nameToken}");
        //            if (NetworkUser.readOnlyLocalPlayersList[0].master?.GetBody().baseNameToken == item.nameToken)
        //            {
        //                //DebugClass.Log($"{item.nameToken}         {item.sprite}");
        //                joy.sprite = item.sprite;
        //                break;
        //            }
        //        }
        //        selected = gameObjects[0];
        //        selectedNum = 0;
        //        events = input.GetFieldValue<RoR2.UI.MPEventSystem>("eventSystem");
        //        RefreshWheels();
        //        started = true;
        //    }
        //    catch (Exception)
        //    {
        //        return;
        //    }
        //}
        //if (RoR2.PauseManager.isPaused)
        //    return;

        Vector3 v = new Vector3(0, 0, 0);
        if (transform.localPosition == v)
        {
            XScale = Screen.width / 1980f;
            YScale = Screen.height / 1080f;
            if (!(Math.Abs(Input.mousePosition.x - (Screen.width / 2.0f)) < 30f * XScale && Math.Abs(Input.mousePosition.y - (Screen.height / 2.0f)) < 30f * YScale))
            {
                float dist = 99999;
                foreach (var item in gameObjects)
                {
                    if (dist > Vector2.Distance(new Vector2(item.GetComponent<RectTransform>().localPosition.x + (Screen.width / 2), item.GetComponent<RectTransform>().localPosition.y + (Screen.height / 2)), (Vector2)Input.mousePosition))
                    {
                        dist = Vector2.Distance(new Vector2(item.GetComponent<RectTransform>().localPosition.x + (Screen.width / 2), item.GetComponent<RectTransform>().localPosition.y + (Screen.height / 2)), (Vector2)Input.mousePosition);
                        selected = item;
                        selectedNum = gameObjects.IndexOf(selected);
                    }
                    item.GetComponent<RectTransform>().localScale = new Vector3(0.6771638f, 0.6771638f, 0.6771638f);
                }
                if (dist > Vector2.Distance(new Vector2(dontPlayButton.GetComponent<RectTransform>().localPosition.x + (Screen.width / 2), dontPlayButton.GetComponent<RectTransform>().localPosition.y + (Screen.height / 2)), (Vector2)Input.mousePosition))
                {
                    dist = Vector2.Distance(new Vector2(dontPlayButton.GetComponent<RectTransform>().localPosition.x + (Screen.width / 2), dontPlayButton.GetComponent<RectTransform>().localPosition.y + (Screen.height / 2)), (Vector2)Input.mousePosition);
                    selected = dontPlayButton;
                    selectedNum = gameObjects.IndexOf(selected);
                }
                dontPlayButton.GetComponent<RectTransform>().localScale = new Vector3(0.6771638f, 0.6771638f, 0.6771638f);



                selected.GetComponent<RectTransform>().localScale = new Vector3(0.9771638f, 0.9771638f, 0.9771638f);
            }
            else
            {
                selected.GetComponent<RectTransform>().localScale = new Vector3(0.6771638f, 0.6771638f, 0.6771638f);
            }
        }
        //TODO joy is not an image anymore or something
        //switch (activePage)
        //{
        //    case 0:
        //        joy.color = Color.Lerp(joy.color, new Color(114f / 255f, 157f / 255f, 255f / 255f, .9f), Time.deltaTime * 4);
        //        break;
        //    case 1:
        //        joy.color = Color.Lerp(joy.color, new Color(114f / 255f, 255f / 255f, 157f / 255f, .9f), Time.deltaTime * 4);
        //        break;
        //    case 2:
        //        joy.color = Color.Lerp(joy.color, new Color(255f / 255f, 114f / 255f, 157f / 255f, .9f), Time.deltaTime * 4);
        //        break;
        //    default:
        //        break;
        //}
        //DebugClass.Log($"----------{activePage} ---  {joy.color} ---  {joy.sprite.name}");
    }
    private void SaveWheelFromGame(int currentPage, int currentSelected, string newEmoteName)
    {
        ButtonScript.SaveSettingFromGame(currentPage, currentSelected, newEmoteName);
    }

    private IEnumerator SwitchPage(string[] newPage, bool instant = true)
    {
        if (instant)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                //TODO TMPUGUI
                //gameObjects[i].GetComponentInChildren<TextMeshProUGUI>().text = newPage[i];
            }
            yield break;
        }
        bool stay = true;
        float longestLength = 0;
        while (stay)
        {
            stay = false;
            foreach (var item in gameObjects)
            {
                //TODO TMPUGUI
                //if (item.GetComponentInChildren<TextMeshProUGUI>().text.Length != 0)
                //{
                //    if (item.GetComponentInChildren<TextMeshProUGUI>().text.Length > longestLength)
                //        longestLength = item.GetComponentInChildren<TextMeshProUGUI>().text.Length;

                //    item.GetComponentInChildren<TextMeshProUGUI>().text = item.GetComponentInChildren<TextMeshProUGUI>().text.Remove(0, 1);
                //    stay = true;
                //}
            }
            if (longestLength != 0)
            {
                yield return new WaitForSeconds(.05f / longestLength);
            }
        }
        for (int i = 0; i < gameObjects.Count; i++)
        {
            //TODO TMPUGUI
            //gameObjects[i].GetComponentInChildren<TextMeshProUGUI>().text = newPage[i];
        }

        yield break;
    }

    void OnDestroy()
    {
        //TODO do we need to do this in lethal company?
        //if (CustomEmotesAPI.GetKey(Settings.EmoteWheel))
        //{
        //    //TODO gamepad support?
        //    //if (events.cursorOpenerForGamepadCount > 0)
        //    //{
        //    //    events.cursorOpenerForGamepadCount -= 1;
        //    //    events.cursorOpenerCount -= 1;
        //    //}
        //}
    }
}
