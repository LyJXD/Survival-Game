using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// 继承MonoBehaviour的单例模式的通用父类
///<summary>
public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> // 此约束为 T必须为其本身或子类
{
    // 创建私有对象记录取值，可只赋值一次避免多次赋值
    private static T instance; 

    public static T Instance
    {
        // 实现按需加载
        get
        {
            // 当已经赋值，则直接返回即可
            if (instance != null)
            {
                return instance;
            }

            instance = FindObjectOfType<T>();

            // 为了防止脚本还未挂到物体上，找不到的异常情况，可以自行创建空物体挂上去
            if (instance == null)
            {
                // 如果创建对象，则会在创建时调用其身上脚本的Awake即调用T的Awake(T的Awake实际上是继承的父类的）
                // 所以此时无需为instance赋值，其会在Awake中赋值，自然也会初始化所以无需init()
                new GameObject("Singleton of " + typeof(T)).AddComponent<T>();
            }
            else
            {
                instance.Init(); // 保证Init只执行一次
            }

            return instance;
        }
    }

    private void Awake()
    {
        // 若无其它脚本在Awake中调用此实例，则可在Awake中自行初始化instance
        instance = this as T;
        // 初始化
        Init();
    }

    // 子类对成员进行初始化如果放在Awake里仍会出现Null问题所以自行制作一个init函数解决（可用可不用）
    protected virtual void Init()
    {

    }
}
