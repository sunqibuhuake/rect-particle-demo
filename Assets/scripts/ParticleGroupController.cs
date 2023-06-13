using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

public class Point2DMeta
{
    public string edgeName;
    public Vector3 spawnPosition;
    public Vector3 rectPosition;
    public Vector3 sinkPosition;
}

public class RectStepMeta
{
    public float width;
    public float height;
    public int total;
    public int level;
}

namespace RectUIParticle
{
    public class ParticleGroupController : MonoBehaviour
    {
        [BoxGroup("粒子")]
        [LabelText("Prefab 1")]
        public GameObject particlePrefab; // 粒子的预制体

        [BoxGroup("粒子")]
        [LabelText("Prefab 2")]
        public GameObject particlePrefab2; // 粒子的预制体

        [BoxGroup("粒子")]
        [LabelText("Prefab 3")]
        public GameObject particlePrefab3; // 粒子的预制体

        [BoxGroup("粒子")]
        [LabelText("密度")]
        [Range(1, 200)]
        public int density = 30; // 粒子密度，表示每个单位面积上的粒子数量

        [BoxGroup("粒子")]
        [LabelText("尺寸")]
        [MinMaxSlider(0.01f, 0.2f)]
        public Vector2 sizeRange = new Vector2(0.01f, 0.12f); // 粒子大小范围

        [BoxGroup("粒子")]
        [LabelText("最小色值")]
        public Color minColor = Color.white; // 最小颜色

        [BoxGroup("粒子")]
        [LabelText("最大色值")]
        public Color maxColor = Color.white; // 最大颜色

        [BoxGroup("粒子")]
        [LabelText("透明度")]
        [MinMaxSlider(0f, 1.0f)]
        public Vector2 alphaRange = new Vector2(0.3f, 1.0f); // 透明度范围

        [BoxGroup("运动")]
        [LabelText("速度范围")]
        [MinMaxSlider(0f, 1.0f)]
        public Vector2 speedRange = new Vector2(0.1f, 0.3f); // 粒子移动速度范围

        [BoxGroup("运动")]
        [LabelText("振幅范围")]
        [MinMaxSlider(0f, 3.0f)]
        public Vector2 amplitudeRange = new Vector2(0.01f, 1.0f); // 粒子振幅范围

        [BoxGroup("运动")]
        [LabelText("频率范围")]
        [MinMaxSlider(0f, 3.0f)]
        public Vector2 frequencyRange = new Vector2(0.1f, 1.0f); // 粒子频率范围

        [BoxGroup("运动")]
        [LabelText("消失距离")]
        [Range(0f, 50f)]
        public float sinkDistance = 20f; // 淡入距离

        private Particle[] particles;

        [BoxGroup("矩形形状")]
        [LabelText("位置")]
        public Vector3 center = new Vector3(0f, 0f, 2f); // 矩形的中心点

        [BoxGroup("矩形形状")]
        [LabelText("角度")]
        public Vector3 rotation = new Vector3(0f, 0f, 0f); // 矩形的旋转角度

        [BoxGroup("矩形形状")]
        [LabelText("宽度")]
        // [Range(0.1f, 10f)]
        public float rectWidth = 6f; // 矩形的宽度

        [BoxGroup("矩形形状")]
        [LabelText("高度")]
        // [Range(0.1f, 5f)]
        public float rectHeight = 4f; // 矩形的高度

        [BoxGroup("矩形形状")]
        [LabelText("深度")]
        // [Range(0.1f, 3f)]
        public float rectDepth = 2f; // 矩形的深度

        [BoxGroup("矩形形状")]
        [LabelText("边宽")]
        // [Range(0.1f, 1f)]
        public float strokeWidth = 0.5f; // 矩形边框的宽度

        [BoxGroup("矩形形状")]
        [LabelText("分层")]
        public int strokeStepCount = 10; // 矩形边框的分段数

        // [BoxGroup("Rect Shape"), HideLabel]
        // public RectShapeStruct rectShape;

        // [Serializable]
        // public struct RectShapeStruct
        // {
        //     public float width;
        //     public float height;
        //     public float depth;
        //     public float strokeWidth;
        //     public int strokeStepCount;
        // }


        private int numParticles = 0; // 粒子数量

        void Start()
        {
            RectStepMeta[] steps = CalcRectStepSequence(strokeStepCount); // 计算边框分段数
            for (int i = 0; i < steps.Length; i++)
            {
                numParticles += steps[i].total;
                Debug.Log("step " + i + " total: " + steps[i].total);
            }

            Debug.Log("numParticles: " + numParticles);

            particles = new Particle[numParticles];

            int index = 0;
            for (int i = 0; i < steps.Length; i++)
            {
                Point2DMeta[] points = CalcRandomRectPosition(
                    steps[i]
                );
                for (int j = 0; j < points.Length; j++)
                {
                    particles[index] = CreateParticle(index, points[j]);
                    index++;
                }
            }
        }

