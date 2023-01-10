using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
            Instance = this;
        }
    }

    [ReadOnly, Header("Score"), SerializeField] private int _score;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Transform _scoreUI;
    [SerializeField, Header("Life")] private Image _lifeRange;
    [SerializeField, Header("Bomb")] private Transform _bombUI;
    [SerializeField] private TextMeshProUGUI _numberOfBombText;
    [SerializeField] private Image _circularReload;
    [SerializeField, Space(10), Header("Exit"),ReadOnly] public TileExit TileExit;


    private void Start()
    {
        _scoreText.text = _score.ToString();
        _lifeRange.fillAmount = 1;
        _circularReload.fillAmount = 1;
        _numberOfBombText.text = "1";
    }

    public void ChangeScore(int value)
    {
        _score += value;
        _scoreText.text = _score.ToString();
        
        //anim
        _scoreUI.DOComplete();
        _scoreUI.DOPunchScale(Vector3.one * 0.4f, 0.4f);
    }

    public void ChangeLife(float fillAmount)
    {
        _lifeRange.DOFillAmount(fillAmount, 0.5f);
        _lifeRange.transform.parent.DOComplete();
        _lifeRange.transform.parent.DOPunchScale(Vector3.one * 0.25f, 0.3f);
    }

    public void BombUIReload(float reloadFill)
    {
        _circularReload.fillAmount = reloadFill;
    }

    public void ChangeBombNumber(int number)
    {
        _bombUI.DOComplete();
        _bombUI.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        _numberOfBombText.text = number.ToString();
    }
    
    public void CheckExit()
    {
        if (MapManager.Instance.EnemyList.Count <= 0)
        {
            TileExit.Exit();
        }
    }

    public void Win()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
