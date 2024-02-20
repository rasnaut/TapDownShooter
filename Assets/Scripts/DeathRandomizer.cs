using UnityEngine;

public class DeathRandomizer : StateMachineBehaviour
{
  private const string DeathIdKey  = "DeathId"; // Константа с ключом смерти персонажа
  private const int AnimationCount = 3;         // Константа с числом анимаций

  override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
    
    animator.SetInteger(DeathIdKey, Random.Range(0, AnimationCount)); // Запускаем случайную анимацию из заданных
  }
}
