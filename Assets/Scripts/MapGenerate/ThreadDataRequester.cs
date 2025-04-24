using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadDataRequester : MonoBehaviour
{
    static ThreadDataRequester instance;

    // ConcurrentQueue<T> 是一个线程安全的先进先出队列，属于 System.Collections.Concurrent 命名空间。
    // 它遵循先进先出的原则，允许多个线程同时对队列进行操作，而无需额外的锁机制。
    ConcurrentQueue<ThreadInfo> dataQueue = new();

    private void Awake()
    {
        instance = FindObjectOfType<ThreadDataRequester>();
    }

    public static void RequestData(Func<object> generateData, Action<object> callback)
    {
        /*
        // 匿名委托(ThreadStart是一个委托)
        ThreadStart threadStart = delegate
        {
            instance.DataThread(generateData, callback);
        };

        new Thread(threadStart).Start();
        */

        ThreadPool.QueueUserWorkItem(delegate
        {
            instance.DataThread(generateData, callback);
        });
    }

    private void DataThread(Func<object> generateData, Action<object> callback)
    {
        object data = generateData();

        dataQueue.Enqueue(new ThreadInfo(callback, data));
    }

    private void Update()
    {
        if (dataQueue.Count > 0)
        {
            for (int i = 0; i < dataQueue.Count; i++)
            {
                // 如果队列中有元素，TryDequeue会成功取出元素并将队列修改为相应的状态，返回true；如果队列为空，则返回false
                if (dataQueue.TryDequeue(out var result))
                {
                    ThreadInfo threadInfo = result;
                    threadInfo.Callback(threadInfo.Parameter);
                }
            }
        }
    }

    public readonly struct ThreadInfo
    {
        public Action<object> Callback { get; }
        public object Parameter { get; }

        public ThreadInfo(Action<object> callback, object parameter)
        {
            Callback = callback;
            Parameter = parameter;
        }
    }
}