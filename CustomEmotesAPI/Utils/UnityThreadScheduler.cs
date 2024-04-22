using System;
using System.Collections;
using System.Collections.Concurrent;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LethalEmotesAPI.Utils;

internal static class UnityThreadScheduler
{
    private static GameObject _instance;
    
    private static readonly ConcurrentQueue<Action> UnityWorkQueue = new();
    
    public static void Init()
    {
        _instance = new GameObject("LethalEmotesAPI ThreadWorker");
        Object.DontDestroyOnLoad(_instance);
        _instance.hideFlags = HideFlags.DontSave;

        _instance.AddComponent<LethalEmotesApiThreadWorker>();
    }

    /// <summary>
    /// Enqueues work to be run on the main Unity thread.
    /// <remarks>Thread safe</remarks>
    /// </summary>
    public static void EnqueueWorkOnUnityThread(Action action) => UnityWorkQueue.Enqueue(action);

    private class LethalEmotesApiThreadWorker : MonoBehaviour
    {
        [CanBeNull] private IEnumerator _processWorkQueueCoroutine;

        private bool _working;
        
        private void Update()
        {
            if (UnityWorkQueue.IsEmpty)
                return;

            if (_working)
                return;
            
            StartProcessWorkQueue();
        }

        private void StartProcessWorkQueue()
        {
            if (_working)
                return;

            _working = true;
            
            if (_processWorkQueueCoroutine is not null)
            {
                StopCoroutine(_processWorkQueueCoroutine);
                _processWorkQueueCoroutine = null;
            }

            _processWorkQueueCoroutine = ProcessWorkQueue();
            StartCoroutine(_processWorkQueueCoroutine);
        }

        private IEnumerator ProcessWorkQueue()
        {
            while (UnityWorkQueue.TryDequeue(out var action))
            {
                action.Invoke();
                yield return null;
            }
            
            _working = false;
        }
    }
}