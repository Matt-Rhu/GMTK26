using System;
using UnityEngine;

public class StandaloneUIAnimSequence : UIAnimSequencePlayer
{
    [Tooltip("So uhm, due to how I've been making this, you kinda need to keep default appear as the 1st one " +
             "and default disappear as the 2nd one for the legacy play functions to work properly")]
    [SerializeField] private AnimSequence[] sequences;
    
    
    #region EventsHandling
    // private void OnEnable()
    // {
    //     Inputs.Menus.SkipMenuAnim.performed += _ => SkipSequence();
    // }
    // private void OnDisable()
    // {
    //     Inputs.Menus.PreviousMenu.performed -= SkipSequence;
    // }
    // private void OnDestroy()
    // {
    //     Inputs.Menus.PreviousMenu.performed -= SkipSequence;
    // }
    #endregion
    
    
    public void Play(int index, Action callWhenFinished)
    {
        //handle counting from end of list
        if (index <= -1) index = sequences.Length - index;
        
        StartSequence(sequences[index], callWhenFinished);
    }
    public void Play(int index) {
        Play(index, null);
    }
    public void Play(string name, Action callWhenFinished = null)
    {
        foreach (AnimSequence seq in sequences)
        {
            if (seq.name != name) continue;
            StartSequence(seq, callWhenFinished);
            return;
        }
        Debug.LogWarning("No anim sequence named " + name + " was found");
    }
    //clunky legacy play functions so I don't need to edit all the scripts :)))))))
    public void Appear(Action callWhenFinished = null)
    {
        StartSequence(sequences[0], callWhenFinished);
    }
    public void Disappear(Action callWhenFinished = null)
    {
        StartSequence(sequences[1], callWhenFinished);
    }

}
