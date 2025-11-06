using System;

[Serializable]
public class StatusEffectInstance
{
    public StatusEffect Effect;
    public int Stack;
    public float Remaining;

    public StatusEffectInstance(StatusEffect effect, int stack, float duration)
    {
        Effect = effect;
        Stack = stack;
        Remaining = duration;
    }
}