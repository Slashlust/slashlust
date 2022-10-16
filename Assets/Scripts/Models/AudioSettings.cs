using System.Collections.Generic;
using UnityEngine;

#nullable enable

[System.Serializable]
class AudioSettings
{
  public List<AudioClip> npcDeathAudioClips = default!;
  public List<AudioClip> swordHitAudioClips = default!;
  public List<AudioClip> swordSwingAudioClips = default!;

  public AudioClip GetRandomNpcDeathAudioClip()
  {
    return npcDeathAudioClips[Random.Range(0, npcDeathAudioClips.Count)];
  }

  public AudioClip GetRandomSwordHitAudioClip()
  {
    return swordHitAudioClips[Random.Range(0, swordHitAudioClips.Count)];
  }

  public AudioClip GetRandomSwordSwingAudioClip()
  {
    return swordSwingAudioClips[Random.Range(0, swordSwingAudioClips.Count)];
  }
}
