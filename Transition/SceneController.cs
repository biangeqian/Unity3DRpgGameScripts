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
        InventoryManager.Instance.SaveData();
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
        if (scene != "")
        {
            SceneFader fade = Instantiate(sceneFaderPrefeb);
            yield return StartCoroutine(fade.FadeOut(2f));
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefeb, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);
            //保存
            SaveManager.Instance.SavePlayerData();
            InventoryManager.Instance.SaveData();
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
        StartCoroutine(Resurrection());
    }
    IEnumerator Resurrection()
    {
        player = GameManager.Instance.playerStats.gameObject;
        player.GetComponent<CharacterController>().enabled = false;
        yield return new WaitForSeconds(2.5f);  
        //角色死亡传送回出生点
        //用move会被路径上的东西卡主,传送最好还是重新生成或者取消碰撞体或者setposition     
        SceneFader fade = Instantiate(sceneFaderPrefeb);
        yield return StartCoroutine(fade.FadeOut(3f));
        player.GetComponent<Transform>().SetPositionAndRotation(GetDestination(TransitionDestination.DestinationTag.ENTER).transform.position, transform.rotation);
        //player.GetComponent<CharacterController>().Move(GetDestination(TransitionDestination.DestinationTag.ENTER).transform.position - player.transform.position);
        //player.GetComponent<CharacterController>().Move(Vector3.zero);
        player.GetComponent<CharacterController>().enabled = true;
        GameManager.Instance.playerStats.CurrentHealth = GameManager.Instance.playerStats.MaxHealth;
        GameManager.Instance.PlayAgainObserver();
        yield return StartCoroutine(fade.FadeIn(3f));
        SaveManager.Instance.SavePlayerData();
        yield break;
    }

    public void PlayAgain()
    {
        GameManager.Instance.playerStats.gameObject.GetComponent<PlayerControllerWASD>().ObserverFlag = false;
    }
}
