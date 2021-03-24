using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    public GameObject playerPrefeb;
    public SceneFader sceneFaderPrefeb;
    GameObject player;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        GameManager.Instance.AddObserver(this);
    }
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));               
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }
    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
    {
        //保存数据
        SaveManager.Instance.SavePlayerData();
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            //异步加载
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefeb,GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            //读取数据(这里实际上和人物自己脚本上的读取重复了,不过问题不大)
            SaveManager.Instance.LoadPlayerData();
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            player.GetComponent<CharacterController>().Move(GetDestination(destinationTag).transform.position - player.transform.position);
            player.GetComponent<CharacterController>().Move(Vector3.zero);
            yield return null;
        }  
    }
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        for(int i=0;i<entrances.Length;i++)
        {
            if (entrances[i].destinationTag == destinationTag)
            {
                return entrances[i];
            }
        }
        return null;
    }
    public void TransitionToMenu()
    {
        StartCoroutine(LoadMenu());
    }
    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("SampleScene"));
    }
    IEnumerator LoadLevel(string scene)
    {
        SceneFader fade = Instantiate(sceneFaderPrefeb);
        if (scene != "")
        {
            yield return StartCoroutine(fade.FadeOut(2f));
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefeb, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);
            //保存
            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fade.FadeIn(2f));
            yield break;
        }  
    }
    IEnumerator LoadMenu()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            SaveManager.Instance.SavePlayerData();
            yield return SceneManager.LoadSceneAsync("Menu");
            yield break;
        }
        yield break;
    }

    public void EndNotify()
    {
        //角色死亡传送回出生点,复活
        //TODO
    }
}