        Particle CreateParticle(int index, Point2DMeta point)
        {
            Particle particle = new Particle();

            string edgeName = point.edgeName;

            ParticleInitOptions options = new ParticleInitOptions();
            options.name = "Particle " + index;
            options.edgeName = edgeName;
            // options.particlePrefab = particlePrefab;
            options.particlePrefab = Random.Range(0f, 1f) > 0.5f ? particlePrefab : particlePrefab2;
            if (Random.Range(0f, 1f) > 0.5f)
            {
                options.particlePrefab = particlePrefab3;
            }

            options.color = Random.ColorHSV(
                minColor.r,
                maxColor.r,
                minColor.g,
                maxColor.g,
                minColor.b,
                maxColor.b,
                0,
                0
            );

            options.spawnPosition = point.spawnPosition + center;
            options.rectPosition = point.rectPosition + center;
            options.sinkPosition = point.sinkPosition + center;

            options.randomOffset = Random.Range(0f, 2f * Mathf.PI);

            if (edgeName == "top" || edgeName == "bottom")
            {
                options.amplitude = new Vector3(
                    Random.Range(rectWidth / 5, rectWidth / 2),
                    Random.Range(amplitudeRange.x, amplitudeRange.y),
                    Random.Range(amplitudeRange.x, amplitudeRange.y)
                );
            }
            else
            {
                options.amplitude = new Vector3(
                    Random.Range(amplitudeRange.x, amplitudeRange.y),
                    Random.Range(rectHeight / 5, rectHeight / 2),
                    Random.Range(amplitudeRange.x, amplitudeRange.y)
                );
            }
            options.frequency = Random.Range(frequencyRange.x, frequencyRange.y);

            options.fadeInDuration = 3f + Random.Range(0f, 0.5f);
            options.fadeOutDuration = 3f + Random.Range(0f, 0.5f);

            float randomSize = Random.Range(sizeRange.x, sizeRange.y);
            options.size = new Vector3(randomSize, randomSize, randomSize);
            options.speed = Random.Range(speedRange.x, speedRange.y);

            Debug.Log("particle size: " + options.size);
            Debug.Log("particle speed: " + options.speed);
            Debug.Log("particle amplitude: " + options.amplitude);
            Debug.Log("particle frequency: " + options.frequency);

            particle.Init(options);

            return particle;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Q key was pressed");
                for (int i = 0; i < numParticles; i++)
                {
                    Particle p = particles[i];
                    CancelParticleContinue(p);
                    StartParticleContinue(p, Random.Range(0f, 1.5f), p.FadeIn);
                }
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("W key was pressed");
                for (int i = 0; i < numParticles; i++)
                {
                    Particle p = particles[i];
                    CancelParticleContinue(p);
                    StartParticleContinue(p, Random.Range(0f, 1.5f), p.FadeFar);
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("W key was pressed");
                for (int i = 0; i < numParticles; i++)
                {
                    Particle p = particles[i];
                    CancelParticleContinue(p);
                    StartParticleContinue(p, Random.Range(0f, 1.5f), p.FadeOut);
                }
            }

            for (int i = 0; i < numParticles; i++)
            {
                if (particles[i] == null)
                {
                    continue;
                }
                else
                {
                    particles[i].Update();
                }
            }
        }

        IEnumerator ExecuteAfterDelay(float delay, System.Action action)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }

        void CancelParticleContinue(Particle p)
        {
            if (p.particleContinue != null)
            {
                StopCoroutine(p.particleContinue);
                p.particleContinue = null;
            }
        }

        void StartParticleContinue(Particle p, float delay, System.Action action)
        {
            p.particleContinue = StartCoroutine(ExecuteAfterDelay(delay, action));
        }

        RectStepMeta[] CalcRectStepSequence(int stepAmount)
        {
            int amount = Mathf.RoundToInt(
                density * (rectWidth + rectHeight) * strokeWidth * rectDepth
            ); // 根据密度和矩形面积计算粒子数量
            Debug.Log("target amount: " + amount);
            // int[] sequence = CalculateGeometricSequence(amount, stepAmount);
            int[] sequence = CalculateGeometricSequence2(stepAmount);

            RectStepMeta[] steps = new RectStepMeta[stepAmount];
            for (int i = 0; i < sequence.Length; i++)
            {
                float interval = strokeWidth / stepAmount * i;
                int total = sequence[i];
                RectStepMeta step = new RectStepMeta();
                step.total = total;
                step.width = interval * i * 2f + rectWidth;
                step.height = interval * i * 2f + rectHeight;
                step.level = i;
                steps[i] = step;
            }
            return steps;
        }

