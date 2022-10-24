#if UNITY_EDITOR
using System;
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
            private readonly UxmlEnumAttributeDescription<PhysicsDimensions> _physicsDimensions = new() { name = "physics-dimensions" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var collisionMatrix = ve as CollisionMatrix;
                collisionMatrix.physicsDimensions = _physicsDimensions.GetValueFromBag(bag, cc);
            }
        }

        public const string UssClassName = "collision-matrix";

        private readonly CollisionMatrixRowToggles[] _rows = new CollisionMatrixRowToggles[Layer.MaxCount];
        public IReadOnlyList<CollisionMatrixRowToggles> rows => _rows;

        private PhysicsDimensions _physicsDimensions;

        public PhysicsDimensions physicsDimensions
        {
            get => _physicsDimensions;
            set
            {
                _physicsDimensions = value;

                for (var i = 0; i < Layer.MaxCount; i++)
                {
                    var row = this.rows[i];
                    if (row == null) return;

                    var propertyName = this.physicsDimensions switch
                    {
                        PhysicsDimensions.Three => nameof(SerializableLayer._collisionMatrix),
                        PhysicsDimensions.Two => nameof(SerializableLayer._collisionMatrix2D),
                        _ => throw new ArgumentOutOfRangeException(),
                    };

                    row.bindingPath = bindingPath = $"Array.data[{i}].{propertyName}";
                }
            }
        }

        public CollisionMatrix()
        {
            this.AddToClassList(UssClassName);

            for (var i = 0; i < Layer.MaxCount; i++)
            {
                var row = new CollisionMatrixRowToggles { toggleCount = Layer.MaxCount - i };

                this.Add(row);
                _rows[i] = row;
            }

            this.physicsDimensions = default;
        }
    }
}
#endif
