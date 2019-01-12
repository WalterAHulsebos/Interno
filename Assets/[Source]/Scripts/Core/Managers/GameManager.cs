using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.Pathfinding;
using Core.Utilities;
using Combat;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : PersistentSingleton<GameManager>
{
    public CombatManager CombatManager { get; private set; }
    public Pathfinding Pathfinding { get; private set; }
    public Node[,] Grid { get; private set; }

    [SerializeField]
    private int maxCombatants;

    #region Loading Level
    [SerializeField]
    private RectTransform loadScreen;
    [SerializeField]
    private Image loadBar;
    [SerializeField]
    private Button startButton;
    private AsyncOperation asyncLoad;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        CombatManager = new CombatManager(maxCombatants, false);
    }

    public void SetupLevel(Node[,] grid)
    {
        Grid = grid;
        Pathfinding = new Pathfinding(grid);
    }

    private void OnLevelWasLoaded(int level)
    {
        CombatManager.Clear();
    }

    #region Load Level
    public void LoadLevel(int index)
    {
        StartCoroutine(LoadLevelAsync(index));
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
}