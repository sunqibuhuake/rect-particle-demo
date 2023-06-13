using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RectUIParticle
{

    // define motion type enums
    public enum MotionTypes
    {
        FadeIn,
        FadeOut,
        FadeFar,
        Floating,
        Init
    }

    // define particle init options
    public class ParticleInitOptions
    {
        public string name = "";
        public GameObject particlePrefab;
        public Color color;
        public Vector3 spawnPosition;
        public Vector3 rectPosition;
        public Vector3 sinkPosition;
        public float randomOffset;
        public float speed;
        public Vector3 amplitude;
        public float frequency;
        public float fadeInDuration;
        public float fadeOutDuration;
        public float sinkDuration;
        public string edgeName;

        public Vector3 size;
    }

    public class Particle
    {
        public GameObject gameObject;
        public Vector3 spawnPosition;
        public Vector3 sinkPosition;
        public Vector3 rectPosition;
        private Vector3 motionDelta;
        private Vector3 currentPosition;
        private float currentAlpha = 0f;
        private Vector3 currentScale = Vector3.zero;
        public float randomOffset;
        public float speed;
        public Vector3 amplitude;
        public float frequency;
        // public int edgeIndex;
        public string edgeName;
        private MotionTypes motionType = MotionTypes.Init;
        private float ease = 0f;


        private float fadeInDuration = 3.0f;
        private float fadeOutDuration = 3.0f;


        private float floatingDuration = 0f;
        private float animaDuration = 0f;


        public Coroutine particleContinue = null;


        public ParticleInitOptions options;

        public void Init(ParticleInitOptions initOptions)
        {
            options = initOptions;
            gameObject = GameObject.Instantiate(options.particlePrefab);
            gameObject.transform.position = options.spawnPosition;
            spawnPosition = options.spawnPosition;
            rectPosition = options.rectPosition;
            sinkPosition = options.sinkPosition;
            randomOffset = options.randomOffset;
            speed = options.speed;
            amplitude = options.amplitude;
            frequency = options.frequency;
            fadeInDuration = options.fadeInDuration;
            fadeOutDuration = options.fadeOutDuration;
            edgeName = options.edgeName;
            gameObject.name = options.name;

            SetParticleColor(options.color);
            SetParticleAlpha(0);
            SetParticleSize(options.size);
        }

        public void Update()
        {


            animaDuration += Time.deltaTime;
            if (motionType == MotionTypes.Init)
            {
                SetParticleColor(options.color);
                SetParticleAlpha(0);
                return;
            }

            bool isFadeMotion = motionType == MotionTypes.FadeIn || motionType == MotionTypes.FadeOut || motionType == MotionTypes.FadeFar;

            if (isFadeMotion)
            {
                Fade();
            }
            else
            {
                Floating();
            }
        }


        public void FadeIn()
        {

            motionType = MotionTypes.FadeIn;
            currentAlpha = GetParticleAlpha();
            // CLONE CURRENT POSITION
            currentPosition = gameObject.transform.position;
            currentScale = GetParticleSize();
            motionDelta = rectPosition - currentPosition;
            ease = 0f;
        }

        public void FadeOut()
        {
            motionType = MotionTypes.FadeOut;
            currentAlpha = GetParticleAlpha();
            currentPosition = gameObject.transform.position;
            currentScale = GetParticleSize();
            motionDelta = spawnPosition - currentPosition;
            ease = 0f;
        }


        public void FadeFar()
        {
            motionType = MotionTypes.FadeFar;
            currentAlpha = GetParticleAlpha();
            currentPosition = gameObject.transform.position;
            currentScale = GetParticleSize();
            motionDelta = sinkPosition - currentPosition;
            ease = 0f;
        }



        void Fade()
        {
            float easeValue = 0;
            float newAlpha = 0;
            // Vector3 newSize = new Vector3(0, 0, 0);
            if (motionType == MotionTypes.FadeIn)
            {
                ease += Time.deltaTime / fadeInDuration;
                // ease = Mathf.Clamp(ease, 0, 1);
                easeValue = CubicEaseOut(ease);
                newAlpha = Mathf.Lerp(currentAlpha, 0.3f, ease);
                // newSize = Vector3.Lerp(currentScale, options.scale, ease);
            }
            else if (motionType == MotionTypes.FadeOut)
            {
                ease += Time.deltaTime / fadeOutDuration;
                // ease = Mathf.Clamp(ease, 0, 1);
                easeValue = CubicEaseOut(ease);
                newAlpha = Mathf.Lerp(currentAlpha, 0, ease);
                // newSize = Vector3.Lerp(currentScale, Vector3.zero, ease);
            }
            else if (motionType == MotionTypes.FadeFar)
            {
                ease += Time.deltaTime / fadeOutDuration;
                // ease = Mathf.Clamp(ease, 0, 1);
                easeValue = CubicEaseOut(ease);
                newAlpha = Mathf.Lerp(currentAlpha, 0, ease);
                // newSize = Vector3.Lerp(currentScale, Vector3.zero, ease);

            }

            // ease  = Mathf.Clamp(ease, 0, 1);
            // Debug.Log("easeValue:" + easeValue);

            if (ease <= 1.0f )
            {
                Vector3 newPosition = Vector3.Lerp(currentPosition, currentPosition + motionDelta, easeValue);
                // float randomValue = Mathf.PerlinNoise(ease * 100, ease * 100) - 0.5f;
                // newPosition += randomValue * Vector3.one * 0.1f;
                // float time = animaDuration;
                // float sinX = Mathf.Sin(time * frequency);
                // float xOffset = amplitude.x * sinX;
                // float yOffset = amplitude.y * Mathf.Cos(time * frequency);
                // float zOffset = amplitude.z * sinX;
                // newPosition += new Vector3(xOffset, yOffset, zOffset) * speed;

                SetParticlePosition(newPosition);
                SetParticleAlpha(newAlpha);
                // SetParticleSize(newSize);
            }
            else
            {
                AfterFadeAnimation();
            }

        }

        void Floating()
        {
            // return;
            floatingDuration += Time.deltaTime;

            float smoothRatio =  floatingDuration > 1.0f ? 1.0f : floatingDuration; 

            float randomValue = Mathf.PerlinNoise(floatingDuration, 0f) - 0.5f;

            randomValue *= smoothRatio;
            // Vector3 offset = randomValue / 3f * amplitude;
            // Vector3 newPosition = currentPosition + offset;
            // gameObject.transform.position = newPosition;

            float time = floatingDuration;
            float sinX = Mathf.Sin(time * frequency);
            float xOffset = amplitude.x * sinX;
            // float yOffset = amplitude.y * Mathf.Cos(time * frequency) *  Mathf.Sin(time * frequency);
            float yOffset = amplitude.y * Mathf.Cos(time * frequency) * smoothRatio;
            float zOffset = amplitude.z * sinX ;
            Vector3 newPosition = currentPosition + new Vector3(xOffset, yOffset, zOffset) * speed + randomValue * Vector3.one * 0.1f;
            // 移动粒子
            gameObject.transform.position = newPosition;
        }

        void AfterFadeAnimation()
        {
            if (motionType == MotionTypes.FadeIn)
            {
                motionType = MotionTypes.Floating;
                SetParticleAlpha(0.3f);
                // SetParticlePosition(rectPosition);
                currentPosition = GetParticlePosition();
                floatingDuration = 0f;
            }
            else if (motionType == MotionTypes.FadeFar)
            {
                motionType = MotionTypes.Init;
                SetParticleAlpha(0);
                SetParticlePosition(sinkPosition);
                currentPosition = sinkPosition;
            }
            else if (motionType == MotionTypes.FadeOut)
            {
                motionType = MotionTypes.Init;
                SetParticleAlpha(0);
                SetParticlePosition(spawnPosition);
                currentPosition = spawnPosition;
            }


        }

        void SetParticleAlpha(float alpha)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material;
                Color color = material.color;
                color.a = alpha;
                material.color = color;
            }
            else
            {
                Debug.Log("renderer is null");
            }
        }

        void SetParticleSize(Vector3 size)
        {
            gameObject.transform.localScale = size;
        }

        Vector3 GetParticleSize()
        {
            return gameObject.transform.localScale;
        }

        Color GetParticleColor()
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material;
                Color color = material.color;
                return color;
            }
            return Color.white;
        }


        void SetParticleColor(Color color)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material;
                material.color = color;
            }
        }

        float GetParticleAlpha()
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material;
                Color color = material.color;
                return color.a;
            }
            return 0f;
        }


        void SetParticlePosition(Vector3 position)
        {
            gameObject.transform.position = position;
        }
        Vector3 GetParticlePosition()
        {
            return gameObject.transform.position;
        }
        float EaseInQuart(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value + start;
        }

        float EaseOutQuart(float start, float end, float value)
        {
            value--;
            end -= start;
            return -end * (value * value * value * value - 1) + start;
        }

        float QuadraticEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return 2 * p * p;
            }
            else
            {
                return (-2 * p * p) + (4 * p) - 1;
            }
        }

        float CubicEaseOut(float p)
        {
            float f = (p - 1);
            return f * f * f + 1;
        }
        float CubicEaseIn(float p)
        {
            return p * p * p;
        }


        float EaseOutBack(float p)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(p - 1f, 3) + c1 * Mathf.Pow(p - 1f, 2);
        }

        float easeOutCirc(float x)
        {
            return Mathf.Sqrt(1 - Mathf.Pow(x - 1f, 2));
        }


    }
}

