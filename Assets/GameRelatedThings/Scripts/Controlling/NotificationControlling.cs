using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationControlling : MonoBehaviour
{
    public TMP_Text notif_text;

    private float timeRemaining = 90;
    private GameObject window;
    private Animator notif_animator;
    private Queue<string> notif_queue;
    private bool isOn;
    private Coroutine queue_checker;

    // Start is called before the first frame update
    private void Start()
    {
        window = transform.GetChild(0).gameObject;
        notif_animator = window.GetComponent<Animator>();
        window.SetActive(false);
        notif_queue = new Queue<string>();
    }
    // Update is called once per frame
    void Update()
    {
        if (timeRemaining > 0)
            timeRemaining -= Time.deltaTime;
        else
        {
            DisplayNotif(notif_text.text);
            timeRemaining = 90;
        }
    }
    public void Queue_Adding(string text)
    {
        notif_queue.Enqueue(text);
        if (queue_checker == null)
            queue_checker = StartCoroutine(QueueCheck());
    }
    private void DisplayNotif(string text)
    {
        window.SetActive(true);
        notif_text.text = text;
        notif_animator.Play("notif_animation");
    }
    private IEnumerator QueueCheck()
    {
        do
        {
            DisplayNotif(notif_queue.Dequeue());
            do
            {
                yield return null;
            } while (!notif_animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"));
        } while (notif_queue.Count > 0);
        window.SetActive(false);
        queue_checker = null;
    }
}
