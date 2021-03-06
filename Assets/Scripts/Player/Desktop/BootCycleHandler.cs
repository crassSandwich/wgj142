﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WitchOS
{
    public class BootCycleHandler : MonoBehaviour
    {
        public Animator Animator;
        public Image Image;
        public string AnimatorOnStateBool, AnimatorTrigger, IdleStateName, ShutDownStateName;
        
        public TimeState TimeState;

        int idleAnimationNameHash, shutDownAnimationNameHash;

        void Awake ()
        {
            idleAnimationNameHash = Animator.StringToHash(IdleStateName);
            shutDownAnimationNameHash = Animator.StringToHash(ShutDownStateName);

            TimeState.DayStarted.AddListener(startUp);
            TimeState.DayEnded.AddListener(shutDown);
        }

        void startUp ()
        {
            StartCoroutine(startUpRoutine());
        }

        void shutDown ()
        {
            StartCoroutine(shutDownRoutine());
        }

        IEnumerator shutDownRoutine ()
        {
            Image.raycastTarget = true;

            Animator.SetBool(AnimatorOnStateBool, false);
            Animator.SetTrigger(AnimatorTrigger);

            yield return new WaitUntil(() => Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == shutDownAnimationNameHash);
            yield return new WaitUntil(() => Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == idleAnimationNameHash);

            TimeState.StartNewDay();
        }

        IEnumerator startUpRoutine ()
        {
            Animator.SetBool(AnimatorOnStateBool, true);
            Animator.SetTrigger(AnimatorTrigger);

            yield return new WaitUntil(() => Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == idleAnimationNameHash);

            Image.raycastTarget = false;
        }
    }
}
