using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.Pathfinding;
using Core.Utilities;
using Combat;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Jext;
using System;

public class GameManager : PersistentSingleton<GameManager>
{
    public CombatManager<Combatant> CombatManager { get; private set; }
    public Pathfinding Pathfinding { get; private set; }
    public Node[,] Grid { get; private set; }

    [SerializeField]
    private int maxCombatants = 0, maxFiller = 0;

    #region Loading Level
    [SerializeField]
    private RectTransform loadScreen = null, mainMenu = null, ingameMenu = null;
    [SerializeField]
    private Image loadBar = null;
    [SerializeField]
    private Button startButton = null;
    private AsyncOperation asyncLoad;
    #endregion

    #region Level Data
    [NonSerialized]
    public List<Filler> filler;
    #endregion

    private List<Node> nodeCache;

    protected override void Awake()
    {
        base.Awake();
        CombatManager = new CombatManager<Combatant>(maxCombatants, false);
        filler = new List<Filler>(maxFiller);
    }

    public void SetupLevel(Node[,] grid)
    {
        Grid = grid;
        Pathfinding = new Pathfinding(grid);
        nodeCache = new List<Node>(grid.GetLength(0) * grid.GetLength(1));
    }

    private void OnLevelWasLoaded(int level)
    {
        CombatManager.Clear();
        filler.Clear();

        bool b = SceneManager.GetActiveScene().name.Contains("Level_");
        mainMenu.gameObject.SetActive(!b);
        ingameMenu.gameObject.SetActive(b);
    }

    #region Load Level
    public void LoadLevel(int index)
    {
        StartCoroutine(LoadLevelAsync(index));
    }

    public void ReloadLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator LoadLevelAsync(int index)
    {
        asyncLoad = SceneManager.LoadSceneAsync(string.Format("{0}{1}", "Level_", index));
        asyncLoad.allowSceneActivation = false;
        loadScreen.gameObject.SetActive(true);
        startButton.gameObject.SetActive(false);

        while (!asyncLoad.isDone)
        {
            loadBar.fillAmount = asyncLoad.progress + .1f;
            yield return null;
        }

        startButton.gameObject.SetActive(true);
    }

    public void AllowSceneActivation()
    {
        asyncLoad.allowSceneActivation = true;
        loadScreen.gameObject.SetActive(false);
    }
    #endregion

    // Not the perfect place for this function, but did it for the cache. Might make a seperate script for this
    
    public bool CanSee(Vector2Int from, Vector2Int to)
    {
        nodeCache.Clear();
        Grid.GetLine(nodeCache, from, to);
        nodeCache.RemoveAt(0);
        return !nodeCache.IsLineInterrupted();
    }

    public List<Node> GetPath(Combatant combatant, Vector2Int to)
    {
        nodeCache.Clear();
        Pathfinding.Get(combatant, to, nodeCache);
        nodeCache.RemoveAt(nodeCache.Count - 1);
        return nodeCache;
    }
}