using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public static class Util
{
    public static string Name(this AttackDirection direction)
    {
        return direction switch
        {
            AttackDirection.Right => "Attack_Right",
            AttackDirection.DownRight => "Attack_DownRight",
            AttackDirection.RightUp => "Attack_RightUp",
            AttackDirection.UpRightHand => "Attack_Up_RightHand",
            AttackDirection.Left => "Attack_Left",
            AttackDirection.DownLeft => "Attack_DownLeft",
            AttackDirection.LeftUp => "Attack_LeftUp",
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public static T Random<T>(this IEnumerable<T> enumerable)
    {
        List<T> list = enumerable?.ToList();
        if (list == null || list.Count == 0)
        {
            return default;
        }

        int idx = new Random().Next(0, list.Count);
        return list[idx];
    }
    
    public static IEnumerator PlaySoundWithDelay(this Component gameObject, AudioClip audioClip, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioSource.PlayClipAtPoint(audioClip, gameObject.transform.position);
    }
}
