using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    public class TriggerEventForwarder : MonoBehaviour
    {
        public GameObject target;

        public void OnTriggerEnter(Collider other)
        {
            this.target.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
        }

        public void OnTriggerExit(Collider other)
        {
            this.target.SendMessage("OnTriggerExit", other, SendMessageOptions.DontRequireReceiver);
        }

        public void OnTriggerStay(Collider other)
        {
            this.target.SendMessage("OnTriggerStay", other, SendMessageOptions.DontRequireReceiver);
        }
    }
}