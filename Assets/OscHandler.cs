using System;
using OscJack;
using Unity.VisualScripting;
using UnityEngine;

public class OscHandler : MonoBehaviour
{
    
    public OscVariable[] OscVars = new OscVariable[] {};

    private OscServer server;

    [Serializable]
    public class OscVariable {
        public string Address;
        public string SceneVarName;
    }

    void OnEnable() {
        VariableDeclarations activeScene = Variables.ActiveScene;
        int OscPort = (int)Variables.Application.Get("OSCPort");
        server = new OscServer(OscPort);

        foreach (OscVariable oscVar in OscVars) {
            server.MessageDispatcher.AddCallback(oscVar.Address, (string address, OscDataHandle data) => {
                float score = data.GetElementAsFloat(0);
                activeScene.Set(oscVar.SceneVarName, score);
            });
        }

    }

    void OnDisable() {
        server.Dispose();
        server = null;
    }
}