        Point2DMeta[] CalcRandomRectPosition(RectStepMeta step)
        {
            float width = step.width;
            float height = step.height;

            int upperLowerCount = (int)Mathf.RoundToInt(step.total * (width / (width + height))); // 上下边的点数
            int leftRightCount = step.total - upperLowerCount; // 左右边的点数

            Point2DMeta[] points = new Point2DMeta[step.total];

            float radius = Mathf.Sqrt(width * width + height * height) / 2f;

            float offset = 0.01f * step.level * step.level;

            float levelDepth = step.level * step.level / 10f;

            // 生成上下边的点
            for (int i = 0; i < upperLowerCount; i++)
            {
                float rectX = Random.Range(-width / 2f, width / 2f); // 随机横坐标
                float rectY = (i % 2 == 0) ? -height / 2f : height / 2f; // 交替选择上边界或下边界作为纵坐标

                float theta = Mathf.Atan2(rectY, rectX);
                if (theta < 0)
                {
                    theta += 2 * Mathf.PI;
                }

                float randomRadius = radius * 8 + Random.Range(radius, radius * 2f);
                float spawnX = randomRadius * Mathf.Cos(theta);
                float spawnY = randomRadius * Mathf.Sin(theta);

                Point2DMeta p = new Point2DMeta();

                p.rectPosition = new Vector3(
                    rectX + Random.Range(-offset, offset),
                    rectY + Random.Range(-offset, offset),
                    Random.Range(-levelDepth / 2f, levelDepth / 2f)
                );
                if (Random.Range(0f, 1f) < 0.03f)
                {
                    p.rectPosition.z += Random.Range(-3f, 3f);
                    p.rectPosition.x *= Random.Range(1.3f, 1.8f);
                    p.rectPosition.y *= Random.Range(1.3f, 1.8f);
                }
                p.spawnPosition = new Vector3(
                    spawnX,
                    spawnY,
                    Random.Range(-rectDepth / 2f, rectDepth / 2f)
                );
                p.sinkPosition = new Vector3(
                    spawnX * Random.Range(1.5f, 2f),
                    spawnY * Random.Range(1.5f, 2f),
                    Random.Range(10f, 20f)
                );
                p.edgeName = (i % 2 == 0) ? "bottom" : "top";
                points[i] = p;
            }

            // 生成左右边的点
            for (int i = 0; i < leftRightCount; i++)
            {
                float rectX = (i % 2 == 0) ? -width / 2f : width / 2f; // 交替选择左边界或右边界作为横坐标
                float rectY = Random.Range(-height / 2f, height / 2f); // 随机纵坐标

                float theta = Mathf.Atan2(rectY, rectX);
                if (theta < 0)
                {
                    theta += 2 * Mathf.PI;
                }

                float randomRadius = radius * 4 + Random.Range(0f, radius);
                float spawnX = randomRadius * Mathf.Cos(theta);
                float spawnY = randomRadius * Mathf.Sin(theta);

                Point2DMeta p = new Point2DMeta();

                p.rectPosition = new Vector3(
                    rectX + Random.Range(-offset, offset),
                    rectY + Random.Range(-offset, offset),
                    Random.Range(-levelDepth / 2f, levelDepth / 2f)
                );

                if (Random.Range(0f, 1f) < 0.03f)
                {
                    p.rectPosition.z += Random.Range(-3f, 3f);
                    p.rectPosition.x *= Random.Range(1.3f, 1.8f);
                    p.rectPosition.y *= Random.Range(1.3f, 1.8f);
                }

                p.spawnPosition = new Vector3(
                    spawnX,
                    spawnY,
                    Random.Range(-rectDepth / 2f, rectDepth / 2f)
                );
                p.sinkPosition = new Vector3(
                    spawnX * Random.Range(1.5f, 2f),
                    spawnY * Random.Range(1.5f, 2f),
                    Random.Range(10f, 20f)
                );

                p.edgeName = (i % 2 == 0) ? "left" : "right";
                points[i + upperLowerCount] = p;
            }

            return points;
        }

        static int[] CalculateGeometricSequence(float x, int l)
        {
            int[] sequence = new int[l];
            float r = Mathf.Pow(x, 1.0f / l); // 公比

            float a = x * (1f - r) / (1f - Mathf.Pow(r, l));
            // Debug.Log("a: " + a);
            // sequence[0] = Mathf.RoundToInt(a);
            for (int i = 0; i < l; i++)
            {
                int v = Mathf.RoundToInt(x / Mathf.Pow(r, i));
                if (v < 10)
                {
                    v = 10;
                }
                sequence[i] = v;
            }

            return sequence;
        }

        static int[] CalculateGeometricSequence2(int l)
        {
            int[] sequence = new int[l];

            for (int i = 0; i < l; i++)
            {
                float r = 1f  - (float)i / (float)l;
                float v  = Mathf.Pow(r, 3);
                int num = Mathf.RoundToInt(1000 * v);
                if (num < 3)
                {
                    num = 3;
                }
                sequence[i] = num;
            }

            return sequence;
        }


    }
}
