using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Core.Pathfinding;
using Core.Utilities;
using Core.Combat;
using Core.Jext;
using Core.Filler;

public class GameManager : PersistentSingleton<GameManager>
{
    // This is being used to determine turn order. Combatants will automatically be added and removed from this object.
    public CombatManager<Combatant> CombatManager { get; private set; }
    // This is being used to get paths between positions
    public Pathfinding Pathfinding { get; private set; }
    // This is the current level
    public Node[,] Grid { get; private set; }

    // This is used to determine cache size
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
    // This is all the filler in the current level. Filler will be automatically added and removed.
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

    /// <summary>
    /// Set level data, initialize pathfinding and node cache.
    /// </summary>
    /// <param name="grid"></param>
    public void SetupLevel(Node[,] grid)
    {
        Grid = grid;
        Pathfinding = new Pathfinding(grid);
        nodeCache = new List<Node>(grid.GetLength(0) * grid.GetLength(1));
    }

    private void OnLevelWasLoaded(int level)
    {
        // Reset objects
        CombatManager.Clear();
        filler.Clear();

        // Update UI
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

    /// <summary>
    /// Called from a button after a level is loaded
    /// </summary>
    public void AllowSceneActivation()
    {
        asyncLoad.allowSceneActivation = true;
        loadScreen.gameObject.SetActive(false);
    }
    #endregion

    // Not the perfect place for this function, but did it for the cache. Might make a seperate script for this
    
    /// <summary>
    /// Used for objects to check if an object can see amother object
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public bool CanSee(Vector2Int from, Vector2Int to)
    {
        nodeCache.Clear();
        Grid.GetLine(nodeCache, from, to);
        nodeCache.RemoveAt(0);
        nodeCache.RemoveAt(nodeCache.Count - 1);
        return !nodeCache.IsLineInterrupted();
    }

    /// <summary>
    /// Use A* to get a path to target destination
    /// </summary>
    /// <param name="combatant"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public List<Node> GetPath(Combatant combatant, Vector2Int to)
    {
        nodeCache.Clear();
        Pathfinding.GetPath(combatant, to, nodeCache);
        nodeCache.RemoveAt(nodeCache.Count - 1);
        return nodeCache;
    }
}