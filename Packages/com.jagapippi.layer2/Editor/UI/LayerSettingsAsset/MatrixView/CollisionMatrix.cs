using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Jagapippi.Layer2.Editor.UIElements
{
    internal class CollisionMatrix : BindableElement
    {
        public new class UxmlFactory : UxmlFactory<CollisionMatrix, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
        }

        public const string UssClassName = "collision-matrix";

        private readonly CollisionMatrixRowToggles[] _rows = new CollisionMatrixRowToggles[Layer.MaxCount];
        public IReadOnlyList<CollisionMatrixRowToggles> rows => _rows;

        public CollisionMatrix()
        {
            this.AddToClassList(UssClassName);

            for (var i = 0; i < Layer.MaxCount; i++)
            {
                var row = new CollisionMatrixRowToggles
                {
                    toggleCount = Layer.MaxCount - i,
                    bindingPath = $"Array.data[{i}].{nameof(SerializableLayer._collisionMatrix)}",
                };

                this.Add(row);
                _rows[i] = row;
            }
        }
    }
}
