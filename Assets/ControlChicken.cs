using Microsoft.Mixer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControlChicken : MonoBehaviour
{
    public float speed;
    public Rigidbody rb;
    private float distToGround;
    // Start is called before the first frame update
    void Start()
    {
        MixerInteractive.GoInteractive();
        MixerInteractive.OnParticipantStateChanged += ParticipantStateChanged;
        MixerInteractive.OnError += MixerError;
        MixerInteractive.OnGoInteractive += OnInteractive;
        MixerInteractive.OnInteractiveButtonEvent += ButtonEvent;

        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.05f);
    }

void ButtonEvent(object sender, InteractiveButtonEventArgs e)
    {
        if (!e.IsPressed) return; //Only handle button downpresses
        switch (e.ControlID)
        {
            case "Progress":
                InteractiveButtonControl button = InteractivityManager.SingletonInstance.GetButton(e.ControlID);
                float progress = button.Progress;// Get current button progress
                float newprogress = progress + 0.1f;
                button.SetProgress(newprogress); //Increase button progress by 10%

                if (newprogress >= 1f)
                {
                    button.SetDisabled(true); //Disable button when reaching 100%
                    return;
                }
                

                button.TriggerCooldown(1000); //Trigger 1 second cooldown
                break;
            case "AttackScene":
                e.Participant.Group = MixerInteractive.GetGroup("attacking"); //Set participant's group to change MixPlay scene
                break;
            case "Jump":
                if (IsGrounded())
                {
                    rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
                    setAnimationState("Run");
                }
                break;
        }

        e.CaptureTransaction(); //Capture the spark transaction
    }
    void ParticipantStateChanged(object sender, InteractiveParticipantStateChangedEventArgs e)
    {
        InteractiveParticipant participant = e.Participant;
        if (e.State == InteractiveParticipantState.Joined) print(participant.UserName+" joined");
        if (e.State == InteractiveParticipantState.Left) print(participant.UserName + " left");
        if (e.State == InteractiveParticipantState.InputDisabled) print(participant.UserName + " input disabled");
    }

    void MixerError(object sender, InteractiveEventArgs e)
    {
        Debug.LogError(e.ErrorCode + " " + e.ErrorMessage);
    }

    void OnInteractive(object sender, InteractiveEventArgs e)
    {
        Debug.Log("Game is now interactive");
    }

    // Update is called once per frame
    void Update()
    {
        int participantCount = MixerInteractive.Participants.Count;
        double x = InteractivityManager.SingletonInstance.GetJoystick("move").X * participantCount;
        double y = InteractivityManager.SingletonInstance.GetJoystick("move").Y * participantCount;
 
        if(x==0 && y == 0)
        {
            if(IsGrounded()) setAnimationState("");
        }

        if (x!=0 || y != 0)
        {
            Vector3 tempVect = new Vector3((float)x * speed, rb.velocity.y, (float)y * speed);
            tempVect = tempVect.normalized * speed * Time.deltaTime;
            rb.MovePosition(transform.position + tempVect);
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(new Vector3((float)x * speed, 0, (float)y * speed)), 1000 * Time.deltaTime);

            if (x > 0.5d || y > 0.5d || x < -0.5d || y < -0.5d)
            {
                setAnimationState("Run");
            } else
            {
                setAnimationState("Walk");
            }
        }

        if (!IsGrounded()) setAnimationState("Run"); //Flap wings while falling

    }

    void setAnimationState(string animation)
    {
        Animator animator = GetComponent<Animator>();
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            animator.SetBool(parameter.name, false);
        }

        if (animation == "") return;
        animator.SetBool(animation, true);
    }
}
