using UnityEngine;

public abstract class Character : MonoBehaviour
{
  private CharacterPart[]   _parts;    // Части персонажа
  
  void Start() {
    Init(); // Вызываем метод Init()
  }

  private void Init() {
    _parts = GetComponents<CharacterPart>();
    for (int i = 0; i < _parts.Length; i++) { // Проходим по всем элементам массива
      if (_parts[i]) {                        // Проверяем, существует ли текущий элемент
        _parts[i].BaseInit();                 // Вызываем метод Init() для текущего элемента
      }
    }
    InitDeath(); // Вызываем метод InitDeath()
  }

  private void InitDeath() {
    for (int i = 0; i < _parts.Length; i++) // Проходим по всем частям
    {
      // Если часть — экземпляр класса CharacterHealth
      if (_parts[i] is CharacterHealth health) { // Подписываемся на событие OnDie объекта health
        health.OnDie += Stop;                    // Указываем, что при возникновении события должен выполниться метод Stop()
      }
    }
  }

  private void Stop() {
    for (int i = 0; i < _parts.Length; i++) { // Проходим по всем частям
      _parts[i].BaseStop();                       // Вызываем метод Stop() для каждой части
    }
  }
}
