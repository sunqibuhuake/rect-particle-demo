using UnityEngine;
using System.Collections;


public class Point2DMeta
{

    public string edgeName;
    public Vector3 spawnPosition;
    public Vector3 rectPosition;
    public Vector3 disapearPosition;

}

public class RectStepMeta
{
    public float width;
    public float height;
    public int total;

}



namespace RectUIParticle
{






    public class ParticleGroupController : MonoBehaviour
    {
        public GameObject particlePrefab; // 粒子的预制体
        // public GameObject particlePrefab2; // 粒子的预制体
        // public GameObject particlePrefab3; // 粒子的预制体
        public int density = 50; // 粒子密度，表示每个单位面积上的粒子数量
        public float minSize = 0.01f; // 最小粒子大小
        public float maxSize = 0.12f; // 最大粒子大小
        public Color minColor = Color.white; // 最小颜色
        public Color maxColor = Color.white; // 最大颜色
        public float minAlpha = 0.3f; // 最小透明度
        public float maxAlpha = 1.0f; // 最大透明度
        public float speedMin = 0.1f; // 最小粒子移动速度
        public float speedMax = 0.3f; // 最大粒子移动速度
        public float amplitudeMin = 0.01f; // 最小运动振幅
        public float amplitudeMax = 1.0f; // 最大运动振幅
        public float frequencyMin = 0.1f; // 最小运动频率
        public float frequencyMax = 1f; // 最大运动频率


        public float fadeInDistance = 1f; // 淡入距离

        private Particle[] particles;

        public Vector3 center = new Vector3(0f, 0f, 2f); // 矩形的中心点
        public float width = 6f; // 矩形的宽度
        public float height = 4f; // 矩形的高度
        public float depth = 2f; // 矩形的深度


        public float strokeWidth = 0.5f; // 矩形边框的宽度
        public int strokeStepCount = 10; // 矩形边框的分段数
        private int numParticles = 1000; // 粒子数量


        void Start()
        {
            numParticles = Mathf.RoundToInt(density * (width + height) * strokeWidth * depth); // 根据密度和矩形面积计算粒子数量
            particles = new Particle[numParticles];
            RectStepMeta[] steps = CalcRectStep(numParticles, strokeStepCount, width, height, strokeWidth); // 计算边框分段数

            Debug.Log("numParticles: " + numParticles);
            Debug.Log("steps.Length: " + steps.Length);

            int index = 0;
            for (int i = 0; i < steps.Length; i++)
            {
                Debug.Log("step index: " + i);
                Debug.Log("steps[i].total: " + steps[i].total);
                for (int j = 0; j < steps[i].total; j++)
                {
                    Point2DMeta[] points = CalcRandomRectPosition(steps[i].width, steps[i].height, steps[i].total);
                    for (int k = 0; k < points.Length; k++)
                    {
                        if (index >= numParticles)
                        {
                            break;
                        }
                        particles[index] = CreateParticle(index, points[k]);
                        index++;
                    }
                }
            }


            // for (int i = 0; i < numParticles; i++)
            // {
            //     particles[i] = CreateParticle(i);
            // }
        }


