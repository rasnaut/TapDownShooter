using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponAiming : MonoBehaviour
{
  private MultiAimConstraint[] _constraints; // Массив ограничителей на несколько целей
  
  public void Init(Transform aim)
  {
    // Создаём объект ограничителя цели constraintSourceObject
    // С помощью метода CreateConstraintSourceObject()
    WeightedTransformArray constraintSourceObject = CreateConstraintSourceObject(aim);
    _constraints = GetComponentsInChildren<MultiAimConstraint>(true); // Присваиваем _constraints компоненты MultiAimConstraint из дочерних объектов

    // Проходим по всем элементам массива _constraints
    for (int i = 0; i < _constraints.Length; i++) {                // Устанавливаем источник объекта constraintSourceObject
      _constraints[i].data.sourceObjects = constraintSourceObject; // В свойство sourceObjects каждого элемента _constraints
    }
  }

  public void SetActive(bool value) { gameObject.SetActive(value); } // Делаем оружие активным или неактивным

  private WeightedTransformArray CreateConstraintSourceObject(Transform aim)
  {
    // Создаём переменную-массив constraintAimArray
    var constraintAimArray = new WeightedTransformArray(1); // Типа WeightedTransformArray со значением 1

    // Присваиваем первому элементу constraintAimArray значение нового объекта
    constraintAimArray[0] = new WeightedTransform(aim, 1); // Класса WeightedTransform с параметрами aim и 1
    
    return constraintAimArray; // Возвращаем значение constraintAimArray
  }
}
