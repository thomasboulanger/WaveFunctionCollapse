using System;
using UnityEngine;
using NaughtyAttributes;

public class CircularStackTester : MonoBehaviour
{
    [SerializeField, ReadOnly] private CircularStack<string> _stack;

    [ShowNativeProperty] private Vector2Int BufferRange => 
        new Vector2Int(_stack.EndIndex, _stack.StartIndex);

    [ShowNativeProperty] private int Size =>
        _stack.Count;

    private void Awake()
    {
        _stack = new CircularStack<string>(5);
    }

    [SerializeField] private string _input;
    [SerializeField, ReadOnly] private string _lastPeek;
    [SerializeField, ReadOnly] private string _lastPop;

    [Button]
    private void Push() => _stack.Push(_input);

    [Button]
    private void Pop()
    {
        try { _lastPop = _stack.Pop(); }
        catch (Exception e) { Debug.LogWarning(e.Message); }
    }
    
    [Button]
    private void Peek()
    {
        try { _lastPeek = _stack.Peek(); }
        catch (Exception e) { Debug.LogWarning(e.Message); }
    }
}
