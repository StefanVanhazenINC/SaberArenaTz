using System.Linq;

namespace _Project.Scripts._Common.Utility
{
    using System.Collections.Generic;
    using UnityEngine;
    public static class ComponentFinder
    {
        public static List<T> FindAllInHierarchy<T>(Transform start) where T : Component
        {
            var result = new List<T>();
            //result.AddRange(start.GetComponents<T>());
            // Поиск в самом объекте и всех детях
            result.AddRange(start.GetComponentsInChildren<T>(includeInactive: true));

            // Поднимаемся вверх по иерархии и ищем в родителях
            var current = start.parent;
            while (current != null)
            {
                result.AddRange(current.GetComponents<T>());
                current = current.parent;
            }

            return result;
        }
        //искать в иерархии первое вхождение где в названии есть "String" заданный и нужный тип 
        public static T FindFirstInHierarchy<T>(Transform start) where T : Component
        {
            var result = new List<T>();
           
            result.AddRange(start.GetComponentsInChildren<T>(includeInactive: true));

            // Поднимаемся вверх по иерархии и ищем в родителях
            var current = start.parent;
            while (current != null)
            {
                result.AddRange(current.GetComponents<T>());
                current = current.parent;
            }

            return result[0];
        }
        public static List<T> FindAllUpHierarchy<T>(Transform start) where T : Component
        {
            return start.GetComponentsInParent<T>(includeInactive: true).ToList();
        }
        public static List<T> FindAllUpHierarchyParentsOnly<T>(Transform start) where T : Component
        {
            var result = new List<T>();

            var current = start.parent;
            while (current != null)
            {
                result.AddRange(current.GetComponents<T>());
                current = current.parent;
            }

            return result;
        }
        public static T FindFirstUpHierarchy<T>(Transform start) where T : Component
        {
            return start.GetComponentInParent<T>(includeInactive: true);
        }
        public static T FindFirstInParents<T>(Transform start) where T : Component
        {
            var current = start.parent;

            while (current != null)
            {
                var component = current.GetComponent<T>();
                if (component != null)
                    return component;

                current = current.parent;
            }

            return null;
        }
    }
}