using EmotesAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollManager : MonoBehaviour
{
    // Start is called before the first frame update
    internal GameObject content;
    internal static GameObject buttonTemplate;
    internal static List<GameObject> buttons = new List<GameObject>();
    internal static List<string> emoteNames = new List<string>();
    internal GameObject emoteInQuestion;
    internal static List<GameObject> circularButtons = new List<GameObject>();
    GameObject wheels;
    void Start()
    {
        gameObject.transform.parent.Find("Wheels").gameObject.SetActive(true);

        content = gameObject.transform.Find("Scroll View").Find("Viewport").Find("Content").gameObject;
        //TODO stuff
        //foreach (var item in gameObject.transform.Find("InputField (TMP)").GetComponentsInChildren<TextMeshProUGUI>())
        //{
        //    item.font = Settings.NakedButton.GetComponentInChildren<TextMeshProUGUI>().font;
        //    item.fontMaterial = Settings.NakedButton.GetComponentInChildren<TextMeshProUGUI>().fontMaterial;
        //    item.fontSharedMaterial = Settings.NakedButton.GetComponentInChildren<TextMeshProUGUI>().fontSharedMaterial;
        //}


        //buttonTemplate = gameObject.transform.Find("Scroll View").Find("Viewport").Find("Content").Find("Button").gameObject;
        //var butt = buttonTemplate.AddComponent<ButtonScript>();
        //GameObject nut = GameObject.Instantiate(Settings.NakedButton);
        //nut.transform.SetParent(buttonTemplate.transform);
        //nut.transform.localPosition = new Vector3(-80, -20, 0);
        //nut.transform.localScale = new Vector3(.8f, .8f, .8f);
        //buttonTemplate.GetComponent<Image>().enabled = false;
        //buttonTemplate.GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
        //buttonTemplate.GetComponentInChildren<HGButton>().onClick.AddListener(butt.PickNewEmote);


        //nut = GameObject.Instantiate(Settings.NakedButton);
        //nut.transform.SetParent(gameObject.transform.Find("Finish"));
        //nut.transform.localPosition = new Vector3(-80, -20, 0);
        //nut.transform.localScale = new Vector3(.8f, .8f, .8f);
        //gameObject.transform.Find("Finish").GetComponent<Image>().enabled = false;
        //gameObject.transform.Find("Finish").GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
        //gameObject.transform.Find("Finish").GetComponentInChildren<HGButton>().onClick.AddListener(Cancel);
        //TMP_InputField field = gameObject.transform.Find("InputField (TMP)").gameObject.GetComponent<TMP_InputField>();
        //field.onValueChanged.AddListener(delegate { UpdateButtonVisibility(field.text); });
        //wheels = transform.parent.Find("Wheels").gameObject;
        //var basedonwhatbasedonthehardwareinside = wheels.transform.Find("Middle");
        //for (int i = 0; i < 8; i++)
        //{
        //    basedonwhatbasedonthehardwareinside.Find($"Button ({i})").GetComponent<Image>().enabled = false;
        //    basedonwhatbasedonthehardwareinside.Find($"Button ({i})").GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
        //    circularButtons.Add(basedonwhatbasedonthehardwareinside.Find($"Button ({i})").Find("NakedButton(Clone)").gameObject);
        //}
        //basedonwhatbasedonthehardwareinside = wheels.transform.Find("Left");
        //for (int i = 0; i < 8; i++)
        //{
        //    basedonwhatbasedonthehardwareinside.Find($"Button ({i})").GetComponent<Image>().enabled = false;
        //    basedonwhatbasedonthehardwareinside.Find($"Button ({i})").GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
        //    circularButtons.Add(basedonwhatbasedonthehardwareinside.Find($"Button ({i})").Find("NakedButton(Clone)").gameObject);
        //}
        //basedonwhatbasedonthehardwareinside = wheels.transform.Find("Right");
        //for (int i = 0; i < 8; i++)
        //{
        //    basedonwhatbasedonthehardwareinside.Find($"Button ({i})").GetComponent<Image>().enabled = false;
        //    basedonwhatbasedonthehardwareinside.Find($"Button ({i})").GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
        //    circularButtons.Add(basedonwhatbasedonthehardwareinside.Find($"Button ({i})").Find("NakedButton(Clone)").gameObject);

        //}
        //foreach (var item in circularButtons)
        //{
        //    item.GetComponent<HGButton>().onClick.AddListener(delegate { Activate(item); });
        //}
        //LoadSettings();



        //nut = GameObject.Instantiate(Settings.NakedButton);
        //nut.transform.SetParent(wheels.transform.Find("Button"));
        //nut.transform.localPosition = new Vector3(-80, -20, 0);
        //nut.transform.localScale = new Vector3(.8f, .8f, .8f);
        //var script = wheels.transform.Find("Button").gameObject.AddComponent<ButtonScript>();
        //wheels.transform.Find("Button").gameObject.GetComponentInChildren<HGButton>().onClick.AddListener(wheels.transform.Find("Button").gameObject.GetComponent<ButtonScript>().Finish);

        //wheels.transform.Find("Button").GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
        //wheels.transform.Find("Button").GetComponent<Image>().enabled = false;

        //gameObject.SetActive(false);
        //gameObject.transform.parent.gameObject.SetActive(false);
    }

    internal void LoadSettings()
    {
        //TODO stuff
        //ScrollManager.circularButtons[0].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote0.Value;
        //ScrollManager.circularButtons[1].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote1.Value;
        //ScrollManager.circularButtons[2].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote2.Value;
        //ScrollManager.circularButtons[3].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote3.Value;
        //ScrollManager.circularButtons[4].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote4.Value;
        //ScrollManager.circularButtons[5].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote5.Value;
        //ScrollManager.circularButtons[6].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote6.Value;
        //ScrollManager.circularButtons[7].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote7.Value;
        //ScrollManager.circularButtons[8].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote8.Value;
        //ScrollManager.circularButtons[9].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote9.Value;
        //ScrollManager.circularButtons[10].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote10.Value;
        //ScrollManager.circularButtons[11].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote11.Value;
        //ScrollManager.circularButtons[12].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote12.Value;
        //ScrollManager.circularButtons[13].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote13.Value;
        //ScrollManager.circularButtons[14].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote14.Value;
        //ScrollManager.circularButtons[15].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote15.Value;
        //ScrollManager.circularButtons[16].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote16.Value;
        //ScrollManager.circularButtons[17].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote17.Value;
        //ScrollManager.circularButtons[18].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote18.Value;
        //ScrollManager.circularButtons[19].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote19.Value;
        //ScrollManager.circularButtons[20].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote20.Value;
        //ScrollManager.circularButtons[21].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote21.Value;
        //ScrollManager.circularButtons[22].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote22.Value;
        //ScrollManager.circularButtons[23].GetComponentInChildren<HGTextMeshProUGUI>().text = Settings.emote23.Value;
    }
    static bool alreadySetup = false;
    public static void SetupButtons(List<string> names)
    {
        if (!buttonTemplate || alreadySetup)
        {
            return;
        }
        alreadySetup = true;
        emoteNames = new List<string>();
        emoteNames.Clear();
        foreach (var item in names)
        {
            emoteNames.Add(item);
        }
        buttons.Clear();
        foreach (var item in emoteNames)
        {
            GameObject cum = GameObject.Instantiate(buttonTemplate);
            cum.name = item;
            //TODO stuff
            //cum.GetComponentInChildren<HGTextMeshProUGUI>().text = item;
            cum.transform.SetParent(buttonTemplate.transform.parent);
            cum.transform.transform.localScale = Vector3.one;
            buttons.Add(cum);
        }
        foreach (var item in buttons)
        {
            //TODO stuff
            //item.GetComponentInChildren<HGButton>().onClick.AddListener(item.GetComponent<ButtonScript>().PickNewEmote);
        }
    }
    void OnEnable()
    {
        UpdateButtonVisibility("");
    }

    public void SetEmoteInQuestion(GameObject e)
    {
        emoteInQuestion = e;
    }
    public void Activate(GameObject button)
    {
        gameObject.transform.parent.Find("Wheels").gameObject.SetActive(false);
        emoteInQuestion = button;
        gameObject.SetActive(true);
    }

    public void Cancel()
    {
        gameObject.transform.parent.Find("Wheels").gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    static bool first = true;
    public void UpdateButtonVisibility(string filter)
    {
        if (first)
        {
            DebugClass.Log($"Wanna see a cool NRE?");
            first = false;
        }
        List<GameObject> validButtons = new List<GameObject>();
        foreach (var item in buttons)
        {
            //TODO stuff
            //if (item.GetComponentInChildren<HGTextMeshProUGUI>().text.ToUpper().Contains(filter.ToUpper()))
            //{
            //    validButtons.Add(item);
            //}
            //else
            //{
            //    item.SetActive(false);
            //}
        }
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, validButtons.Count * 10);
        int spot = 0;
        for (int i = 0; i < validButtons.Count; i++)
        {
            validButtons[i].SetActive(true);
            validButtons[i].GetComponent<RectTransform>().localScale = new Vector3(.5f, .5f, .5f);
            if (i % 2 == 0)
                validButtons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-43, -14f + (content.GetComponent<RectTransform>().sizeDelta.y * .5f) + (spot * -18));
            else
            {
                validButtons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(43, -14f + (content.GetComponent<RectTransform>().sizeDelta.y * .5f) + (spot * -18));
                spot++;
            }
            validButtons[i].GetComponent<ButtonScript>().emoteInQuestion = emoteInQuestion;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
    }
}
