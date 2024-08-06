using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MsgBox : MonoBehaviour
{
    [SerializeField] Text msgTxt;
    [SerializeField] RectTransform tf;
    public static MsgBox Instance;
    

    public Queue<MsgClip> msgQueue = new Queue<MsgClip>();

    bool free = true;
    bool cancel = false;
    Coroutine anim = null;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        tf.localPosition = new Vector3(tf.rect.width, 0, 0);
    }

    IEnumerator putMsg(MsgClip msg)
    {
        free = false;
        msgTxt.text = msg.msgText;
        float w = tf.rect.width;
        float t = 0.3f;
        float timer = 0f;

        while (timer < t)
        {
            timer += Time.unscaledDeltaTime;
            tf.localPosition = new Vector3(w * (1f - (timer / t)), 0, 0);
            yield return new WaitForNextFrameUnit();
        }

        yield return new WaitForSecondsRealtime(msg.time);

        t = 0.1f;
        timer = 0f;
        while (timer < t)
        {
            timer += Time.unscaledDeltaTime;
            tf.localPosition = new Vector3(w * (timer / t), 0, 0);
            yield return new WaitForNextFrameUnit();
        }
        free = true;
        yield return null;
    }

    public void NextMsgImmediately()
    {
        if(anim != null)
        {
            StopCoroutine(anim);
            tf.localPosition = new Vector3(tf.rect.width, 0, 0);
            free = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (free && msgQueue.Count > 0)
        {
            if (anim != null) StopCoroutine(anim);
            MsgClip m = msgQueue.Dequeue();
            anim = StartCoroutine(putMsg(m));
        }
    }

    public void PushMsg(string msg, float time)
    {
        msgQueue.Enqueue(new MsgClip(msg, time));
    }



    public class MsgClip{
        public string msgText;
        public float time;
        public MsgClip(string msg, float wait)
        {
            msgText = msg;
            time = wait;
        }
    }


}
