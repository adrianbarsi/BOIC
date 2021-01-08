using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOIC.Code.Utility
{
    public class CooldownTimer
    {
        private bool onCooldown = false;
        public bool OnCooldown { get => onCooldown; }
        private bool internalOnCooldown = false;
        private float currentTime = 0;
        private float timeToFinish = 0;

        private void start(float currentTime, float coolDownDuration)
        {
            if (onCooldown && !internalOnCooldown)
            {
                this.currentTime = currentTime;
                timeToFinish = currentTime + coolDownDuration;
                internalOnCooldown = true;
            }
        }

        private void update(float timePassed)
        {
            if (onCooldown && internalOnCooldown)
            {
                currentTime += timePassed;
                if (currentTime > timeToFinish)
                {
                    onCooldown = false;
                    internalOnCooldown = false;
                }
            }
        }

        public void startCooldown()
        {
            if(!onCooldown)
            {
                onCooldown = true;
            }
        }

        public static void updateCooldown(CooldownTimer cooldownTimer, float cooldownTime, float elapsedTime)
        {
            if (cooldownTimer.onCooldown && !cooldownTimer.internalOnCooldown)
            {
                cooldownTimer.start(0, cooldownTime);
            }

            if (cooldownTimer.internalOnCooldown)
            {
                cooldownTimer.update(elapsedTime);
            }
        }
    }
}
