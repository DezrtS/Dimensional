using System;
using System.Collections.Generic;
using System.Linq;
using Scriptables.Actions.Movement;
using Scriptables.Shapes;
using Scriptables.User_Interface;
using Systems.Actions.Movement;
using UnityEngine;

namespace Scriptables.Save
{
    [Serializable]
    public class NewActionShapes
    {
        public MovementActionType movementActionType;
        public List<ShapeType> shapeTypes;
    }
    
    [Serializable]
    public class NewShapesActions
    {
        public List<NewActionShapes> newActionShapes;
    }
    
    [CreateAssetMenu(fileName = "NewShapes", menuName = "Scriptable Objects/Save/NewShapes")]
    public class NewShapes : SaveVariable
    {
        [Space]
        [SerializeField] private MovementActionShapesDatum[] movementActionShapesData;
        [Space]
        [SerializeField] private NewShapesActions defaultValue;
        
        private NewShapesActions _runtimeValue;

        private Dictionary<MovementActionType, List<ShapeType>> _newActionShapes;
        private Dictionary<MovementActionType, List<ShapeType>> _movementActionShapesDictionary;

        public void SetNewShape(ShapeType shapeType)
        {
            foreach (var newActionShape in _runtimeValue.newActionShapes.Where(newActionShape => _movementActionShapesDictionary[newActionShape.movementActionType].Contains(shapeType)).Where(newActionShape => !_newActionShapes[newActionShape.movementActionType].Contains(shapeType)))
            {
                _newActionShapes[newActionShape.movementActionType].Add(shapeType);
            }
            
            SetIsDirty();
        }
        
        public void RemoveNewShape(MovementActionType movementActionType, ShapeType shapeType)
        {
            if (_newActionShapes[movementActionType].Contains(shapeType))
            {
                _newActionShapes[movementActionType].Remove(shapeType);
                foreach (var newActionShape in _runtimeValue.newActionShapes.Where(newActionShape => newActionShape.movementActionType == movementActionType))
                {
                    newActionShape.shapeTypes.Remove(shapeType);
                }
            }
            SetIsDirty();
        }

        public override string Capture()
        {
            base.Capture();
            return JsonUtility.ToJson(_runtimeValue);
        }

        public override void Restore(string data)
        {
            base.Restore(data);
            _runtimeValue = JsonUtility.FromJson<NewShapesActions>(data);
            SetupMovementActionShapesDictionary();
        }

        public override void Reset()
        {
            base.Reset();
            _runtimeValue ??= new NewShapesActions();
            _runtimeValue.newActionShapes ??= new List<NewActionShapes>();
            _runtimeValue.newActionShapes.Clear();
            foreach (var newActionShape in defaultValue.newActionShapes)
            {
                _runtimeValue.newActionShapes.Add(new NewActionShapes() { movementActionType = newActionShape.movementActionType, shapeTypes = new List<ShapeType>(newActionShape.shapeTypes) });
            }
            
            SetupMovementActionShapesDictionary();
        }

        private void SetupMovementActionShapesDictionary()
        {
            _movementActionShapesDictionary = new Dictionary<MovementActionType, List<ShapeType>>();
            foreach (var movementActionShapeDatum in movementActionShapesData)
            {
                var shapeList = movementActionShapeDatum.ShapeData.Select(shapeDatum => shapeDatum.ShapeType).ToList();
                _movementActionShapesDictionary.Add(movementActionShapeDatum.MovementActionType, shapeList);
            }

            _newActionShapes = new Dictionary<MovementActionType, List<ShapeType>>();
            foreach (var newActionShape in _runtimeValue.newActionShapes)
            {
                var shapeTypes = newActionShape.shapeTypes.Where(shapeType => _movementActionShapesDictionary[newActionShape.movementActionType].Contains(shapeType)).ToList();
                _newActionShapes.Add(newActionShape.movementActionType, shapeTypes);
            }
        }

        public List<ShapeType> GetNewActionShapes(MovementActionType movementActionType)
        {
            if (_runtimeValue?.newActionShapes == null) Reset();
            return _newActionShapes.GetValueOrDefault(movementActionType);
        }
    }
}
