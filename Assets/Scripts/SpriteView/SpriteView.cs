using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteView : MonoBehaviour
{
    
    #region Serialize Fields

    [Header("States"), SerializeField] 
    private string _defaultStateName;

    [SerializeField] 
    private List<State> _statesList;

    [Header("Action"), SerializeField] 
    private List<State> _actionsList;

    [HideInInspector] public UnityEvent OnActionEnd = new UnityEvent();
    [HideInInspector] public UnityEvent OnStateEnd = new UnityEvent();
    
    #endregion

    #region Private values

    //sprite
    private SpriteRenderer _spriteRenderer;
    private float _changeCountdown;
    private int _currentSprite;
    //state
    [HideInInspector] public State State;
    private int _currentState;
    //actions
    private bool _isPlayingAction;
    //dictionaries
    private Dictionary<string, State> _stateDictionary = new();
    private Dictionary<string, State> _actionDictionary = new();

    #endregion
    
    
    private void Awake()
    {
        //state
        State = _statesList.First(x => x.Name == _defaultStateName);
        _currentState = _statesList.IndexOf(State);

        //sprite
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _changeCountdown = State.TimeBetweenFrames;
        ResetAnimation();
        
        //dictionary
        foreach (State state in _statesList)
        {
            _stateDictionary.Add(state.Name, state);
        }
        foreach (State action in _actionsList)
        {
            _actionDictionary.Add(action.Name, action);
        }
    }

    private void Update()
    {
            Animate();
    }

    #region Animation methods

    private void Animate()
    {
        _changeCountdown -= Time.deltaTime;

        if (_changeCountdown <= 0)
        {
            _changeCountdown = State.TimeBetweenFrames;

            _currentSprite++;
            if (_currentSprite >= State.SpriteSheet.Count)
            {
                _currentSprite = 0;
                
                //if action -> reset to current state
                if (_isPlayingAction)
                {
                    _isPlayingAction = false;
                    OnActionEnd.Invoke();
                    State = _statesList[_currentState];
                    _spriteRenderer.sprite = State.SpriteSheet[_currentSprite];
                }
                else
                {
                    OnStateEnd.Invoke();
                }
            }

            if (State.SpriteSheet.Count > 0)
            {
                _spriteRenderer.sprite = State.SpriteSheet[_currentSprite];
            }
        }
    }
    
    private void ResetAnimation()
    {
        if (State.SpriteSheet.Count > 0)
        {
            _spriteRenderer.sprite = State.SpriteSheet[0];
        }
        _currentSprite = 0;
    }

    #endregion

    #region Play State/Action public methods

    public void PlayState(string state)
    {
        if (State.Name == state || _isPlayingAction)
        {
            return;
        }

        State = _stateDictionary[state];
        _currentState = _stateDictionary.Values.ToList().IndexOf(State);
        ResetAnimation();
    }

    public void PlayAction(string state)
    {
        _isPlayingAction = true;
        State = _actionDictionary[state];
        ResetAnimation();
    }

    #endregion
    
    
}
