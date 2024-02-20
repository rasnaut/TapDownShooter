using UnityEngine;

public abstract class Character : MonoBehaviour
{
  private CharacterMovement _movement; // Движение персонажа
  private CharacterAiming   _aiming;   // Прицеливание персонажа
  private CharacterPart[]   _parts;    // Части персонажа
  private CharacterShooting _shooting; // Стрельба персонажа

  // Start is called before the first frame update
  void Start()
  {
    Init(); // Вызываем метод Init()
  }

  private void Init()
  {
    _movement = GetComponent<CharacterMovement>(); // Получаем компонент движения персонажа
    _aiming   = GetComponent<CharacterAiming>  (); // Получаем компонент прицеливания персонажа
    _shooting = GetComponent<CharacterShooting>(); // Получаем компонент стрельбы персонажа

    _parts = new CharacterPart[] { // Создаём новый массив частей персонажа
        _movement,                 // Элемент массива «Движение»
        _aiming  ,                 // Элемент массива «Прицеливание»
        _shooting                  // Элемент массива «Стрельба»
    };

    for (int i = 0; i < _parts.Length; i++) { // Проходим по всем элементам массива
      if (_parts[i]) {                        // Проверяем, существует ли текущий элемент
        _parts[i].Init();                     // Вызываем метод Init() для текущего элемента
      }
    }
  }
}
