using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadDataRequester : MonoBehaviour
{
    static ThreadDataRequester instance;

    // ConcurrentQueue<T> ��һ���̰߳�ȫ���Ƚ��ȳ����У����� System.Collections.Concurrent �����ռ䡣
    // ����ѭ�Ƚ��ȳ���ԭ���������߳�ͬʱ�Զ��н��в��������������������ơ�
    ConcurrentQueue<ThreadInfo> dataQueue = new();

    private void Awake()
    {
        instance = FindObjectOfType<ThreadDataRequester>();
    }

    public static void RequestData(Func<object> generateData, Action<object> callback)
    {
        /*
        // ����ί��(ThreadStart��һ��ί��)
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
                // �����������Ԫ�أ�TryDequeue��ɹ�ȡ��Ԫ�ز��������޸�Ϊ��Ӧ��״̬������true���������Ϊ�գ��򷵻�false
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