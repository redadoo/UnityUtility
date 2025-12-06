using System;
using UnityEngine;
using System.Reflection;

namespace UnityUtility
{
    public static class ExtentionsMethods
    {
        public static void CopyCollidersFromPrefab(GameObject target, GameObject prefab)
        {
            foreach (var c in target.GetComponents<Collider2D>())
                GameObject.Destroy(c);

            var prefabColliders = prefab.GetComponents<Collider2D>();

            foreach (var col in prefabColliders)
            {
                Collider2D newCol = null;

                if (col is BoxCollider2D)
                    newCol = target.AddComponent<BoxCollider2D>();
                else if (col is CircleCollider2D)
                    newCol = target.AddComponent<CircleCollider2D>();
                else if (col is PolygonCollider2D)
                    newCol = target.AddComponent<PolygonCollider2D>();
                else if (col is CapsuleCollider2D)
                    newCol = target.AddComponent<CapsuleCollider2D>();
                else if (col is EdgeCollider2D)
                    newCol = target.AddComponent<EdgeCollider2D>();
                else
                    Debug.LogWarning("Collider type not supported: " + col.GetType());

                if (newCol == null)
                    continue;

                newCol.isTrigger = col.isTrigger;
                newCol.sharedMaterial = col.sharedMaterial;
                newCol.offset = col.offset;

                if (col is BoxCollider2D box && newCol is BoxCollider2D newBox)
                    newBox.size = box.size;

                if (col is CircleCollider2D circle && newCol is CircleCollider2D newCircle)
                    newCircle.radius = circle.radius;

                if (col is CapsuleCollider2D capsule && newCol is CapsuleCollider2D newCap)
                {
                    newCap.size = capsule.size;
                    newCap.direction = capsule.direction;
                }

                if (col is PolygonCollider2D poly && newCol is PolygonCollider2D newPoly)
                {
                    newPoly.points = poly.points;
                }

                if (col is EdgeCollider2D edge && newCol is EdgeCollider2D newEdge)
                {
                    newEdge.points = edge.points;
                }
            }
        }

        public static TComponent CopyComponent<TComponent>(this GameObject destination, TComponent originalComponent) where TComponent : Component
        {
            Type componentType = typeof(TComponent);
        
            Component copy = destination.AddComponent(componentType);

            FieldInfo[] fields = componentType.GetFields();

            foreach (FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(originalComponent));
            }

            return copy as TComponent;
        }

    }
}
