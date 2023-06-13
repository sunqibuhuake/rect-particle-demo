using UnityEngine;

public class ParticlePrefabController : MonoBehaviour
{
    public Sprite sprite; // 粒子的Sprite
    public Material material; // 粒子的发光材质

    void Start()
    {
        // 添加SpriteRenderer组件，并设置Sprite和Material
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.material = material;
    }
}