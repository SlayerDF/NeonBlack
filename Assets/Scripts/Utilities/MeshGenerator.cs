using UnityEngine;

public static class MeshGenerator
{
    public static Mesh GenerateLosMesh(float innerRadius, float outerRadius, float angle, float height, int segments)
    {
        // Initialize data
        var totalVertices = (segments + 1) * 2 + 1;
        var vertices = new Vector3[totalVertices + 6];
        var triangles = new int[segments * 12 + 6];
        var uv = new Vector2[totalVertices + 6];
        var normals = new Vector3[totalVertices + 6];

        // Create top point
        var topPoint = new Vector3(0, height, 0);
        var topPointIndex = totalVertices - 1;
        vertices[topPointIndex] = topPoint;
        uv[topPointIndex] = new Vector2(0.5f, 0.5f);
        normals[topPointIndex] = Vector3.up;

        // Calculate angles
        var angleStep = angle / segments;
        var halfAngle = angle / 2f;

        // Create vertices
        for (var i = 0; i <= segments; i++)
        {
            var currentAngle = -halfAngle + i * angleStep;
            var angleRad = Mathf.Deg2Rad * currentAngle;

            // Outer sector
            var xOuter = Mathf.Cos(angleRad) * outerRadius;
            var zOuter = Mathf.Sin(angleRad) * outerRadius;
            vertices[i] = new Vector3(xOuter, 0, zOuter);
            uv[i] = new Vector2((xOuter / outerRadius + 1) / 2, (zOuter / outerRadius + 1) / 2);
            normals[i] = new Vector3(xOuter, 0, zOuter).normalized;

            // Inner sector
            var xInner = Mathf.Cos(angleRad) * innerRadius;
            var zInner = Mathf.Sin(angleRad) * innerRadius;
            vertices[i + segments + 1] = new Vector3(xInner, 0, zInner);
            uv[i + segments + 1] = new Vector2((xInner / outerRadius + 1) / 2, (zInner / outerRadius + 1) / 2);
            normals[i + segments + 1] = new Vector3(xInner, 0, zInner).normalized;

            if (i != 0 && i != segments)
            {
                continue;
            }

            // Sides with copied vertices to create UV cut
            var index = i == 0 ? 6 : 3;
            var centerUV = new Vector2(0.5f, 0.5f);
            var outerUVDir = new Vector2(-(uv[i].x - 0.5f), uv[i].y - 0.5f);

            vertices[^index] = topPoint;
            uv[^index] = centerUV + outerUVDir.normalized * outerUVDir.magnitude / outerRadius * height;
            normals[^index] = Vector3.up;

            vertices[^(index - 1)] = vertices[i];
            uv[^(index - 1)] = uv[i];
            normals[^(index - 1)] = normals[i];

            vertices[^(index - 2)] = vertices[i + segments + 1];
            uv[^(index - 2)] = uv[i + segments + 1];
            normals[^(index - 2)] = normals[i + segments + 1];
        }

        // Create triangles
        for (var i = 0; i < segments; i++)
        {
            var outerIndex1 = i;
            var outerIndex2 = (i + 1) % (segments + 1);
            var innerIndex1 = i + segments + 1;
            var innerIndex2 = (i + 1) % (segments + 1) + segments + 1;

            // Bottom triangles
            triangles[i * 12] = outerIndex1;
            triangles[i * 12 + 1] = outerIndex2;
            triangles[i * 12 + 2] = innerIndex2;

            triangles[i * 12 + 3] = outerIndex1;
            triangles[i * 12 + 4] = innerIndex2;
            triangles[i * 12 + 5] = innerIndex1;

            // Front triangle
            triangles[i * 12 + 6] = outerIndex1;
            triangles[i * 12 + 7] = topPointIndex;
            triangles[i * 12 + 8] = outerIndex2;

            // Back triangle
            triangles[i * 12 + 9] = innerIndex1;
            triangles[i * 12 + 10] = innerIndex2;
            triangles[i * 12 + 11] = topPointIndex;
        }

        // Create side triangles
        triangles[^6] = vertices.Length - 6;
        triangles[^5] = vertices.Length - 5;
        triangles[^4] = vertices.Length - 4;

        triangles[^3] = vertices.Length - 3;
        triangles[^2] = vertices.Length - 1;
        triangles[^1] = vertices.Length - 2;

        // Create mesh from the data
        var mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles,
            uv = uv,
            normals = normals
        };
        mesh.RecalculateBounds();

        return mesh;
    }
}