        Particle CreateParticle(int index, Point2DMeta point)
        {

            Debug.Log("CreateParticle index: " + index);

            Particle particle = new Particle();


            string edgeName = point.edgeName;


            ParticleInitOptions options = new ParticleInitOptions();
            options.name = "Particle " + index;
            options.edgeName = edgeName;
            options.particlePrefab = particlePrefab;
            // options.particlePrefab = Random.Range(0f, 1f) > 0.5f ? particlePrefab : particlePrefab2;
            // if (Random.Range(0f, 1f) > 0.8f)
            // {
            //     options.particlePrefab = particlePrefab3;
            // }

            options.color = Random.ColorHSV(minColor.r, maxColor.r, minColor.g, maxColor.g, minColor.b, maxColor.b, 0, 0);

            options.spawnPosition = point.spawnPosition + center;
            options.rectPosition = point.rectPosition + center;
            options.disapearPosition = point.disapearPosition + center;

            options.randomOffset = Random.Range(0f, 2f * Mathf.PI);
            options.speed = Random.Range(speedMin, speedMax);

            if (edgeName == "top" || edgeName == "bottom")
            {
                options.amplitude = new Vector3(Random.Range(width / 5, width / 2), Random.Range(amplitudeMin, amplitudeMax), Random.Range(amplitudeMin, amplitudeMax));

            }
            else
            {
                options.amplitude = new Vector3(Random.Range(amplitudeMin, amplitudeMax), Random.Range(height / 5, height / 2), Random.Range(amplitudeMin, amplitudeMax));
            }
            options.frequency = Random.Range(frequencyMin, frequencyMax);
            options.fadeInDuration = 2.0f;
            options.fadeOutDuration = 2.0f;

            float randomSize = Random.Range(minSize, maxSize);
            options.scale = new Vector3(randomSize, randomSize, randomSize);
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
                particles[i].Update();
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

        RectStepMeta[] CalcRectStep(int N, int L, float rectWidth, float rectHeight, float rectStrokeWidth)
        {
            float interval = rectStrokeWidth / L;
            RectStepMeta[] steps = new RectStepMeta[L];
            float r = Mathf.Pow(N, 1f / L);
            Debug.Log("r:" + r);
            int a = (int)(N / Mathf.Pow(r, L));
            Debug.Log("a:" + a);
            for (int i = 0; i < L; i++)
            {
                // int b = (int)Mathf.RoundToInt((a * Mathf.Pow(r, i)));
                int b = (int)Mathf.RoundToInt(N / L);
                Debug.Log("b:" + b);
                RectStepMeta step = new RectStepMeta();
                step.total = b;
                step.width = interval * i * 2f + rectWidth;
                step.height = interval * i * 2f + rectHeight;
                steps[i] = step;
            }
            return steps;
        }




        Point2DMeta[] CalcRandomRectPosition(float width, float height, int n)
        {


            int upperLowerCount = (int)Mathf.RoundToInt(n * (width / (width + height))); // 上下边的点数
            int leftRightCount = n - upperLowerCount; // 左右边的点数

            Point2DMeta[] points = new Point2DMeta[n];

            // 生成上下边的点
            for (int i = 0; i < upperLowerCount; i++)
            {
                float rectX = Random.Range(-width / 2f, width / 2f); // 随机横坐标
                float rectY = (i % 2 == 0) ? -height / 2f : height / 2f; // 交替选择上边界或下边界作为纵坐标

                float spawnX = rectX * 2f;
                float spawnY = rectY * Random.Range(1.5f, 2f);

                Point2DMeta p = new Point2DMeta();

                p.rectPosition = new Vector3(rectX + Random.Range(-0.1f, 0.1f), rectY + Random.Range(-0.1f, 0.1f), Random.Range(-depth / 2f, depth / 2f));
                p.spawnPosition = new Vector3(spawnX, spawnY, Random.Range(-depth / 2f, depth / 2f));
                p.disapearPosition = new Vector3(spawnX * Random.Range(1.5f, 2f), spawnY * Random.Range(1.5f, 2f), Random.Range(10f, 20f));
                p.edgeName = (i % 2 == 0) ? "bottom" : "top";
                points[i] = p;
            }

            // 生成左右边的点
            for (int i = 0; i < leftRightCount; i++)
            {
                float rectX = (i % 2 == 0) ? -width / 2f : width / 2f; // 交替选择左边界或右边界作为横坐标
                float rectY = Random.Range(-height / 2f, height / 2f); // 随机纵坐标

                float spawnX = rectX * Random.Range(1.5f, 2f);
                float spawnY = rectY * 2f;

                Point2DMeta p = new Point2DMeta();

                p.rectPosition = new Vector3(rectX + Random.Range(-0.1f, 0.1f), rectY + Random.Range(-0.1f, 0.1f), Random.Range(-depth / 2f, depth / 2f));
                p.spawnPosition = new Vector3(spawnX, spawnY, Random.Range(-depth / 2f, depth / 2f));
                p.disapearPosition = new Vector3(spawnX * Random.Range(1.5f, 2f), spawnY * Random.Range(1.5f, 2f), Random.Range(10f, 20f));

                p.edgeName = (i % 2 == 0) ? "left" : "right";
                points[i + upperLowerCount] = p;

            }

            return points;
        }




    }




}

