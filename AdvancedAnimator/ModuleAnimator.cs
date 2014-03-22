using System;
using UnityEngine;

/* AdvancedAnimator was made by Christophe Savard (stupid_chris) and is licensed under CC-BY-SA. You are free to share and modify this code freely
 * under the attribution clause to me. You can contact me on the forums for more information. */

namespace AdvancedAnimator
{
    public class ModuleAnimator : PartModule
    {
        #region KSPFields
        [KSPField]
        public string animationName = string.Empty;
        [KSPField]
        public string guiEnableName = string.Empty;
        [KSPField]
        public string guiDisableName = string.Empty;
        [KSPField]
        public string guiToggleName = string.Empty;
        [KSPField]
        public string actionEnableName = string.Empty;
        [KSPField]
        public string actionDisableName = string.Empty;
        [KSPField]
        public string actionToggleName = string.Empty;
        [KSPField]
        public float animationSpeed = 1f;
        [KSPField]
        public bool oneShot = false;
        [KSPField]
        public bool activeEditor = false;
        [KSPField]
        public bool activeFlight = false;
        [KSPField]
        public bool activeUnfocused = false;
        [KSPField]
        public float unfocusedRange = 5f;
        [KSPField(isPersistant = true)]
        public bool enabled = false;
        #endregion

        #region Part GUI
        [KSPEvent(active = true, guiActive = true, guiActiveEditor = true, guiActiveUnfocused = true, guiName = "Toggle", unfocusedRange = 5)]
        public void GUIToggle()
        {
            if (this.enabled)
            {
                if (CheckAnimationPlaying()) { PlayAnimation(-this.animationSpeed, GetAnimationTime()); }
                else { PlayAnimation(-this.animationSpeed, 1); }
                this.enabled = false;
            }
            else
            {
                if (CheckAnimationPlaying()) { PlayAnimation(this.animationSpeed, GetAnimationTime()); }
                else { PlayAnimation(this.animationSpeed, 0); }
                this.enabled = true;
            }
        }
        #endregion

        #region Action Groups
        [KSPAction("Enable")]
        public void ActionEnable(KSPActionParam param)
        {
            if (CheckAnimationPlaying()) { PlayAnimation(-this.animationSpeed, GetAnimationTime()); }
            else { PlayAnimation(-this.animationSpeed, 1); }
            this.enabled = false;
        }

        [KSPAction("Disable")]
        public void ActionDisable(KSPActionParam param)
        {
            if (CheckAnimationPlaying()) { PlayAnimation(this.animationSpeed, GetAnimationTime()); }
            else { PlayAnimation(this.animationSpeed, 1); }
            this.enabled = true;
        }

        [KSPAction("Toggle")]
        public void ActionToggle(KSPActionParam param)
        {
            GUIToggle();
        }
        #endregion

        #region Methods
        public void InitiateAnimation()
        {
            foreach (Animation animation in this.part.FindModelAnimators(this.animationName))
            {
                AnimationState state = animation[this.animationName];
                state.normalizedTime = this.enabled ? 1 : 0;
                state.normalizedSpeed = 0;
                state.enabled = false;
                state.wrapMode = WrapMode.Clamp;
                state.layer = 1;
                animation.Play(this.animationName);
            }
        }

        public void PlayAnimation(float animationSpeed, float animationTime)
        {
            //Plays the animation
            foreach (Animation animation in this.part.FindModelAnimators(this.animationName))
            {
                AnimationState state = animation[this.animationName];
                state.normalizedTime = animationTime;
                state.normalizedSpeed = animationSpeed;
                state.enabled = true;
                state.wrapMode = WrapMode.Clamp;
                animation.Play(this.animationName);
            }
        }

        public bool CheckAnimationPlaying()
        {
            //Checks if a given animation is playing
            foreach (Animation animation in this.part.FindModelAnimators(this.animationName))
            {
                return animation.IsPlaying(this.animationName);
            }
            return false;
        }

        public float GetAnimationTime()
        {
            foreach (Animation animation in this.part.FindModelAnimators(this.animationName))
            {
                return animation[this.animationName].normalizedTime;
            }
            return 0f;
        }
        #endregion

        #region Overrides
        public override void OnStart(PartModule.StartState state)
        {
            if (!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor) { return; }

            //In case of errors
            if (string.IsNullOrEmpty(this.animationName) || this.part.FindModelAnimators(this.animationName).Length <= 0)
            {
                Events.ForEach(e => e.active = false);
                Actions.ForEach(a => a.active = false);
                return;
            }

            else
            {
                foreach (BaseEvent e in Events)
                {
                    e.guiActiveEditor = this.activeEditor;
                    e.guiActive = this.activeFlight;
                    e.guiActiveUnfocused = this.activeUnfocused;
                    e.unfocusedRange = this.unfocusedRange;
                }
            }

            //Initiates the animation
            InitiateAnimation();

            //Sets the action groups/part GUI
            BaseEvent enable = Events["GUIEnable"], disable = Events["GUIDisable"], toggle = Events["GUIToggle"];
            BaseAction aEnable = Actions["ActionEnable"], aDisable = Actions["ActionDisable"], aToggle = Actions["ActionToggle"];

            if (string.IsNullOrEmpty(this.guiEnableName)) { enable.active = false; }
            else { enable.guiName = this.guiEnableName; }

            if (string.IsNullOrEmpty(this.guiDisableName)) { disable.active = false; }
            else { disable.guiName = this.guiDisableName; }

            if (string.IsNullOrEmpty(this.guiToggleName)) { toggle.active = false; }
            else { toggle.guiName = this.guiToggleName; }

            if (string.IsNullOrEmpty(this.actionEnableName)) { aEnable.active = false; }
            else { aEnable.guiName = this.actionEnableName; }

            if (string.IsNullOrEmpty(this.actionDisableName)) { aDisable.active = false; }
            else { aDisable.guiName = this.actionDisableName; }

            if (string.IsNullOrEmpty(this.actionToggleName)) { aToggle.active = false; }
            else { aToggle.guiName = this.actionToggleName; }
        }
        #endregion
    }
}