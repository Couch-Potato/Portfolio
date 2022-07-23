using Microsoft.Xna.Framework;
using Novovu.Xenon.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Engine
{
    public class EventModel
    {
        public delegate void TransformDelegate(Vector3 UpdatedVector);

        public event TransformDelegate SelectionScale;
        public event TransformDelegate SelectionRotate;
        public event TransformDelegate SelectionTranslate;

        public delegate void GameObjectSelected(GameObject obj);

        public delegate void GameModelSelected(BasicModel mdl);

        public event GameObjectSelected ObjectSelected;

        public event GameModelSelected ModelSelected;


        public enum ChangeType { Scale, Rotate, Translate}
        public void InvokeChange(ChangeType type, Vector3 nv)
        {
            switch (type)
            {
                case ChangeType.Rotate:
                    SelectionRotate?.Invoke(nv);
                    break;
                case ChangeType.Scale:
                    SelectionScale?.Invoke(nv);
                    break;
                case ChangeType.Translate:
                    SelectionTranslate?.Invoke(nv);
                    break;
            }
        }
        public void InvokeSelection(GameObject obj)
        {
            ObjectSelected?.Invoke(obj);
        }
        public void InvokeSelection(BasicModel obj)
        {
            ModelSelected?.Invoke(obj);
        }
        public void InvokeSelection(ITransformable obj)
        {
            if (obj is GameObject)
            {
                ObjectSelected?.Invoke((GameObject)obj);
            }
            if (obj is BasicModel)
            {
                ModelSelected?.Invoke((BasicModel)obj);
            }
        }


    }
}
