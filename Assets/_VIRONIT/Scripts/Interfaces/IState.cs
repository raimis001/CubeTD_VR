using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

public interface IState {

    void Enter();
    void Execute();
    void Exit();
    

}
