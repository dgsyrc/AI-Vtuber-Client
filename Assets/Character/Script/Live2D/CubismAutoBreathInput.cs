/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEngine;


namespace Live2D.Cubism.Framework
{
    /// <summary>
    /// Automatic mouth movement.
    /// </summary>
    public sealed class CubismAutoBreathInput : MonoBehaviour
    {
        /// <summary>
        /// Mean time between eye blinks in seconds.
        /// </summary>
        [SerializeField, Range(0f, 5f)]
        public float Mean = 1f;

        /// <summary>
        /// Maximum deviation from <see cref="Mean"/> in seconds.
        /// </summary>
        [SerializeField, Range(0f, 1f)]
        public float MaximumDeviation = 0f;

        /// <summary>
        /// Timescale.
        /// </summary>
        [SerializeField, Range(1f, 20f)]
        public float Timescale = 10f;

        /// <summary>
        /// Target controller.
        /// </summary>


        [SerializeField, Range(1f, 20f)]
        public float BreathTime = 12.5f;



        private CubismBreathController Controller { get; set; }

        /// <summary>
        /// Control over whether output should be evaluated.
        /// </summary>
        private Phase CurrentPhase { get; set; }

        /// <summary>
        /// Used for switching from <see cref="Phase.BreathUp"/> to <see cref="Phase.BreathDown"/> and back to <see cref="Phase.Idling"/>.
        /// </summary>
        private float LastValue { get; set; }

        /// <summary>
        /// Totalized delta time [s].
        /// </summary>
        private float UserTimeSeconds { get; set; }

        /// <summary>
        /// Time when the current state started [sec].
        /// </summary>
        private float StateStartTimeSeconds { get; set; }

        /// <summary>
        /// Duration of eyelid closing motion [sec]
        /// </summary>
        private float UpSeconds { get; set; }

        /// <summary>
        /// Duration of eyelid closed state [sec]
        /// </summary>
        private float UpedSeconds { get; set; }

        /// <summary>
        /// Duration of eyelid opening motion [sec]
        /// </summary>
        private float DownSeconds { get; set; }

        /// <summary>
        /// Next blinking time.
        /// </summary>
        private float NextBreathTime { get; set; }

        /// <summary>
        /// Resets the input.
        /// </summary>
        public void Reset()
        {
            CurrentPhase = Phase.Idling;
            NextBreathTime = Mean + Random.Range(-MaximumDeviation, MaximumDeviation);
        }

        /// <summary>
        /// Calculate the value when the eyes are closed.
        /// </summary>
        /// <returns>Eye closing value.</returns>
        private float UpdateBreathUp()
        {
            var t = (UserTimeSeconds - StateStartTimeSeconds) / UpSeconds;

            if (t >= 1.0f)
            {
                CurrentPhase = Phase.UpedBreath;
                StateStartTimeSeconds = UserTimeSeconds;
            }

            var value = t;

            return value;
        }

        /// <summary>
        /// Calculate the value when the eyes are closed.
        /// </summary>
        /// <returns>Eye closed value.</returns>
        private float UpdateBreathUped()
        {
            var t = (UserTimeSeconds - StateStartTimeSeconds) / UpedSeconds;

            if (t >= 1.0f)
            {
                CurrentPhase = Phase.BreathDown;
                StateStartTimeSeconds = UserTimeSeconds;
            }

            var value = 1.0f;

            return value;
        }

        /// <summary>
        /// Calculate the value when the eyes are opening.
        /// </summary>
        /// <returns>Eye opening value.</returns>
        private float UpdateBreathDown()
        {
            var t = (UserTimeSeconds - StateStartTimeSeconds) / DownSeconds;

            if (t >= 1.0f)
            {
                t = 1.0f;
                CurrentPhase = Phase.Idling;
                NextBreathTime = Mean + Random.Range(-MaximumDeviation, MaximumDeviation);
            }

            var value = 1.0f - t;

            return value;
        }

        /// <summary>
        /// Calculate the value when the eyes are opened.
        /// </summary>
        /// <returns>Eye opened value.</returns>
        private float UpdateBreathIdling()
        {
            NextBreathTime -= Time.deltaTime;

            if (NextBreathTime < 0.0f)
            {
                CurrentPhase = Phase.BreathUp;
                StateStartTimeSeconds = UserTimeSeconds;
            }

            var value = 0.0f;

            return value;
        }



        /// <summary>
        /// Update eye blink.
        /// </summary>
        private void UpdateBreath()
        {
            UserTimeSeconds += (Time.deltaTime * Timescale);
            var value = 0.0f;
            SetBreathSettings(BreathTime, 0.5f, BreathTime);
            switch (CurrentPhase)
            {
                case Phase.BreathUp:
                    {
                        value = UpdateBreathUp();
                        break;
                    }
                case Phase.UpedBreath:
                    {
                        value = UpdateBreathUped();
                        break;
                    }
                case Phase.BreathDown:
                    {
                        value = UpdateBreathDown();
                        break;
                    }
                case Phase.Idling:
                    {
                        value = UpdateBreathIdling();
                        break;
                    }
            }

            Controller.Breathing = value;
        }

        /// <summary>
        /// Set the details of the blinking motion.
        /// </summary>
        /// <param name="closing">Duration of eyelid closing motion [sec].</param>
        /// <param name="closed">Duration of eyelid closed state [sec].</param>
        /// <param name="opening">Duration of eyelid opening motion [sec].</param>
        public void SetBreathSettings(float closing, float closed, float opening)
        {
            UpSeconds = closing;
            UpedSeconds = closed;
            DownSeconds = opening;
        }

        #region Unity Event Handling

        /// <summary>
        /// Called by Unity. Initializes input.
        /// </summary>
        private void Start()
        {
            Controller = GetComponent<CubismBreathController>();
            CurrentPhase = Phase.Idling;
            NextBreathTime = Mean + Random.Range(-MaximumDeviation, MaximumDeviation);
            
        }


        /// <summary>
        /// Called by Unity. Updates controller.
        /// </summary>
        /// <remarks>
        /// Make sure this method is called after any animations are evaluated.
        /// </remarks>
        private void LateUpdate()
        {
            // Fail silently.
            if (Controller == null)
            {
                return;
            }

            UpdateBreath();
        }

        #endregion

        /// <summary>
        /// Internal states.
        /// </summary>
        private enum Phase
        {
            /// <summary>
            /// Idle state.
            /// </summary>
            Idling,

            /// <summary>
            /// State when closing eyes.
            /// </summary>
            BreathUp,

            /// <summary>
            /// State when closed eyes.
            /// </summary>
            UpedBreath,

            /// <summary>
            /// State when opening eyes.
            /// </summary>
            BreathDown
        }
    }
}
