using UnityEngine;

public class AxesHelper : MonoBehaviour
{
    public float axisLength = 1.0f;
    public float lineWidth = 0.02f;
    public Material lineMaterial;

    private void OnDrawGizmos()
    {
        if (lineMaterial == null)
            return;

        // 绘制X轴线段
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * axisLength);

        // 绘制Y轴线段
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * axisLength);

        // 绘制Z轴线段
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * axisLength);

        // 重设Gizmos的矩阵
        Gizmos.matrix = Matrix4x4.identity;
    }

    private void OnRenderObject()
    {
        if (lineMaterial == null)
            return;

        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        GL.Begin(GL.LINES);

        // 绘制X轴线段
        GL.Color(Color.red);
        GL.Vertex(Vector3.zero);
        GL.Vertex(transform.right * axisLength);

        // 绘制Y轴线段
        GL.Color(Color.green);
        GL.Vertex(Vector3.zero);
        GL.Vertex(transform.up * axisLength);

        // 绘制Z轴线段
        GL.Color(Color.blue);
        GL.Vertex(Vector3.zero);
        GL.Vertex(transform.forward * axisLength);

        GL.End();
        GL.PopMatrix();
    }
}
