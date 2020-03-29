using System;
using UnityEngine;

namespace Engarde_Teo
{

    [Serializable]
    public class SimpleTimer
    {

        [SerializeField] private float duration;
        [SerializeField] private float current;

        // Cannot use auto property because PropertyDrawer depends on existence of physical variable.
        public float Current { get => current; set => current = value; }
        public float Duration { get => duration; set => duration = value; }

        public bool Done => current <= 0;
        public bool Running => current > 0;
        public float Progress => 1 - Mathf.Clamp01(current / duration);

        public SimpleTimer(float duration = 0)
        {
            Duration = duration;
            Current = Duration;
        }

        public SimpleTimer(SimpleTimer other) :
            this(other.duration)
        {
        }

        /// <summary>
        /// Reset the time remaining back to full.
        /// </summary>
        public void Start()
        {
            Current = Duration;
        }

        /// <summary>
        /// Immediately mark the timer as complete.
        /// </summary>
        public void Stop()
        {
            Current = 0;
        }

        /// <summary>
        /// Updates the amount of time remaining in the timer. Call repeatedly once every update period.
        /// </summary>
        public void Update(bool fixedUpdate = false)
        {
            Update(fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime);
        }

        /// <summary>
        /// Updates the amount of time remaining in the timer. Call repeatedly once every update period.
        /// </summary>
        public void Update(float delta)
        {
            if (current >= 0) current -= delta;
        }

    }

}