using UnityEngine;
using Zenject;
namespace Props
{
    public class RandomPropsSystem : IInitializable
    {
        public RandomPropsSystem(GameObject propWithRb,Transform propsHolder)
        {
            _propWithRb = propWithRb;
            _propsHolder = propsHolder;
        }
        readonly GameObject _propWithRb;
        readonly Transform _propsHolder;
        public void Initialize()
        {
            CreateProps(randAmount: new Vector2Int(20, 50),
                        center: new Vector3(5f, -20f, -9f),
                        randShift: Vector3.one * 3f,
                        randScaleFrom: Vector3.one * 0.2f,
                        randScaleTo: Vector3.one * 2f);
        }
        void CreateProps(Vector2Int randAmount, Vector3 center, Vector3 randShift, Vector3 randScaleFrom, Vector3 randScaleTo)
        {
            for (int i = 0; i < Random.Range(randAmount.x, randAmount.y); i++)
            {
               var spawned = GameObject.Instantiate(original: _propWithRb,
                                                    position: center + new Vector3(x: Random.Range(-randShift.x, randShift.x),
                                                                                 y: Random.Range(-randShift.y, randShift.y),
                                                                                 z: Random.Range(-randShift.z, randShift.z)),
                                                    rotation:Quaternion.identity);
                spawned.transform.localScale = new Vector3(x: Random.Range(-randScaleFrom.x, randScaleTo.x),
                                                           y: Random.Range(-randScaleFrom.y, randScaleTo.y),
                                                           z: Random.Range(-randScaleFrom.z, randScaleTo.z));
                spawned.transform.SetParent(_propsHolder,worldPositionStays:true);
            }
        }

    }
}
