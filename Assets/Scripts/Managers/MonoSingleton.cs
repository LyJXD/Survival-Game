using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// �̳�MonoBehaviour�ĵ���ģʽ��ͨ�ø���
///<summary>
public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> // ��Լ��Ϊ T����Ϊ�䱾�������
{
    // ����˽�ж����¼ȡֵ����ֻ��ֵһ�α����θ�ֵ
    private static T instance; 

    public static T Instance
    {
        // ʵ�ְ������
        get
        {
            // ���Ѿ���ֵ����ֱ�ӷ��ؼ���
            if (instance != null)
            {
                return instance;
            }

            instance = FindObjectOfType<T>();

            // Ϊ�˷�ֹ�ű���δ�ҵ������ϣ��Ҳ������쳣������������д������������ȥ
            if (instance == null)
            {
                // ���������������ڴ���ʱ���������Ͻű���Awake������T��Awake(T��Awakeʵ�����Ǽ̳еĸ���ģ�
                // ���Դ�ʱ����Ϊinstance��ֵ�������Awake�и�ֵ����ȻҲ���ʼ����������init()
                new GameObject("Singleton of " + typeof(T)).AddComponent<T>();
            }
            else
            {
                instance.Init(); // ��֤Initִֻ��һ��
            }

            return instance;
        }
    }

    private void Awake()
    {
        // ���������ű���Awake�е��ô�ʵ���������Awake�����г�ʼ��instance
        instance = this as T;
        // ��ʼ��
        Init();
    }

    // ����Գ�Ա���г�ʼ���������Awake���Ի����Null����������������һ��init������������ÿɲ��ã�
    protected virtual void Init()
    {

    }
}
