using UnityEngine;

public abstract class CharacterPart : MonoBehaviour
{
  protected bool IsActive;    // Флаг активности персонажа

  // Метод инициализации
  public void BaseInit()
  {
    IsActive = true; // Делаем персонажа активным
    Init();          // Вызываем метод Init() Специфичный для дочерних объектов
  }

  // Метод остановки персонажа
  public void BaseStop()
  {
    IsActive = false; // Делаем персонажа неактивным
    Stop();           // Вызываем метод Stop() Специфичный для дочерних объектов
  }

  protected virtual void Init() {} // Виртуальный метод инициализации 
  protected virtual void Stop() {} // Виртуальный метод остановки 
}
