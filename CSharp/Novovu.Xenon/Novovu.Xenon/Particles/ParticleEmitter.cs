using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Novovu.Xenon.Particles
{
    public class ParticleEmitter
    {
        CustomParticleSystem particleSystem;
        float timeBetweenParticles;
        Vector3 previousPosition;
        float timeLeftOver = 0;
        public ParticleEmitter(CustomParticleSystem particleSystemw,
                               float particlesPerSecond, Vector3 initialPosition)
        {
          
            this.particleSystem = particleSystemw;

            timeBetweenParticles = 1.0f / particlesPerSecond;

            previousPosition = initialPosition;
        }

        public Vector3 LocationOffset = new Vector3(0,0,0);

        public void Emit(Matrix a, Matrix b)
        {
            particleSystem.SetCamera(a, b);
            particleSystem.Draw();
        }
        public void Update(GameTime gameTime, Vector3 newPosition)
        {
            particleSystem.Update(gameTime);
            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            // Work out how much time has passed since the previous update.
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedTime > 0)
            {
                // Work out how fast we are moving.
                Vector3 velocity = (newPosition - previousPosition) / elapsedTime;

                // If we had any time left over that we didn't use during the
                // previous update, add that to the current elapsed time.
                float timeToSpend = timeLeftOver + elapsedTime;

                // Counter for looping over the time interval.
                float currentTime = -timeLeftOver;

                // Create particles as long as we have a big enough time interval.
                while (timeToSpend > timeBetweenParticles)
                {
                    currentTime += timeBetweenParticles;
                    timeToSpend -= timeBetweenParticles;

                    // Work out the optimal position for this particle. This will produce
                    // evenly spaced particles regardless of the object speed, particle
                    // creation frequency, or game update rate.
                    float mu = currentTime / elapsedTime;

                    Vector3 position = Vector3.Lerp(previousPosition, newPosition, mu);

                    // Create the particle.
                    particleSystem.AddParticle(position, velocity);
                }

                // Store any time we didn't use, so it can be part of the next update.
                timeLeftOver = timeToSpend;
            }

            previousPosition = newPosition;
        }

    }
}
